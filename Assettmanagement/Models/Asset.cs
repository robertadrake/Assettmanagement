namespace Assettmanagement.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public string SerialNumber { get; set; }
        public string AssetNumber { get; set; }
        public string Location { get; set; }
        public int? BookedOutTo { get; set; }
    }
}
