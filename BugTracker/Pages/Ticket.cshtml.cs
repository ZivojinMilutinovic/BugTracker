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
    public class TicketModel : PageModel
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
        [BindProperty(SupportsGet =true)]
        public Ticket Ticket { get; set; }
        [BindProperty]
        public string SCurrentUserId { get; set; }
        [BindProperty]
        public string SCurrentProjectId { get; set; }
        [BindProperty]
        public string SCurrentTicketId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string OldReleaseId { get; set; }
        [BindProperty(SupportsGet = true)]
        public string OldSprintId { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public List<string> ListaStanjaTiketa { get; set; }
        public async Task OnGetAsync(string user_id, string project_id,string ticket_id)
        {
            Project = await DataProvider.ReadProject(new ObjectId(project_id));
            CurrentUser = await DataProvider.ReadUser(new ObjectId(user_id));
            Ticket= await DataProvider.ReadTicket(new ObjectId(ticket_id));
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
            OldReleaseId = Ticket.ReleaseId;
            OldSprintId = Ticket.SprintId;
            
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
        public async Task<IActionResult> OnPostUpdateAsync()
        {
            Ticket.Id = new ObjectId(SCurrentTicketId);
            Ticket.ProjectId = SCurrentProjectId;
            await DataProvider.UpdateTicket(Ticket);
            
            if (OldSprintId != null)
            {
                var oldSprint = await DataProvider.ReadSprint(new ObjectId(OldSprintId));
                oldSprint.TicketsIds.Remove(Ticket.Id.ToString());
                await DataProvider.UpdateSprint(oldSprint);
            }
            if (Ticket.SprintId != null)
            {
                var sprint = await DataProvider.ReadSprint(new ObjectId(Ticket.SprintId));
                sprint.TicketsIds.Add(Ticket.Id.ToString());
                await DataProvider.UpdateSprint(sprint);
            }

            if (OldReleaseId != null)
            {
                var oldRelease = await DataProvider.ReadRelease(new ObjectId(OldReleaseId));

                oldRelease.TicketsIds.Remove(Ticket.Id.ToString());
                await DataProvider.UpdateRelease(oldRelease);
            }
            if (Ticket.ReleaseId != null)
            {
                var release = await DataProvider.ReadRelease(new ObjectId(Ticket.ReleaseId));
                release.TicketsIds.Add(Ticket.Id.ToString());
                await DataProvider.UpdateRelease(release);
            }
            
           
            return  Redirect($"/ActiveSprint?user_id={SCurrentUserId}&project_id={SCurrentProjectId}");
        }
        public async Task<IActionResult> OnPostDeleteAsync()
        {
            Ticket.Id = new ObjectId(SCurrentTicketId);
            await DataProvider.DeleteTicket(Ticket.Id);
            if (OldSprintId != null)
            {
                var oldSprint = await DataProvider.ReadSprint(new ObjectId(OldSprintId));
                oldSprint.TicketsIds.Remove(Ticket.Id.ToString());
                await DataProvider.UpdateSprint(oldSprint);
            }
            if (OldReleaseId != null)
            {
                var oldRelease = await DataProvider.ReadRelease(new ObjectId(OldReleaseId));
                oldRelease.TicketsIds.Remove(Ticket.Id.ToString());
                await DataProvider.UpdateRelease(oldRelease);
            }
            
            return Redirect($"/ActiveSprint?user_id={SCurrentUserId}&project_id={SCurrentProjectId}");
        }

    }
}
