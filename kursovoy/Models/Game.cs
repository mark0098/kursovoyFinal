namespace kursovoy.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CoverImage { get; set; }
        public List<string> Screenshots { get; set; }
        public string SystemRequirements { get; set; }
    }
}
