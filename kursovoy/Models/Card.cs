namespace kursovoy.Models
{
    public class Card
    {
        public int UserId { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string Cvv { get; set; }
    }
}
