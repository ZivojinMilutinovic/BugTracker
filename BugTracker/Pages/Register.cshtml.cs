using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BugTracker.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public List<string> Pozicije { get; set; }
        [BindProperty]
        public string IzabranaPozicija { get; set; }
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public string Surname { get; set; }
        [BindProperty]
        public string Greska { get; set; }

        public void OnGet()
        {
            Pozicije = new List<string>
            {
                "Front-end",
                "Back-end",
                "Android",
                "IOs",
                "QA"
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await DataProvider.ReadUserByUsername(Username);
            if (user != null)
            {
                Greska = "Ovo korisnicko ime je zauzeto";
                Pozicije = new List<string>
                {
                    "Front-end",
                "Back-end",
                "Android",
                "IOs",
                "QA"
                };
                return Page();
            }
            if (String.IsNullOrEmpty(Username))
            {
                Greska = "Korisnicko ime je obavezno polje";
                Pozicije = new List<string>
                {
                    "Front-end",
                "Back-end",
                "Android",
                "IOs",
                "QA"
                };
                return Page();
            }
            if (String.IsNullOrEmpty(Password))
            {
                Greska = "Sifra je obavezno polje";
                Pozicije = new List<string>
                {
                    "Front-end",
                "Back-end",
                "Android",
                "IOs",
                "QA"
                };
                return Page();
            }
            user = new User
            {
                Name = Name,
                Password = Password,
                SurName = Surname,
                Position = IzabranaPozicija,
                UserName = Username
            };
            await DataProvider.CreateUser(user);
            return Redirect($"/MyProjects?user_id={user.Id}");
        }


    }
}