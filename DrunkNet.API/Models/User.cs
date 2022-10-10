namespace DrunkNet.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string ExternalAuthId { get; set; } = "";

        public string Name { get; set; } = "";
        public decimal Weight { get; set; }
        public Gender Gender { get; set; }
        
        public decimal GetAlcoholMetabolizeRate()
        {
            return Gender switch
            {
                Gender.Male => 0.015M,
                Gender.Female => 0.017M,
                _ => 0.015M
            };
        }
    
        public decimal GetBodyWaterRatio()
        {
            return Gender switch
            {
                Gender.Male => 0.68M,
                Gender.Female => 0.55M,
                _ => 0.68M
            };
        }
    }
}
