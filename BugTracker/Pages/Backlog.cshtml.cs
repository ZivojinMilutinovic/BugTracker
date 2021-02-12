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
    public class BacklogModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public Project CurrentProject { get; set; }
        [BindProperty(SupportsGet = true)]
        public User CurrentUser { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<Sprint> NotCompletedSprints { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<Ticket> Tickets { get; set; }
        [BindProperty(SupportsGet = true)]
        public Dictionary<string, User> Assignees { get; set; }
        public async Task OnGetAsync(string user_id, string project_id)
        {
            CurrentUser = await DataProvider.ReadUser(new ObjectId(user_id));
            CurrentProject = await DataProvider.ReadProject(new ObjectId(project_id));
            var sprints = await DataProvider.ReadAllSprintsForProject(project_id);
            foreach (var sprint in sprints)
            {
                if (!sprint.Completed)
                    NotCompletedSprints.Add(sprint);
            }
            Tickets = await DataProvider.ReadAllTicketsForProject(project_id);
            foreach (var userid in CurrentProject.UsersIds)
            {
                Assignees[userid] = await DataProvider.ReadUser(new ObjectId(userid));
            }
        }
    }
}