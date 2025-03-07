using kursovoy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace kursovoy.Pages
{
    public class IndexModel : PageModel
    {
        private readonly string _usersFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json", "users.json");

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnPost()
        {
            // Проверяем, существует ли файл users.json
            if (!System.IO.File.Exists(_usersFilePath))
            {
                ErrorMessage = "Файл с пользователями не найден.";
                return Page();
            }

            // Читаем данные пользователей из файла
            var users = JsonConvert.DeserializeObject<List<User>>(System.IO.File.ReadAllText(_usersFilePath));

            // Ищем пользователя по логину
            var user = users.FirstOrDefault(u => u.Username == Username);

            if (user == null)
            {
                ErrorMessage = "Пользователь с таким логином не найден.";
                return Page();
            }

            // Проверяем пароль
            if (!BCrypt.Net.BCrypt.Verify(Password, user.Password))
            {
                ErrorMessage = "Неверный пароль.";
                return Page();
            }

            // Сохраняем имя пользователя в сессии
            HttpContext.Session.SetString("Username", user.Username);

            // Перенаправляем на страницу магазина
            return RedirectToPage("/Store/Index");
        }
    }
}
