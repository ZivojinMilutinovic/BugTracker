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
    public class CreateSprintModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public Project CurrentProject { get; set; }
        [BindProperty(SupportsGet = true)]
        public User CurrentUser { get; set; }
        [BindProperty(SupportsGet = true)]
        public Sprint Sprint { get; set; }
        [BindProperty]
        public string UserIdBackUp { get; set; }
        [BindProperty]
        public string SprintIdBackUp { get; set; }
        public async Task OnGetAsync(string user_id, string project_id)
        {
            CurrentUser = await DataProvider.ReadUser(new ObjectId(user_id));
            CurrentProject = await DataProvider.ReadProject(new ObjectId(project_id));
        }
        public async Task<IActionResult> OnPostCreateAsync()
        {
            Sprint.SprintNumber = await DataProvider.FindNextSprintNumberForProject(Sprint.ProjectId);
            Sprint.Completed = false;
            await DataProvider.CreateSprint(Sprint);
            CurrentProject = await DataProvider.ReadProject(new ObjectId(Sprint.ProjectId));
            var projectSprints = await DataProvider.ReadAllSprintsForProject(Sprint.ProjectId);
            CurrentProject.SprintsIds = new List<string>();
            foreach (var s in projectSprints)
            {
                CurrentProject.SprintsIds.Add(s.Id.ToString());
            }
            if (CurrentProject.CurrentSprintId == null)
                CurrentProject.CurrentSprintId = Sprint.Id.ToString();
            await DataProvider.UpdateProject(CurrentProject);
            return Redirect("/Backlog?user_id=" + UserIdBackUp + "&project_id=" + Sprint.ProjectId);
        }
        public async Task<IActionResult> OnPostUpdateAsync()
        {
            var oldSprint = await DataProvider.ReadSprint(new ObjectId(SprintIdBackUp));
            oldSprint.SprintGoals = Sprint.SprintGoals;
            oldSprint.StartDate = Sprint.StartDate;
            oldSprint.EndDate = Sprint.EndDate;
            await DataProvider.UpdateSprint(Sprint);

            return Redirect("/Backlog?user_id=" + UserIdBackUp + "&project_id=" + Sprint.ProjectId);
        }
    }
}