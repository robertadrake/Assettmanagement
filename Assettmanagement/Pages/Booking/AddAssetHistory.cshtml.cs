using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assettmanagement.Data;
using Assettmanagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Assettmanagement.Pages.Booking
{
    public class AddAssetHistoryModel : PageModel
    {
        private readonly IDataAccess _dataAccess;
        public List<AssetHistory> AssetHistories { get; set; }

        public AddAssetHistoryModel(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [BindProperty]
        public AssetHistory AssetHistory { get; set; }
        private async Task LoadAssetHistoriesAsync(int assetId)
        {
            AssetHistories = await _dataAccess.GetAssetHistoriesWithUsersAsync(assetId);
        }

        public async Task<IActionResult> OnGetLoadAssetHistoriesAsync(int assetId)
        {
            AssetHistories = await _dataAccess.GetAssetHistoriesWithUsersAsync(assetId);
            return new JsonResult(AssetHistories);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var assets = await _dataAccess.GetAssetsAsync();
            var users = await _dataAccess.GetUsersAsync();

            ViewData["AssetList"] = new SelectList(assets, "Id", "Name");
            ViewData["UserList"] = new SelectList(users, "Id", "FirstName");

            if (assets.Any())
            {
                await LoadAssetHistoriesAsync(assets.First().Id);
            }

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("AssetHistory.Asset");
            ModelState.Remove("AssetHistory.User");
            if (!ModelState.IsValid)
            {
                return Page();
            }

            AssetHistory.Timestamp = DateTime.UtcNow;

            try
            {
                await _dataAccess.AddAssetHistoryAsync(AssetHistory);
                ViewData["ResultMessage"] = "Asset history successfully added.";
                ViewData["IsSuccess"] = true;
            }
            catch (Exception ex)
            {
                ViewData["ResultMessage"] = $"Error adding asset history: {ex.Message}";
                ViewData["IsSuccess"] = false;
            }
            if (AssetHistory.AssetId != 0)
            {
                await LoadAssetHistoriesAsync(AssetHistory.AssetId);
            }
            // Reset the form values
            AssetHistory = new AssetHistory();

            // Refresh the asset and user lists
            await OnGetAsync();

            return Page();
        }
    }
}
