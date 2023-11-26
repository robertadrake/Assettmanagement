using System.Globalization;
using System.Threading.Tasks;
using Assettmanagement.Data;
using Assettmanagement.Models;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Assettmanagement.Pages.Admin
{
    [Authorize(Policy = "AdministratorOnly")]
    public class AddAssetModel : PageModel
    {
        private readonly IDataAccess _dataAccess;

        public AddAssetModel(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        [BindProperty]
        public int Quantity { get; set; } = 1;
        [BindProperty]
        public IFormFile File { get; set; }
        [BindProperty]
        public int HighestAssetNumber { get; set; }

        [BindProperty]
        public Asset Asset { get; set; }

        [BindProperty]
        public string ResultMessage { get; set; }

        public async Task OnGetAsync()
        {
            if (TempData["ResultMessage"] != null)
            {
                ResultMessage = TempData["ResultMessage"].ToString();
            }
            var highestAssetNumberResult = await _dataAccess.GetHighestAssetNumberAsync();
            int highestAssetNumber = highestAssetNumberResult.HasValue ? highestAssetNumberResult.Value : 0; // Default to 0 if null
        }

        public async Task<IActionResult> OnPostSaveAssetAsync()
        {
            ModelState.Remove("File");
            ModelState.Remove("Asset.User");
            ModelState.Remove("ResultMessage");
            if (!ModelState.IsValid)
            {
                TempData["ResultMessage"] = ("Missing Data");
                return RedirectToPage();
            }

            var highestAssetNumberResult = await _dataAccess.GetHighestAssetNumberAsync();
            int highestAssetNumber = highestAssetNumberResult.HasValue ? highestAssetNumberResult.Value : 0; // Default to 0 if null


            if ((Asset.AssetNumber) <= HighestAssetNumber)
            {
                TempData["ResultMessage"]=("The asset number must be higher than the current highest asset number.");
                return RedirectToPage();
            }

            for (int i = 0; i < Quantity; i++)
            {
                Asset assetToAdd = new Asset
                {
                    Name = Asset.Name,
                    Description = Asset.Description,
                    AssetType = Asset.AssetType,
                    SerialNumber =  Asset.SerialNumber,
                    AssetNumber = ((Asset.AssetNumber) + i),   // Increment asset number
                    Location = Asset.Location
                };

                await _dataAccess.AddAssetAsync(assetToAdd);
            }

            TempData["ResultMessage"] = $"{Quantity} assets added successfully.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostExportAssetAsync()
        {
            var assets = await _dataAccess.GetAssetsAsync();
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csvWriter.WriteRecords(assets);
            await writer.FlushAsync();
            stream.Position = 0;
            TempData["ResultMessage"] = "Asset saved successfully!";
            return File(stream, "text/csv", "Assets.csv");
        }


        public async Task<IActionResult> OnPostImportAssetAsync()
        {
            if (File == null || File.Length == 0)
            {
                TempData["ResultMessage"]=("File", "Please choose a valid CSV file.");
                return Page();
            }

            using var reader = new StreamReader(File.OpenReadStream());
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var newAsset = csv.GetRecords<Asset>().ToList();
            var existingAsset = await _dataAccess.GetAssetsAsync();

            // Find Asset to delete
            var AssetToDelete = existingAsset.Except(newAsset, new AssetComparer()).ToList();

            // Find Asset to add
            var AssetToAdd = newAsset.Except(existingAsset, new AssetComparer()).ToList();

            // Find Asset to update
            var AssetToUpdate = newAsset.Intersect(existingAsset, new AssetComparer()).ToList();

            // Check if Asset have assigned assets before deleting or updating
            foreach (var Asset in AssetToDelete.Concat(AssetToUpdate))
            {
                var assignedAssets = await _dataAccess.GetAssignedAssetsByUserAsync(Asset.Id);
                if (assignedAssets.Any())
                {
                    TempData["ResultMessage"]=("File", $"Asset is assigned to a user, Please unassign the assets before importing.");
                    //return Page();
                }
            }

            // Delete Asset
            foreach (var Asset in AssetToDelete)
            {
                await _dataAccess.DeleteAssetAsync(Asset.Id);
            }

            // Add new Asset
            foreach (var Asset in AssetToAdd)
            {
                await _dataAccess.AddAssetAsync(Asset);
            }

            // Update existing Assets
            /*  foreach (var user in AssetToUpdate)
              {
                  await _dataAccess.UpdateAssetAsync(user);
              }*/
            TempData["ResultMessage"] = "Assets Imported successfully!";
            return RedirectToPage(); 
        }

    }

    public class AssetComparer : IEqualityComparer<Asset>
    {
        public bool Equals(Asset x, Asset y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Asset obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}