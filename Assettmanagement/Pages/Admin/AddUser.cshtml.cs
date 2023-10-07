using System.Threading.Tasks;
using Assettmanagement.Data;
using Assettmanagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using System.Formats.Asn1;
using Assettmanagement.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Assettmanagement.Pages.Admin
{
    [Authorize(Policy = "AdministratorOnly")]
    public class AddUserModel : PageModel
    {
        private readonly IDataAccess _dataAccess;

        public AddUserModel(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [BindProperty]
        public User NewUser { get; set; }
        [BindProperty]
        public IFormFile ImportFile { get; set; }
        [BindProperty(SupportsGet = true)]
        public int SelectedUserId { get; set; }
        public SelectList UserList { get; set; }
        [BindProperty]
        public string ResultMessage { get; set; }
        [BindProperty(SupportsGet = true)] 
        public bool IsEditMode { get; set; }

        public async Task OnGetAsync(bool reset = false)
        {
            var users = await _dataAccess.GetUsersAsync();

            if (TempData["ResultMessage"] != null)
            {
                ResultMessage = TempData["ResultMessage"].ToString();
            }

            UserList = new SelectList(users, "Id", "LastName", "FirstName");
            if (SelectedUserId > 0 && !reset)
            {
                NewUser = users.FirstOrDefault(u => u.Id == SelectedUserId);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (SelectedUserId > 0)
            {
                NewUser = await _dataAccess.GetUserByIdAsync(SelectedUserId);
                return RedirectToPage(new { SelectedUserId = SelectedUserId, IsEditMode = IsEditMode });
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteUserAsync()
        {
            if (SelectedUserId > 0)
            {
                await _dataAccess.DeleteUserAsync(SelectedUserId);
                // Optionally reset the NewUser model and SelectedUserId
                TempData["ResultMessage"] = $"User '{NewUser.FirstName} {NewUser.LastName}' deleted successfully!";
                NewUser = new User();
                SelectedUserId = 0;
            }
            else
            {
                TempData["ResultMessage"] = "Invalid User index";
            }
            return RedirectToPage(new { SelectedUserId = SelectedUserId, IsEditMode = IsEditMode });
        }

        public async Task<IActionResult> OnPostSaveUserAsync()
        {
            // Remove ImportFile validation if not importing users
            ModelState.Remove("ImportFile");
            ModelState.Remove("NewUser.Id");
            ModelState.Remove("ResultMessage");
            if (!ModelState.IsValid)
                return RedirectToPage(new { SelectedUserId = SelectedUserId, IsEditMode = IsEditMode });

            NewUser.PasswordHash = SecurityHelper.HashPassword(NewUser.PasswordHash);

            if (!IsEditMode) // New user
            {
                await _dataAccess.AddUserAsync(NewUser);
                TempData["ResultMessage"] = $"User '{NewUser.FirstName} {NewUser.LastName}' added successfully!";
                NewUser = new User(); // Reset the User model for the next input
                ModelState.Clear(); // Ensure that the NewUser object and the ModelState are clear
            }
            else // Update existing user
            {
                await _dataAccess.UpdateUserAsync(NewUser);
                TempData["ResultMessage"] = $"User '{NewUser.FirstName} {NewUser.LastName}' updated successfully!";
            }
            return RedirectToPage(new { SelectedUserId = SelectedUserId, IsEditMode = IsEditMode });
        }

        public async Task<IActionResult> OnGetEditAsync(int userId)
        {
            NewUser = await _dataAccess.GetUserByIdAsync(userId);
            return RedirectToPage(new { SelectedUserId = SelectedUserId, IsEditMode = IsEditMode });
        }

        public async Task<IActionResult> OnPostImportUsersAsync()
        {
            if (ImportFile == null || ImportFile.Length == 0)
            {
                TempData["ResultMessage"] = "No file selected for import.";
                return RedirectToPage(new { SelectedUserId = SelectedUserId, IsEditMode = IsEditMode });
            }

            using (var reader = new StreamReader(ImportFile.OpenReadStream()))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<User>();
                foreach (var user in records)
                {
                    user.PasswordHash = SecurityHelper.HashPassword(user.PasswordHash);
                    await _dataAccess.AddUserAsync(user);
                }
            }

            TempData["ResultMessage"] = "Users imported successfully!";
            return RedirectToPage(new { SelectedUserId = SelectedUserId, IsEditMode = IsEditMode });
        }

        public async Task<IActionResult> OnPostExportUsersAsync()
        {
            var users = await _dataAccess.GetUsersAsync();
            var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream, leaveOpen: true)) // Notice the leaveOpen: true
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(users);
            }

            memoryStream.Position = 0;
            return new FileStreamResult(memoryStream, "text/csv")
            {
                FileDownloadName = "users.csv"
            };
        }


    }
}
