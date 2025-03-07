using kursovoy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace kursovoy.Pages.Admin
{
    public class AddGameModel : PageModel
    {
        private readonly string _gamesFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json", "games.json");
        private readonly string _coversPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "cover");
        private readonly string _screenshotsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "screenshot");

        [BindProperty]
        [Required(ErrorMessage = "������� �������� ����.")]
        public string Title { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "������� �������� ����.")]
        public string Description { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "������� ���� ����.")]
        [Range(0, 100000, ErrorMessage = "���� ������ ���� �� 0 �� 100 000 ���.")]
        public decimal Price { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "��������� ������� ����.")]
        public IFormFile CoverImage { get; set; }

        [BindProperty]
        public List<IFormFile> Screenshots { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "������� ��������� ����������.")]
        public string SystemRequirements { get; set; }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // ��������� ������ ���
            var games = JsonConvert.DeserializeObject<List<Game>>(System.IO.File.ReadAllText(_gamesFilePath)) ?? new List<Game>();

            // ���������� ���������� ID ��� ����� ����
            var newGameId = games.Any() ? games.Max(g => g.Id) + 1 : 1;

            // ��������� ������� ����
            var coverImagePath = SaveImage(CoverImage, "covers");

            // ��������� ��������� ����
            var screenshotsPaths = new List<string>();
            if (Screenshots != null && Screenshots.Any())
            {
                foreach (var screenshot in Screenshots)
                {
                    var screenshotPath = SaveImage(screenshot, "screenshots");
                    screenshotsPaths.Add(screenshotPath);
                }
            }

            // ������� ����� ����
            var newGame = new Game
            {
                Id = newGameId,
                Title = Title,
                Description = Description,
                Price = Price,
                CoverImage = coverImagePath,
                Screenshots = screenshotsPaths,
                SystemRequirements = SystemRequirements
            };

            // ��������� ���� � ������
            games.Add(newGame);

            // ��������� ����������� ������ ���
            System.IO.File.WriteAllText(_gamesFilePath, JsonConvert.SerializeObject(games));

            return RedirectToPage("/Admin/Games");
        }

        private string SaveImage(IFormFile image, string folder)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = folder == "covers"
                ? Path.Combine(_coversPath, fileName)
                : Path.Combine(_screenshotsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(stream);
            }

            return folder == "covers"
                ? $"/cover/{fileName}"
                : $"/screenshot/{fileName}";
        }
    }
}
