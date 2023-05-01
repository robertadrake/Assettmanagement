using System.Collections.Generic;
using System.Threading.Tasks;
using Assettmanagement.Data;
using Assettmanagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Assettmanagement.Models;

namespace Assettmanagement.Pages.Booking
{
    public class ReturnAssetModel : PageModel
    {
        private readonly DataAccess _dataAccess;

        public ReturnAssetModel(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [BindProperty]
        public int SelectedAssetId { get; set; }

        public List<Asset> AssignedAssets { get; set; }

        public string ResultMessage { get; set; }

        public async Task OnGetAsync()
        {
            AssignedAssets = await _dataAccess.GetAssignedAssetsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _dataAccess.ReturnAssetAsync(SelectedAssetId);
                // Add a history entry for the returned asset
                var assetHistory = new AssetHistory
                {
                    AssetId = SelectedAssetId,
                    UserId = null, // No user assigned since the asset is returned
                    Comment = "Asset returned",
                    Timestamp = DateTime.UtcNow
                };

                await _dataAccess.AddAssetHistoryAsync(assetHistory);

                ResultMessage = "Asset successfully returned.";
            }
            catch
            {
                ResultMessage = "Failed to return the asset. Please try again.";
            }

            // Refresh the data on the page
            await OnGetAsync();

            return Page();
        }
    }
}
