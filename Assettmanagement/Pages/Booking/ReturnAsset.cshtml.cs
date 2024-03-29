using Assettmanagement.Data;
using Assettmanagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Assettmanagement.Pages.Booking
{
    public class ReturnAssetModel : PageModel
    {
        private readonly IDataAccess _dataAccess;
        [BindProperty(SupportsGet = true)]
        public string SelectedAssetType { get; set; }
        [BindProperty]
        public int SelectedAssetId { get; set; }
        public ReturnAssetModel(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public List<Asset> AssignedAssets { get; set; }

        public string ResultMessage { get; set; }

        public async Task OnGetAsync()
        {
            AssignedAssets = await _dataAccess.GetAssignedAssetsAsync(SelectedAssetType);
           // Assets = await _dataAccess.GetFilteredAssetsAsync(SelectedAssetType);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("SelectedAssetType");

            if (!ModelState.IsValid)
            {
                ResultMessage = "Data Inclompete no action taken.";
                // Refresh the data on the page
                await OnGetAsync();
                return Page();
            }

            try
            {
                await _dataAccess.ReturnAssetAsync(SelectedAssetId);
                // Add a history entry for the returned asset
                // Get or create the System user
                User systemUser = await _dataAccess.GetOrCreateSystemUserAsync();

                var assetHistory = new AssetHistory
                {
                    AssetId = SelectedAssetId,
                    UserId = systemUser.Id, // No user assigned since the asset is returned
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
