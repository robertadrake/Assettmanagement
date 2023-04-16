using System.Threading.Tasks;
using Assettmanagement.Data;
using Assettmanagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Assettmanagement.Pages.Admin
{
    public class AddAssetModel : PageModel
    {
        private readonly DataAccess _dataAccess;

        public AddAssetModel(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [BindProperty]
        public Asset Asset { get; set; }

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
            //    return Page();
            }

            try
            {
                await _dataAccess.AddAssetAsync(Asset);
                ResultMessage = "Asset added successfully!";
            }
            catch (System.Exception ex)
            {
                ResultMessage = $"Error adding asset: {ex.Message}";
            }

           return RedirectToPage("./Index");
        }
    }
}