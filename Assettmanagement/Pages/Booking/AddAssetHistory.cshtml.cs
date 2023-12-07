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

        
        public AddAssetHistoryModel(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        
        [BindProperty]
        public AssetHistory AssetHistory { get; set; }
        public List<AssetHistory> AssetHistories { get; set; }
        public List<AssetHistory_dto> AssetHistory_dtos { get; set; }
        
        public async Task<IActionResult> OnGetAsync()
        {
            var assets = await _dataAccess.GetAssetsAsync();
            var users = await _dataAccess.GetUsersAsync();
            var assetSelectListItems = assets.Select(a =>
            new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = $"Name:{a.Name} Serial:{a.SerialNumber} Asset#:{a.AssetNumber}"
            }).ToList();

            TempData["AssetList"] = new SelectList(assetSelectListItems, "Value", "Text");
            TempData["UserList"] = new SelectList(users, "Id", "FirstName");

            if (assets.Any())
            {
                await OnGetLoadAssetHistoriesAsync(assets.First().Id);
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
                try
                {
                    await _dataAccess.AddAssetHistoryAsync(AssetHistory);
                    TempData["ResultMessage"] = "Asset history successfully added.";
                    TempData["IsSuccess"] = true;
                }
                catch (Exception ex)
                {
                    TempData["ResultMessage"] = $"Error adding asset history: {ex.Message}";
                    TempData["IsSuccess"] = false;
                }
                // Refresh the asset and user lists
                await OnGetAsync();
                if (AssetHistory.AssetId != 0)
                {
                    try
                    {
                        var assetHistories = await _dataAccess.GetAssetHistoriesWithUsersAsync(AssetHistory.AssetId);
                        var AssetHistory_dtos = assetHistories.Select(ah => new AssetHistory_dto
                        {
                            Id = ah.Id,
                            UserName = ah.User != null ? $"{ah.User.FirstName} {ah.User.LastName}" : "N/A",
                            Comment = ah.Comment != null ? $"{ah.Comment}" : "N/A",
                            Timestamp = ah.Timestamp,
                            // Map other properties
                        }).ToList();
                        TempData.Remove("ResultMessage");
                        return new JsonResult(AssetHistory_dtos);
                    }
                    catch (Exception ex)
                    {
                        TempData["ResultMessage"] = $"Error reading asset history:{ex.Message}";
                        TempData["IsSuccess"] = false;
                        // Handle exception
                        return BadRequest(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in OnPostAsync {ex}"); // Assuming you have a logger

                // Return a detailed error message for debugging
                return BadRequest("An error occurred: " + ex.Message);
            }
            return BadRequest();
        }
        public async Task<IActionResult> OnGetLoadAssetHistoriesAsync(int assetId)
        {
            try
            {
                var assetHistories = await _dataAccess.GetAssetHistoriesWithUsersAsync(assetId);
                var AssetHistory_dtos = assetHistories.Select(ah => new AssetHistory_dto
                {
                    Id = ah.Id,
                    UserName = ah.User != null ? $"{ah.User.FirstName} {ah.User.LastName}" : "N/A",
                    Comment = ah.Comment != null ? $"{ah.Comment}" : "N/A",
                    Timestamp = ah.Timestamp,
                    // Map other properties
                }).ToList();
                TempData.Remove("ResultMessage");
                return new JsonResult(AssetHistory_dtos);
            }
            catch (Exception ex)
            {
                TempData["ResultMessage"] = $"Error reading asset history:{ex.Message}";
                TempData["IsSuccess"] = false;
                // Handle exception
                return BadRequest(ex.Message);
            }
        }
    }
}
