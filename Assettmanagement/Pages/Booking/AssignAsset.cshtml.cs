using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Assettmanagement.Models;
using Assettmanagement.Data;

namespace Assettmanagement.Pages.Booking
{
    public class AssignAssetModel : PageModel
    {
        private readonly DataAccess _dataAccess;

        public AssignAssetModel(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [BindProperty]
        public int SelectedAssetId { get; set; }

        [BindProperty]
        public int SelectedUserId { get; set; }

        public List<Asset> AvailableAssets { get; set; }

        public List<User> Users { get; set; }

        public string ResultMessage { get; set; }

        public async Task OnGetAsync()
        {
            AvailableAssets = await _dataAccess.GetAvailableAssetsAsync();
            Users = await _dataAccess.GetUsersAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (SelectedUserId == 0)
            {
                ResultMessage = "Please select a user to assign the asset.";
                await OnGetAsync();
                return Page();
            }

            try
            {
                await _dataAccess.AssignAssetToUserAsync(SelectedAssetId, SelectedUserId);
                ResultMessage = "Asset successfully assigned.";
            }
            catch
            {
                ResultMessage = "Failed to assign the asset. Please try again.";
            }

            // Refresh the data on the page
            await OnGetAsync();

            return Page();
        }
    }
}
