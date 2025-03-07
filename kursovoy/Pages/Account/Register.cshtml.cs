using kursovoy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace kursovoy.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly string _usersFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json", "users.json");

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string Email { get; set; }

        public IActionResult OnPost()
        {
            var users = JsonConvert.DeserializeObject<List<User>>(System.IO.File.ReadAllText(_usersFilePath));
            var user = new User
            {
                Id = users.Count + 1,
                Username = Username,
                Password = BCrypt.Net.BCrypt.HashPassword(Password),
                Email = Email,
                Balance = 0
            };
            users.Add(user);
            System.IO.File.WriteAllText(_usersFilePath, JsonConvert.SerializeObject(users));

            return RedirectToPage("/Account/Login");
        }
    }
}
