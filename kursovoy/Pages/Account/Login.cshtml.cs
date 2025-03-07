using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using BCrypt.Net;
using kursovoy.Models;

namespace kursovoy.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly string _usersFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json", "users.json");

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public IActionResult OnPost()
        {
            var users = JsonConvert.DeserializeObject<List<User>>(System.IO.File.ReadAllText(_usersFilePath));
            var user = users.FirstOrDefault(u => u.Username == Username && BCrypt.Net.BCrypt.Verify(Password, u.Password));

            if (user != null)
            {
                HttpContext.Session.SetString("Username", user.Username);
                return RedirectToPage("/Store/Index");
            }

            ViewData["Error"] = "Неверный логин или пароль";
            return Page();
        }
    }
}
