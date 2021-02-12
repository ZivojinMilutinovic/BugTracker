using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class User
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        //FE,BE,
        public string Position { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public IList<string> Projects { get; set; }

        public User()
        {
            Projects = new List<string>();
        }
    }
}
