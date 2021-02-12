using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker
{
    public static class SessionManager
    {
        public static IMongoDatabase Database;
        public static IMongoDatabase Connect()
        {
            
            var client = new MongoClient("mongodb+srv://admin:admin@cluster-bugtracker.fj2vq.mongodb.net?retryWrites=true&w=majority");
            if(Database==null)
            Database = client.GetDatabase("bug_tracker");
            return Database;
        }
    }
}
