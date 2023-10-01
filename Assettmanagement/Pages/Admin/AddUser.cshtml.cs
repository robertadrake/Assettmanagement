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

namespace Assettmanagement.Pages.Admin
{
    public class AddUserModel : PageModel
    {
        private readonly DataAccess _dataAccess;

        public AddUserModel(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

       // [BindProperty]
        public IFormFile ImportFile { get; set; }

        [BindProperty]
        public User NewUser { get; set; }

        [TempData]
        public string ResultMessage { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
          /*  if (!ModelState.IsValid)
            {
                // Collect validation errors
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new
                    {
                        Field = x.Key,
                        ErrorMessages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    }).ToList();

                // Convert errors to a string for display
                StringBuilder errorMessages = new StringBuilder();
                foreach (var error in errors)
                {
                    errorMessages.AppendLine($"Field: {error.Field}");
                    foreach (var message in error.ErrorMessages)
                    {
                        errorMessages.AppendLine($"- {message}");
                    }
                }

                ResultMessage = $"Validation Errors: {errorMessages.ToString()}";
                return Page();
            }*/

            if (string.IsNullOrEmpty (NewUser.FirstName) || string.IsNullOrEmpty (NewUser.LastName)
                || string.IsNullOrEmpty (NewUser.Email) || string.IsNullOrEmpty(NewUser.Email))
                return Page();


            // Hash the password before saving to the database
            NewUser.PasswordHash = SecurityHelper.HashPassword(NewUser.PasswordHash);
            await _dataAccess.AddUserAsync(NewUser);
            ResultMessage = $"User '{NewUser.FirstName} {NewUser.LastName}' added successfully!";
            NewUser = new User(); // Reset the User model for the next input
            ModelState.Clear(); // Ensure tha the newuser object and the modelstate is clear
            return Page();
        }

        //  USer import ----
        public async Task<IActionResult> OnPostExportUsersAsync()
        {
            var users = await _dataAccess.GetUsersAsync();
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csvWriter.WriteRecords(users);
            await writer.FlushAsync();
            stream.Position = 0;

            return File(stream, "text/csv", "users.csv");
        }

        public async Task<IActionResult> OnPostImportUsersAsync()
        {
            if (ImportFile == null || ImportFile.Length == 0)
            {
                ModelState.AddModelError("File", "Please choose a valid CSV file.");
                return Page();
            }

            using var reader = new StreamReader(ImportFile.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var newUsers = csv.GetRecords<User>().ToList();
            var existingUsers = await _dataAccess.GetUsersAsync();

            // Find users to delete
            var usersToDelete = existingUsers.Except(newUsers, new UserComparer()).ToList();

            // Find users to add
            var usersToAdd = newUsers.Except(existingUsers, new UserComparer()).ToList();

            // Find users to update
            var usersToUpdate = newUsers.Intersect(existingUsers, new UserComparer()).ToList();

            // Check if users have assigned assets before deleting or updating
            foreach (var user in  usersToDelete.Concat(usersToUpdate))
            {
                var assignedAssets = await _dataAccess.GetAssignedAssetsByUserAsync(user.Id);
                if (assignedAssets.Any())
                {
                    ModelState.AddModelError("File", $"User {user.FirstName} {user.LastName} has assigned assets. Please unassign the assets before importing.");
                    //return Page();
                }
            }

            // Delete users
            foreach (var user in usersToDelete)
            {
                await _dataAccess.DeleteUserAsync(user.Id);
            }

            // Add new users
            foreach (var user in usersToAdd)
            {
                await _dataAccess.AddUserAsync(user);
            }

            // Update existing users
          /*  foreach (var user in usersToUpdate)
            {
                await _dataAccess.UpdateUserAsync(user);
            }*/

            return RedirectToPage("./AddUser");
        }

    }
    public class UserComparer : IEqualityComparer<User>
    {
        public bool Equals(User x, User y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(User obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
