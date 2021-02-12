using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BugTracker.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string ErrorZaUsername { get; set; }
        [BindProperty]
        public string ErrorZaPassword { get; set; }
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }


        public async Task<IActionResult> OnPostAsync()
        {
            if (String.IsNullOrEmpty(Username))
            {
                ErrorZaUsername = "Molimo Vas unesite korisnicko ime";
                return Page();
            }
            else if (String.IsNullOrEmpty(Password))
            {
                ErrorZaPassword = "Molimo Vas unesite sifru";
                return Page();
            }

            var user = await DataProvider.ReadUserByUsername(Username);
            if (user == null)
            {
                ErrorZaUsername = "Ne postoji korisnik sa ovim imenom";
                return Page();
            }
            else if (!user.Password.Equals(Password))
            {
                ErrorZaUsername = "Pogresna sifra za ovog korisnika";
                return Page();
            }
            return Redirect($"/MyProjects?user_id={user.Id}");
        }
    }
}