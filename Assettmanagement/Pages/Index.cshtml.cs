using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Assettmanagement.Data;
using Assettmanagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Assettmanagement.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IDataAccess _dataAccess;
        [BindProperty(SupportsGet = true)]
        public string SelectedAssetType { get; set; }
        [BindProperty]
        public int SelectedAssetId { get; set; }
        public IndexModel(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        public List<Asset> Assets { get; set; }
        [BindProperty]
        public Dictionary<int, string> AssetUserNames { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var emailClaim = User.FindFirst(ClaimTypes.Email);
            if (emailClaim == null)
            {
                // Handle error - claim not found
                return Page();
            }
            var email = emailClaim.Value;

            var assetsWithUserNames = await _dataAccess.GetAssetsByUserEmailAsync(email, SelectedAssetType);
            Assets = assetsWithUserNames.Select(x => x.Asset).ToList();
            AssetUserNames = assetsWithUserNames.ToDictionary(x => x.Asset.Id, x => x.UserName);

            return Page();
        }


    }
}