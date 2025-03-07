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
        [Required(ErrorMessage = "Введите сумму пополнения.")]
        [Range(1, 100000, ErrorMessage = "Сумма должна быть от 1 до 100 000 руб.")]
        public decimal Amount { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Введите номер карты.")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Номер карты должен состоять из 16 цифр.")]
        public string CardNumber { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Введите срок действия карты.")]
        [RegularExpression(@"^\d{2}/\d{2}$", ErrorMessage = "Срок действия должен быть в формате MM/YY.")]
        public string ExpiryDate { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Введите CVV код.")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV код должен состоять из 3 цифр.")]
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
