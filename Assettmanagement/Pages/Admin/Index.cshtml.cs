using System.Collections.Generic;
using System.Threading.Tasks;
using Assettmanagement.Data;
using Assettmanagement.Database;
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

        [BindProperty]
        public Asset NewAsset { get; set; }

        [BindProperty]
        public User NewUser { get; set; }

        public List<Asset> Assets { get; private set; }
        public List<User> Users { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Assets = await _dataAccess.GetAssetsAsync();
            Users = await _dataAccess.GetUsersAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAddAssetAsync()
        {
            if (ModelState.IsValid)
            {
                await _dataAccess.AddAssetAsync(NewAsset);
                return RedirectToPage();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAssetAsync(int id)
        {
            await _dataAccess.DeleteAssetAsync(id);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddUserAsync()
        {
            if (ModelState.IsValid)
            {
                await _dataAccess.AddUserAsync(NewUser);
                return RedirectToPage();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteUserAsync(int id)
        {
            await _dataAccess.DeleteUserAsync(id);
            return RedirectToPage();
        }
    }
}
