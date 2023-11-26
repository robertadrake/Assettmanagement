using System.ComponentModel.DataAnnotations;

namespace Assettmanagement.Models
{
    public class User
    {
        [Key] // Primary key
        public int Id { get; set; }
        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, ErrorMessage = "First Name cannot be longer than 50 characters.")]
        public required string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters.")]
        public required string LastName { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(100)] // Optional: Define a maximum length for the email
        public required string Email { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        // Consider using a separate field for password length validation if you're storing a hash
        public required string PasswordHash { get; set; }
        public bool IsAdministrator { get; set; }
        // Optional: Navigation properties for related entities
        public ICollection<Asset> Assets { get; set; }
        public ICollection<AssetHistory> AssetHistories { get; set; }
    }
}
