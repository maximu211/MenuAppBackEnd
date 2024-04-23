using MongoDB.Bson;

namespace MenuApp.DAL.Models.EntityModels
{
    public class Receipts
    {
        public ObjectId Id { get; set; }
        public ObjectId UserId { get; set; }
        public required string Name { get; set; }
        public CookingTime CookTime { get; set; }
        public CookingDifficulty CookingDifficulty { get; set; }
        public required string ReceiptType { get; set; }
        public List<ObjectId> Likes { get; set; } = new List<ObjectId>();
        public List<ObjectId> Saved { get; set; } = new List<ObjectId>();
        public List<ReceiptDescriptionElement> ReceiptDescriptionElements { get; set; } =
            new List<ReceiptDescriptionElement>();
        public List<string> ReceiptIngradients { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum CookingTime
    {
        lessThan15min,
        min15,
        min30,
        min45,
        hour1,
        moreThanHour1,
    }

    public enum CookingDifficulty
    {
        easy,
        medium,
        hard,
        veryHard,
    }

    public class ReceiptDescriptionElement
    {
        public required string ReceiptDescriptionElementText { get; set; }
        public string? ReceiptDescriptionElementPhoto { get; set; }
    }
}
