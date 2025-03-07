using kursovoy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace kursovoy.Pages.Account
{
    public class AddBalanceModel : PageModel
    {
        private readonly string _usersFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json", "users.json");
        private readonly string _cardsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json", "cards.json");

        [BindProperty]
        [Required(ErrorMessage = "������� ����� ����������.")]
        [Range(1, 100000, ErrorMessage = "����� ������ ���� �� 1 �� 100 000 ���.")]
        public decimal Amount { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "������� ����� �����.")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "����� ����� ������ �������� �� 16 ����.")]
        public string CardNumber { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "������� ���� �������� �����.")]
        [RegularExpression(@"^\d{2}/\d{2}$", ErrorMessage = "���� �������� ������ ���� � ������� MM/YY.")]
        public string ExpiryDate { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "������� CVV ���.")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV ��� ������ �������� �� 3 ����.")]
        public string Cvv { get; set; }

        [BindProperty]
        public bool SaveCard { get; set; }

        public IActionResult OnPost()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToPage("/Account/Login");
            }

            var users = JsonConvert.DeserializeObject<List<User>>(System.IO.File.ReadAllText(_usersFilePath));
            var user = users.FirstOrDefault(u => u.Username == username);

            if (user != null)
            {
                user.Balance += Amount;
                System.IO.File.WriteAllText(_usersFilePath, JsonConvert.SerializeObject(users));
            }

            return RedirectToPage("/Store/Index");
        }
    }
}
