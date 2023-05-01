using System.Collections.Generic;
using System.Threading.Tasks;
using Assettmanagement.Data;
using Assettmanagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Assettmanagement.Pages
{
    public class AssetHistoryModel : PageModel
    {
        private readonly DataAccess _dataAccess;

        public AssetHistoryModel(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [BindProperty(SupportsGet = true)]
        public int AssetId { get; set; }

        public Asset Asset { get; set; }

        public List<AssetHistory> AssetHistories { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Asset = await _dataAccess.GetAssetAsync(AssetId);

            if (Asset == null)
            {
                return NotFound();
            }

            AssetHistories = await _dataAccess.GetAssetHistoriesWithUsersAsync(AssetId);

            return Page();
        }
    }
}
