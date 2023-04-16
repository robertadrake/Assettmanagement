using Assettmanagement.Data;
using Assettmanagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Assettmanagement.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly DataAccess _dataAccess;

        public IndexModel(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public List<Asset> Assets { get; set; }
        public List<User> Users { get; set; }

        public async Task OnGetAsync()
        {
            Assets = await _dataAccess.GetAssetsAsync();
            Users = await _dataAccess.GetUsersAsync();
        }

        [BindProperty]
        public Asset NewAsset { get; set; }

        [BindProperty]
        public User NewUser { get; set; }

        public async Task<IActionResult> OnPostAddAssetAsync()
        {
       //     if (ModelState.IsValid)
            {
                try
                {
                    await _dataAccess.AddAssetAsync(NewAsset);
                    TempData["Message"] = "Asset added successfully!";
                }
                catch (Exception ex)
                {
                    TempData["Message"] = $"Error adding asset: {ex.Message}";
                }
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddUserAsync()
        {
            //if (ModelState.IsValid)
            {
                try
                {
                    await _dataAccess.AddUserAsync(NewUser);
                    TempData["Message"] = "User added successfully!";
                }
                catch (Exception ex)
                {
                    TempData["Message"] = $"Error adding user: {ex.Message}";
                }
            }
          //  else
          //  {
          //      TempData["Message"] = $"Error adding user";
          //  }

            return RedirectToPage();
        }
    }
}
