using kursovoy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace kursovoy.Pages.Store
{
    public class GameModel : PageModel
    {
        private readonly string _gamesFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json", "games.json");
        private readonly string _usersFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json", "users.json");
        private readonly string _purchasesFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json", "purchases.json");
        private readonly string _gameKeysFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json", "gamekeys.json");

        public Game Game { get; set; }
        public bool IsPurchased { get; set; }

        public static class KeyGenerator
        {
            private static readonly Random _random = new Random();
            private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            public static string GenerateKey()
            {
                var keyPart1 = new string(Enumerable.Repeat(Chars, 5).Select(s => s[_random.Next(s.Length)]).ToArray());
                var keyPart2 = new string(Enumerable.Repeat(Chars, 5).Select(s => s[_random.Next(s.Length)]).ToArray());
                var keyPart3 = new string(Enumerable.Repeat(Chars, 5).Select(s => s[_random.Next(s.Length)]).ToArray());

                return $"Steam - {keyPart1}-{keyPart2}-{keyPart3}";
            }
        }

        public IActionResult OnGet(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToPage("/Account/Login");
            }
            \
            var games = JsonConvert.DeserializeObject<List<Game>>(System.IO.File.ReadAllText(_gamesFilePath));
            Game = games.FirstOrDefault(g => g.Id == id);

            if (Game == null)
            {
                return NotFound();
            }
            
            var users = JsonConvert.DeserializeObject<List<User>>(System.IO.File.ReadAllText(_usersFilePath));
            var user = users.FirstOrDefault(u => u.Username == username);

            var purchases = JsonConvert.DeserializeObject<List<Purchase>>(System.IO.File.ReadAllText(_purchasesFilePath));
            IsPurchased = purchases.Any(p => p.UserId == user.Id && p.GameId == id);

            return Page();
        }

        public IActionResult OnPost(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToPage("/Account/Login");
            }

            var users = JsonConvert.DeserializeObject<List<User>>(System.IO.File.ReadAllText(_usersFilePath));
            var user = users.FirstOrDefault(u => u.Username == username);

            var games = JsonConvert.DeserializeObject<List<Game>>(System.IO.File.ReadAllText(_gamesFilePath));
            var game = games.FirstOrDefault(g => g.Id == id);

            if (game == null)
            {
                return NotFound();
            }

            if (user.Balance >= game.Price)
            {
                user.Balance -= game.Price;

                var purchases = JsonConvert.DeserializeObject<List<Purchase>>(System.IO.File.ReadAllText(_purchasesFilePath)) ?? new List<Purchase>();
                purchases.Add(new Purchase { UserId = user.Id, GameId = game.Id });

                var key = KeyGenerator.GenerateKey();

                var gameKeys = JsonConvert.DeserializeObject<List<GameKey>>(System.IO.File.ReadAllText(_gameKeysFilePath)) ?? new List<GameKey>();
                gameKeys.Add(new GameKey { UserId = user.Id, GameId = game.Id, Key = key });

                System.IO.File.WriteAllText(_usersFilePath, JsonConvert.SerializeObject(users));
                System.IO.File.WriteAllText(_purchasesFilePath, JsonConvert.SerializeObject(purchases));
                System.IO.File.WriteAllText(_gameKeysFilePath, JsonConvert.SerializeObject(gameKeys));

                return RedirectToPage("/Library/Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Недостаточно средств на балансе.";
                return RedirectToPage("/Store/Game", new { id });
            }
        }
    }
}
