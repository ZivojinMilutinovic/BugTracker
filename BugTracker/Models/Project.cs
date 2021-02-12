using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class Project
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string CurrentSprintId { get; set; }
        public string ProjectOwnerId { get; set; }
        public IList<string> SprintsIds { get; set; }
        public IList<string> UsersIds { get; set; }
        public IList<string> ReleasesIds { get; set; }
        public DateTime StartDate { get; set; }

        public Project()
        {
            SprintsIds = new List<string>();
            UsersIds = new List<string>();
            ReleasesIds = new List<string>();
        }
    }
}
