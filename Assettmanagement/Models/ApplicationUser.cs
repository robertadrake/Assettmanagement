using Microsoft.AspNetCore.Identity;

namespace Assettmanagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? UserInfoId { get; set; }
        public User UserInfo { get; set; }
    }

}
