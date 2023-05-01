namespace Assettmanagement.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SerialNumber { get; set; }
        public string AssetNumber { get; set; }
        public string Location { get; set; }
        public string AssetType { get; set; }

        // The user to whom the asset is assigned
        public User User { get; set; }
        public int? UserId { get; set; }

    }
}