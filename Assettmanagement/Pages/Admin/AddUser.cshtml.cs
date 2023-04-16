using System.Threading.Tasks;
using Assettmanagement.Data;
using Assettmanagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Assettmanagement.Pages.Admin
{
    public class AddUserModel : PageModel
    {
        private readonly DataAccess _dataAccess;

        public AddUserModel(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [BindProperty]
        public User User { get; set; }

        [TempData]
        public string ResultMessage { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _dataAccess.AddUserAsync(User);
                ResultMessage = "User added successfully!";
            }
            catch (System.Exception ex)
            {
                ResultMessage = $"Error adding user: {ex.Message}";
            }

            return RedirectToPage("./Index");
        }
    }
}
