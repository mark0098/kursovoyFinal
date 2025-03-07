using kursovoy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace kursovoy.Pages.Store;

public class IndexModel : PageModel
{
    private readonly string _gamesFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json", "games.json");

    public List<Game> Games { get; set; }

    public void OnGet()
    {
        Games = JsonConvert.DeserializeObject<List<Game>>(System.IO.File.ReadAllText(_gamesFilePath));
    }
}
