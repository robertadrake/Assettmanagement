using System;
using System.ComponentModel.DataAnnotations;

namespace Assettmanagement.Models
{
    public class AssetHistory
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "AssetID is required.")]
        public int AssetId { get; set; }
        public Asset Asset { get; set; }

        [Required(ErrorMessage = "UserID is required.")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required(ErrorMessage = "Comment is required.")]
        public string Comment { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
