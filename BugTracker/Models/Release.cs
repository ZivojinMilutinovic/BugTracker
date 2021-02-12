using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class Release
    {
        public ObjectId Id { get; set; }
        public string Version { get; set; }
        public IList<string> TicketsIds { get; set; }
        public string ProjectId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public bool Released { get; set; } = false;
        public int Done { get; set; }
        public int InProgress { get; set; }
        public int ToDo { get; set; }
        public int BrojTiketa { get; set; }

        public Release()
        {
            TicketsIds = new List<string>();
        }
    }
}
