using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Assettmanagement.Data;
using Assettmanagement.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Assettmanagement.Pages
{
    public class ChangePasswordModel : PageModel
    {
        private readonly IDataAccess _dataAccess;

        public ChangePasswordModel(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [BindProperty, DataType(DataType.Password), Required]
        public string CurrentPassword { get; set; }

        [BindProperty, DataType(DataType.Password), Required]
        public string NewPassword { get; set; }

        [BindProperty, DataType(DataType.Password), Required, Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }

        public string ResultMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var emailClaim = User.FindFirst(ClaimTypes.Email);
            if (emailClaim == null)
            {
                // Handle error - claim not found
                return Page();
            }

            var email = emailClaim.Value;
            var currentUser = await _dataAccess.GetUserByEmailAsync(email);

            if (currentUser == null || !SecurityHelper.VerifyPassword(CurrentPassword, currentUser.PasswordHash))
            {
                ModelState.AddModelError("CurrentPassword", "The current password is incorrect.");
                return Page();
            }

            currentUser.PasswordHash = SecurityHelper.HashPassword(NewPassword);
            await _dataAccess.UpdateUserAsync(currentUser);

            ResultMessage = "Password changed successfully!";
            return Page();
            // return RedirectToPage("/Index"); // Redirect to home page or any other page after successful password change
        }
    }
}
