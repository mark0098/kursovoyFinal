using kursovoy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace kursovoy.Pages.Library
{
    public class IndexModel : PageModel
    {
        private readonly string _purchasesFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json", "purchases.json");
        private readonly string _gamesFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json", "games.json");
        private readonly string _gameKeysFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json", "gamekeys.json");

        public List<Game> PurchasedGames { get; set; }
        public Dictionary<int, string> GameKeys { get; set; } = new Dictionary<int, string>();

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToPage("/Account/Login");
            }

            var purchases = JsonConvert.DeserializeObject<List<Purchase>>(System.IO.File.ReadAllText(_purchasesFilePath));

            var games = JsonConvert.DeserializeObject<List<Game>>(System.IO.File.ReadAllText(_gamesFilePath));

            var users = JsonConvert.DeserializeObject<List<User>>(System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json", "users.json")));
            var user = users.FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var purchasedGameIds = purchases
                .Where(p => p.UserId == user.Id)
                .Select(p => p.GameId)
                .ToList();

            PurchasedGames = games
                .Where(g => purchasedGameIds.Contains(g.Id))
                .ToList();

            var gameKeys = JsonConvert.DeserializeObject<List<GameKey>>(System.IO.File.ReadAllText(_gameKeysFilePath));
            foreach (var key in gameKeys.Where(k => k.UserId == user.Id))
            {
                GameKeys[key.GameId] = key.Key;
            }

            return Page();
        }

        public string GetGameKey(int gameId)
        {
            return GameKeys.ContainsKey(gameId) ? GameKeys[gameId] : "Ключ не найден";
        }
    }
}
