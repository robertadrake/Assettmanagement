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

        public async Task<IActionResult> OnGetAsync()
        {
            Assets = await _dataAccess.GetAssetsWithUsersAsync();
            Users = await _dataAccess.GetUsersAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAssetAsync(int id)
        {
            var asset = await _dataAccess.GetAssetAsync(id);

            if (asset.UserId != null)
            {
                TempData["ErrorMessage"] = $"Cannot delete asset '{asset.Name}' as it is assigned to user '{asset.User.FirstName} {asset.User.LastName}'. Unassign the asset before deleting.";
                return RedirectToPage();
            }                                              

            await _dataAccess.DeleteAssetAsync(id);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteUserAsync(int selectedUserId)
        {
            var user = await _dataAccess.GetUserAsync(selectedUserId); 
            var assignedAssets = await _dataAccess.GetAssignedAssetsByUserAsync(selectedUserId);

            if (assignedAssets.Count > 0)
            {
                ResultMessage = $"User '{user.FirstName} {user.LastName}' has the following assets assigned: {string.Join(", ", assignedAssets.Select(a => a.Name))}. Unassign the assets before deleting the user.";
            }
            else
            {
                await _dataAccess.DeleteUserAsync(selectedUserId);
                ResultMessage = "User deleted successfully.";
            }

            return RedirectToPage();
        }
    }
}
