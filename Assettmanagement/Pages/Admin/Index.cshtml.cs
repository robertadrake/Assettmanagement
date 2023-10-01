using System.Collections.Generic;
using System.Threading.Tasks;
using Assettmanagement.Data;
using Assettmanagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Assettmanagement.Pages.Admin
{
    [Authorize(Policy = "SpecificUserOnly")]
    public class IndexModel : PageModel
    {
        private readonly DataAccess _dataAccess;

        public IndexModel(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        [BindProperty(SupportsGet = true)]
        public string SelectedAssetType { get; set; }


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
            //Assets = await _dataAccess.GetAssetsWithUsersAsync();
            Assets = await _dataAccess.GetFilteredAssetsAsync(SelectedAssetType);
            Users = await _dataAccess.GetUsersAsync();
            return Page();
        }

        //public async Task<IActionResult> OnPostDeleteAssetAsync(int AssetId)
        public async Task<IActionResult> OnPostDeleteAssetAsync(int? SelectedAssetId)
        {
            if (SelectedAssetId != 0) { 
                var asset = await _dataAccess.GetAssetAsync((int)SelectedAssetId);

                if (asset.UserId != null)
                {
                    ResultMessage = $"Cannot delete asset '{asset.Name}' as it is assigned to a User. Unassign the asset before deleting.";
                    return RedirectToPage();
                }

                await _dataAccess.DeleteAssetAsync((int)SelectedAssetId);
                ResultMessage = "Asset deleted successfully.";
            }
            else
            {
                ResultMessage = $"Asset ID is null : {SelectedAssetId}";
            }
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
