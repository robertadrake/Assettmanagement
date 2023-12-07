using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assettmanagement.Models
{
    public class Asset
    {
        [Key] // Denotes the primary key
        public int Id { get; set; }
        [Required] // Indicates that the Name field is required
        [MaxLength(200)] // You can adjust the maximum length as needed
        public string Name { get; set; }
        [MaxLength(500)] // Optional: Define a maximum length for the Description
        public string Description { get; set; }
        [Required]
        [MaxLength(10)] // Adjust the maximum length as needed
        public string SerialNumber { get; set; }
        [Required]
        public int AssetNumber { get; set; }
        [Required]
        [MaxLength(100)] // Adjust the maximum length as needed
        public string Location { get; set; }
        [Required]
        [MaxLength(100)] // Adjust the maximum length as needed
        public string AssetType { get; set; }
        // Navigation property for the User
        // This represents the relationship between Asset and User
        //public User User { get; set; }
        // Foreign key for User
        // The '?' denotes that the UserId can be null (asset may not be assigned to a user)
        public int? UserId { get; set; }
        public DateTime? CalibrationDate { get; set; }
    }
}
