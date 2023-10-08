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
        [BindProperty]
        public string ResultMessage { get; set; }
        private readonly IDataAccess _dataAccess;
        //private readonly IDataAccess _dataAccess; // Assuming you have an IDataAccess interface for database operations

        public LoginModel(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public void OnGet()
        {
            if (TempData["ResultMessage"] != null)
            {
                ResultMessage = TempData["ResultMessage"].ToString();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("ResultMessage");
            if (!ModelState.IsValid)
            {
                TempData["ResultMessage"] = "Please enter your email and password.";
                return RedirectToPage();
            }

            var user = await _dataAccess.GetUserByEmailAsync(Email);
            try
            {
                if (user != null && SecurityHelper.VerifyPassword(Password, user.PasswordHash))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim("IsAdministrator", user.IsAdministrator ? "true" : "false")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToPage("Index");
                }
                else
                {
                    TempData["ResultMessage"] = "Invalid email or password.";
                }
            }
            catch (Exception ex)
            {
                ResultMessage = ex.Message;
            }
            return RedirectToPage();
        }
    }
}
