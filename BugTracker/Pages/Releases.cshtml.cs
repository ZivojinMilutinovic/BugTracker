using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;

namespace BugTracker.Pages
{
    public class ReleasesModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public Project Project { get; set; }
        [BindProperty(SupportsGet = true)]
        public User CurrentUser { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<Release> Releases { get; set; }
        [BindProperty(SupportsGet = true)]
        public string ProjectId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string UserId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Version { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime StartDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public DateTime EndDate { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Description { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool Objavljene { get; set; }
        [BindProperty(SupportsGet = true)]
        public bool Neobjavljene { get; set; } = true;
        [BindProperty(SupportsGet = true)]
        public List<Release> SelectedReleases { get; set; }

        public async Task OnGetAsync(string user_id, string project_id)
        {
            //Project = await DataProvider.ReadProject(new ObjectId(project_id));
            //CurrentUser = await DataProvider.ReadUser(new ObjectId(user_id));
            UserId = user_id;
            ProjectId = project_id;
            Project = await DataProvider.ReadProject(new ObjectId(project_id));
            Releases = await DataProvider.ReadReleaseByProjectId(new ObjectId(project_id));

            foreach (var release in Releases)
            {
                if (Objavljene && release.Released)
                    SelectedReleases.Add(release);
                if (Neobjavljene && !release.Released)
                    SelectedReleases.Add(release);
            }
        }

        public async Task<IActionResult> OnPostKreirajAsync()
        {
            Release noviRelease = new Release()
            {
                Version = Version,
                StartDate = StartDate,
                EndDate = EndDate,
                Description = Description,
                Released = false,
                BrojTiketa = 0,
                ProjectId = ProjectId
            };
            await DataProvider.CreateRelease(noviRelease);
            return Redirect($"/Releases?user_id={UserId}&project_id={ProjectId}");
        }

        public async Task<IActionResult> OnPostOsveziAsync()
        {

            Project = await DataProvider.ReadProject(new ObjectId(ProjectId));
            Releases = await DataProvider.ReadReleaseByProjectId(new ObjectId(ProjectId));

            foreach (var release in Releases)
            {
                if (Objavljene && release.Released)
                    SelectedReleases.Add(release);
                if (Neobjavljene && !release.Released)
                    SelectedReleases.Add(release);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostObjaviAsync(string id)
        {
            var release = await DataProvider.ReadRelease(new ObjectId(id));
            release.Released = true;
            await DataProvider.UpdateRelease(release);

            return Redirect($"/Releases?user_id={UserId}&project_id={ProjectId}");
        }
    }
}