using Assettmanagement.Data;
using Assettmanagement.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Assettmanagement.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [BindProperty]
        [Required]
        public string Password { get; set; }
        public string ResultMessage { get; set; }
        private readonly DataAccess _dataAccess;
        //private readonly IDataAccess _dataAccess; // Assuming you have an IDataAccess interface for database operations

        public LoginModel(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _dataAccess.GetUserByEmailAsync(Email);
            try
            {
                if (user != null && SecurityHelper.VerifyPassword(Password, user.PasswordHash))
                {
                    var claims = new List<Claim>
                    {
                        //new Claim(ClaimTypes.Name, user.LastName),
                        // Add other claims as needed
                        new Claim("IsAdministrator", user.IsAdministrator ? "true" : "false")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToPage("Index");
                }
                else
                {
                    ResultMessage = "Invalid email or password.";
                }
            }
            catch (Exception ex)
            {
                ResultMessage = ex.Message;
            }
            return Page();
        }
    }
}
