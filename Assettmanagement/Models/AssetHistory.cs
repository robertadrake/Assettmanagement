using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assettmanagement.Models
{
    public class AssetHistory
    {
        [Key] // Primary key
        public int Id { get; set; }
        [Required(ErrorMessage = "AssetID is required.")]
        public int AssetId { get; set; }
        // Navigation property for Asset
        [ForeignKey("AssetId")] // Foreign key association
        public Asset Asset { get; set; }
        [Required(ErrorMessage = "UserID is required.")]
        public int UserId { get; set; }
        // Navigation property for User
        [ForeignKey("UserId")] // Foreign key association
        public User User { get; set; }
        [Required(ErrorMessage = "Comment is required.")]
        [MaxLength(500)] // Optional: Define a maximum length for the comment
        public string Comment { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

