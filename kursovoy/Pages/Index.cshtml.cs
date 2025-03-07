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
            // ���������, ���������� �� ���� users.json
            if (!System.IO.File.Exists(_usersFilePath))
            {
                ErrorMessage = "���� � �������������� �� ������.";
                return Page();
            }

            // ������ ������ ������������� �� �����
            var users = JsonConvert.DeserializeObject<List<User>>(System.IO.File.ReadAllText(_usersFilePath));

            // ���� ������������ �� ������
            var user = users.FirstOrDefault(u => u.Username == Username);

            if (user == null)
            {
                ErrorMessage = "������������ � ����� ������� �� ������.";
                return Page();
            }

            // ��������� ������
            if (!BCrypt.Net.BCrypt.Verify(Password, user.Password))
            {
                ErrorMessage = "�������� ������.";
                return Page();
            }

            // ��������� ��� ������������ � ������
            HttpContext.Session.SetString("Username", user.Username);

            // �������������� �� �������� ��������
            return RedirectToPage("/Store/Index");
        }
    }
}
