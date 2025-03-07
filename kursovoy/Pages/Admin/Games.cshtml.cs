using kursovoy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace kursovoy.Pages.Admin
{
    public class GamesModel : PageModel
    {
        private readonly string _gamesFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json", "games.json");



        public List<Game> Games { get; set; }

        public void OnGet()
        {
            Games = JsonConvert.DeserializeObject<List<Game>>(System.IO.File.ReadAllText(_gamesFilePath));
        }

        public IActionResult OnPostDelete(int id)
        {
            var games = JsonConvert.DeserializeObject<List<Game>>(System.IO.File.ReadAllText(_gamesFilePath));
            var game = games.FirstOrDefault(g => g.Id == id);

            if (game != null)
            {
                games.Remove(game);
                System.IO.File.WriteAllText(_gamesFilePath, JsonConvert.SerializeObject(games));
            }

            return RedirectToPage("/Admin/Games");
        }
    }
}
