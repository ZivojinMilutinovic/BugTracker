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
    public class CreateProjectModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public Project Project { get; set; }
        [BindProperty(SupportsGet = true)]
        public User CurrentUser { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<User> AllUsers { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<string> SelectedUsers { get; set; }
        [BindProperty]
        public string ProjectIdBackUp { get; set; }
        public async Task OnGetAsync(string user_id, string project_id)
        {
            CurrentUser = await DataProvider.ReadUser(new ObjectId(user_id));
            if (project_id != null)
            {
                Project = await DataProvider.ReadProject(new ObjectId(project_id));
                SelectedUsers = Project.UsersIds.ToList<string>();
            }
            AllUsers = await DataProvider.ReadAllUsers();
        }
        public async Task<IActionResult> OnPostCreateAsync()
        {
            string user_id = Project.ProjectOwnerId;
            Project.StartDate = DateTime.Now;
            Project.UsersIds = SelectedUsers;
            Project.UsersIds.Add(user_id);
            await DataProvider.CreateProject(Project);
            Project.Id = await DataProvider.FindProjectIdForUser(user_id, Project.StartDate);
            foreach (var id in Project.UsersIds)
            {
                var user = await DataProvider.ReadUser(new ObjectId(id));
                user.Projects.Add(Project.Id.ToString());
                await DataProvider.UpdateUser(user);
            }
            return Redirect("/MyProjects?user_id=" + user_id);
        }
        public async Task<IActionResult> OnPostUpdateAsync()
        {
            string user_id = Project.ProjectOwnerId;
            string project_id = ProjectIdBackUp;
            var oldProject = await DataProvider.ReadProject(new ObjectId(project_id));
            foreach (var id in oldProject.UsersIds)
            {
                var user = await DataProvider.ReadUser(new ObjectId(id));
                if (!SelectedUsers.Contains(user.Id.ToString()))
                {
                    user.Projects.Remove(project_id);
                    await DataProvider.UpdateUser(user);
                }
            }
            oldProject.Name = Project.Name;
            Project = oldProject;
            Project.UsersIds = SelectedUsers;
            Project.UsersIds.Add(user_id);
            await DataProvider.UpdateProject(Project);
            foreach (var id in Project.UsersIds)
            {
                var user = await DataProvider.ReadUser(new ObjectId(id));
                if (!user.Projects.Contains(Project.Id.ToString()))
                {
                    user.Projects.Add(Project.Id.ToString());
                    await DataProvider.UpdateUser(user);
                }
            }
            return Redirect("/MyProjects?user_id=" + user_id);
        }
    }
}