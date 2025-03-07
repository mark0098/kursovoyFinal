using kursovoy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Проверка данных карты (заглушка)
            if (!IsCardValid(CardNumber, ExpiryDate, Cvv))
            {
                ModelState.AddModelError(string.Empty, "Неверные данные карты.");
                return Page();
            }

            // Загружаем данные пользователя
            var users = JsonConvert.DeserializeObject<List<User>>(System.IO.File.ReadAllText(_usersFilePath));
            var username = HttpContext.Session.GetString("Username");
            var user = users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Пополнение баланса
            user.Balance += Amount;

            // Сохраняем данные карты, если пользователь выбрал чек-бокс
            if (SaveCard)
            {
                var cards = JsonConvert.DeserializeObject<List<Card>>(System.IO.File.ReadAllText(_cardsFilePath)) ?? new List<Card>();
                cards.Add(new Card
                {
                    UserId = user.Id,
                    CardNumber = CardNumber,
                    ExpiryDate = ExpiryDate,
                    Cvv = Cvv
                });
                System.IO.File.WriteAllText(_cardsFilePath, JsonConvert.SerializeObject(cards));
            }

            // Сохраняем обновлённые данные пользователя
            System.IO.File.WriteAllText(_usersFilePath, JsonConvert.SerializeObject(users));

            return RedirectToPage("/Store/Index");
        }

        private bool IsCardValid(string cardNumber, string expiryDate, string cvv)
        {
            // Заглушка для проверки данных карты
            // В реальном приложении здесь будет интеграция с платежным шлюзом
            return true;
        }
    }
}
