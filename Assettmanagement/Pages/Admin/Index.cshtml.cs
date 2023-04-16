using System.Collections.Generic;
using System.Threading.Tasks;
using Assettmanagement.Data;
using Assettmanagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Assettmanagement.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly DataAccess _dataAccess;

        public IndexModel(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public List<Asset> Assets { get; set; }
        public List<User> Users { get; set; }

        [BindProperty]
        public int SelectedAssetId { get; set; }

        [BindProperty]
        public int SelectedUserId { get; set; }

        [TempData]
        public string ResultMessage { get; set; }

        public async Task OnGetAsync()
        {
            Assets = await _dataAccess.GetAssetsAsync();
            Users = await _dataAccess.GetUsersAsync();
        }

        public async Task<IActionResult> OnPostDeleteAssetAsync()
        {
            try
            {
                await _dataAccess.DeleteAssetAsync(SelectedAssetId);
                ResultMessage = "Asset deleted successfully!";
            }
            catch (System.Exception ex)
            {
                ResultMessage = $"Error deleting asset: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteUserAsync()
        {
            try
            {
                await _dataAccess.DeleteUserAsync(SelectedUserId);
                ResultMessage = "User deleted successfully!";
            }
            catch (System.Exception ex)
            {
                ResultMessage = $"Error deleting user: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}
