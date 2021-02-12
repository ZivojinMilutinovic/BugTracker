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
    public class ActiveSprintModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public Project Project { get; set; }
        [BindProperty(SupportsGet = true)]
        public Sprint ActiveSprint { get; set; }
        [BindProperty(SupportsGet = true)]
        public User CurrentUser { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<string> TicketTypes { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<string> PriorityTypes { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<string> Labels { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<User> AllUsers { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<Sprint> AllSprints { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<Ticket> AllTickets { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<Release> AllReleases { get; set; }
        [BindProperty]
        public Ticket Ticket { get; set; }
        [BindProperty]
        public string SCurrentUserId { get; set; }
        [BindProperty]
        public string SCurrentProjectId { get; set; }
        [BindProperty]
        public string SCurrentSprintId { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<string> ListaStanjaTiketa { get; set; }
        public async Task OnGetAsync(string user_id, string project_id)
        {
            Project = await DataProvider.ReadProject(new ObjectId(project_id));
            CurrentUser = await DataProvider.ReadUser(new ObjectId(user_id));
            if (Project.CurrentSprintId != null)
                ActiveSprint = await DataProvider.ReadSprint(new ObjectId(Project.CurrentSprintId));
            else ActiveSprint = null;
            AllUsers = await DataProvider.ReadAllUsers();

          
            AllSprints = await DataProvider.ReadAllSprintsForProject(project_id);
            AllSprints.RemoveAll(x => x.Completed == true);
            

            if (ActiveSprint != null)
                AllTickets = await DataProvider.ReadAllTicketsForSprint(ActiveSprint.Id);
            else AllTickets = new List<Ticket>();

            
            AllReleases = await DataProvider.ReadReleaseByProjectId(new ObjectId(project_id));
            

            TicketTypes = new List<string>
            {
                "Bug",
                "Improvment",
                "Task",
                "Subtask"
            };
            PriorityTypes = new List<string>
            {
                "Critical",
                "Major",
                "Normal",
                "Minor",
                "Trivial"
            };
            Labels = new List<string>
            {
                "Front-end",
                "Back-end",
                "Android",
                "IOs",
                "QA"
            };
            ListaStanjaTiketa = new List<string>
            {
                "TO DO",
                "IN PROGRESS",
                "TESTABLE",
                "IN TESTING",
                "BLOCKED",
                "DONE"
            };

        }
        public async Task<IActionResult> OnPostAsync()
        {
            Ticket.CurrentTicketState = "TO DO";
            Ticket.ProjectId = SCurrentProjectId;
            await DataProvider.CreateTicket(Ticket);
            //KADA se kreira tiket takodje moramo da ubacimo informacije u Sprint i u Release sa kojim je
            //tiket povezan
            if (Ticket.SprintId != null)
            {
                var sprint = await DataProvider.ReadSprint(new ObjectId(Ticket.SprintId));
                if (sprint.TicketsIds == null) sprint.TicketsIds = new List<string>();
                sprint.TicketsIds.Add(Ticket.Id.ToString());
                await DataProvider.UpdateSprint(sprint);
            }
            if (Ticket.ReleaseId != null)
            {
                var release = await DataProvider.ReadRelease(new ObjectId(Ticket.ReleaseId));
                if (release.TicketsIds == null) release.TicketsIds = new List<string>();
                release.TicketsIds.Add(Ticket.Id.ToString());
                await DataProvider.UpdateRelease(release);
            }


            return Redirect($"/ActiveSprint?user_id={SCurrentUserId}&project_id={SCurrentProjectId}");
        }
        public async Task<IActionResult> OnPostCompleteAsync()
        {
            var project = await DataProvider.ReadProject(new ObjectId(SCurrentProjectId));
            var sprint = await DataProvider.ReadSprint(new ObjectId(SCurrentSprintId));
            int next = -1;
            foreach (var sId in project.SprintsIds)
            {
                var s = await DataProvider.ReadSprint(new ObjectId(sId));
                if (s.SprintNumber == sprint.SprintNumber + 1)
                {
                    next = s.SprintNumber;
                    project.CurrentSprintId = sId;
                    break;
                }
            }
            if (next == -1)
                project.CurrentSprintId = null;
            await DataProvider.UpdateProject(project);
            sprint.Completed = true;
            await DataProvider.UpdateSprint(sprint);
            foreach (var tId in sprint.TicketsIds)
            {
                var ticket = await DataProvider.ReadTicket(new ObjectId(tId));
                ticket.SprintId = null;
                await DataProvider.UpdateTicket(ticket);
            }
            return Redirect($"/ActiveSprint?user_id={SCurrentUserId}&project_id={SCurrentProjectId}");
        }
        public int VratiVreme()
        {
            return (int)(ActiveSprint.EndDate - ActiveSprint.StartDate).TotalDays;
        }
    }
}