using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class Sprint
    {
        public ObjectId Id { get; set; }
        public int SprintNumber { get; set; }
        public string SprintGoals { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Completed { get; set; } = false;
        public IList<string> TicketsIds { get; set; }
        public string ProjectId { get; set; }

        public Sprint()
        {
            TicketsIds = new List<string>();
        }
    }
}
