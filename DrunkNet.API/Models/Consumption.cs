using LiteDB;

namespace DrunkNet.API.Models
{
    public class Consumption
    {
        public int ConsumptionId { get; set; }
        [BsonRef("users")]
        public User User { get; set; } = new User();
        public DateTime TimeConsumed { get; set; }
        public decimal DrinkAmount { get; set; }
        public decimal AlcoholContent { get; set; }
        public decimal AlcoholMassInGrams { get; set; }
    }
}
