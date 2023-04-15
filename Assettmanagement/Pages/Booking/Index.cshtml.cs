using Assettmanagement.Data;
using Assettmanagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Assettmanagement.Pages.Booking
{
    public class IndexModel : PageModel
    {
        private readonly DataAccess _dataAccess;

        public IndexModel(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [BindProperty]
        public int SelectedAssetId { get; set; }

        [BindProperty]
        public int SelectedUserId { get; set; }

        public List<Asset> Assets { get; private set; }
        public List<User> Users { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Assets = await _dataAccess.GetAssetsAsync();
            Users = await _dataAccess.GetUsersAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostBookAssetAsync()
        {
            await _dataAccess.BookAssetAsync(SelectedAssetId, SelectedUserId);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostReturnAssetAsync()
        {
            await _dataAccess.ReturnAssetAsync(SelectedAssetId);
            return RedirectToPage();
        }
    }
}
