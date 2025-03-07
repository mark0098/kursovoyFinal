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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // �������� ������ ����� (��������)
            if (!IsCardValid(CardNumber, ExpiryDate, Cvv))
            {
                ModelState.AddModelError(string.Empty, "�������� ������ �����.");
                return Page();
            }

            // ��������� ������ ������������
            var users = JsonConvert.DeserializeObject<List<User>>(System.IO.File.ReadAllText(_usersFilePath));
            var username = HttpContext.Session.GetString("Username");
            var user = users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // ���������� �������
            user.Balance += Amount;

            // ��������� ������ �����, ���� ������������ ������ ���-����
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

            // ��������� ���������� ������ ������������
            System.IO.File.WriteAllText(_usersFilePath, JsonConvert.SerializeObject(users));

            return RedirectToPage("/Store/Index");
        }

        private bool IsCardValid(string cardNumber, string expiryDate, string cvv)
        {
            // �������� ��� �������� ������ �����
            // � �������� ���������� ����� ����� ���������� � ��������� ������
            return true;
        }
    }
}
