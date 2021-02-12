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
    public class MyProjectsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public List<Project> Projects { get; set; }
        [BindProperty(SupportsGet = true)]
        public User CurrentUser { get; set; }
        [BindProperty(SupportsGet = true)]
        public Dictionary<string, User> ProjectsOwners { get; set; }
        [BindProperty]
        public string ForDelete { get; set; }
        [BindProperty(SupportsGet = true)]
        public string UserIdBackUp { get; set; }
        public async Task OnGetAsync(string user_id)
        {
            CurrentUser = await DataProvider.ReadUser(new ObjectId(user_id));
            Projects = await DataProvider.ReadAllProjectsForUser(user_id);
            foreach (var project in Projects)
            {
                var user = await DataProvider.ReadUser(new ObjectId(project.ProjectOwnerId));
                ProjectsOwners[project.Id.ToString()] = user;
            }
        }
        public async Task<IActionResult> OnPostDeleteAsync(string user_id)
        {
            var project = await DataProvider.ReadProject(new ObjectId(ForDelete));
            foreach (var id in project.UsersIds)
            {
                var user = await DataProvider.ReadUser(new ObjectId(id));
                user.Projects.Remove(ForDelete);
                await DataProvider.UpdateUser(user);
            }
            await DataProvider.DeleteProject(new ObjectId(ForDelete));
            return Redirect("/MyProjects?user_id=" + UserIdBackUp);
        }
    }
}