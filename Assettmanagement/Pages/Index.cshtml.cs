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

        public async Task<IActionResult> OnGetAsync()
        {
            // needs to show either my assets or if admin then all the assets
          //  if (User.HasClaim(ClaimTypes.Role, "Administrator"))
//            {
                //Assets = await _dataAccess.GetFilteredAssetsAsync(SelectedAssetType);
            //}
            //else
            //{
                var emailClaim = User.FindFirst(ClaimTypes.Email);
                if (emailClaim == null)
                {
                    // Handle error - claim not found
                    return Page();
                }
                var email = emailClaim.Value;
                Assets = await _dataAccess.GetAssetsByUserEmailAsync(email, SelectedAssetType);
            //}
            return Page();
        }

    }
}