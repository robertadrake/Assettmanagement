using System.Collections.Generic;
using System.Threading.Tasks;
using Assettmanagement.Data;
using Assettmanagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Assettmanagement.Pages.Admin
{
    [Authorize(Policy = "AdministratorOnly")]
    public class IndexModel : PageModel
    {
        private readonly IDataAccess _dataAccess;

        public IndexModel(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        [BindProperty]
        public Dictionary<int, string> AssetUserNames { get; set; }
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
            var assetsWithUserNames = await _dataAccess.GetFilteredAssetsWithUserAsync(SelectedAssetType);
            Assets = assetsWithUserNames.Select(x => x.Asset).ToList();
            AssetUserNames = assetsWithUserNames.ToDictionary(x => x.Asset.Id, x => x.UserName);
            Users = await _dataAccess.GetUsersAsync();
            return Page();
        }


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
