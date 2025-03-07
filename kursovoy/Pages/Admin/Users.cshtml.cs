using kursovoy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace kursovoy.Pages.Admin
{
    public class UsersModel : PageModel
    {
        private readonly string _usersFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json", "users.json");

        public List<User> Users { get; set; }

        public void OnGet()
        {
            Users = JsonConvert.DeserializeObject<List<User>>(System.IO.File.ReadAllText(_usersFilePath));
        }

        public IActionResult OnPostDelete(int id)
        {
            var users = JsonConvert.DeserializeObject<List<User>>(System.IO.File.ReadAllText(_usersFilePath));
            var user = users.FirstOrDefault(u => u.Id == id);

            if (user != null)
            {
                users.Remove(user);
                System.IO.File.WriteAllText(_usersFilePath, JsonConvert.SerializeObject(users));
            }

            return RedirectToPage("/Admin/Users");
        }
    }
}
