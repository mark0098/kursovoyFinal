namespace kursovoy.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Зашифрованный пароль
        public string Email { get; set; }
        public decimal Balance { get; set; }
        public string Role { get; set; } = "User";
    }
}
