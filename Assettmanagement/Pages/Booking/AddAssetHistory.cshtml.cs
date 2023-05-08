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
        private readonly DataAccess _dataAccess;
        public List<AssetHistory> AssetHistories { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SelectedAssetType { get; set; }
        public AddAssetHistoryModel(DataAccess dataAccess)
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

            if (!string.IsNullOrEmpty(SelectedAssetType))
            {
                assets = assets.Where(a => a.AssetType == SelectedAssetType).ToList();
            }

            ViewData["AssetList"] = assets.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.SerialNumber
            }).ToList();

            ViewData["AssetList"] = new SelectList(assets, "Id", "SerialNumber"); // Changed from "Name" to "SerialNumber"
            ViewData["UserList"] = new SelectList(users.Select(u => new { u.Id, FullName = $"{u.FirstName} {u.LastName}" }), "Id", "FullName");

            {
                await LoadAssetHistoriesAsync(assets.First().Id);
            }

            return Page();
        }

        public async Task<IActionResult> OnGetGetAssetDetailsAsync(int assetId)
        {
            var asset = await _dataAccess.GetAssetAsync(assetId);
            return new JsonResult(asset);
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
