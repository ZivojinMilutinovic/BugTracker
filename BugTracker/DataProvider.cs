using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace BugTracker
{
    public static class DataProvider
    {
        #region User
        public static async  Task<List<User>> ReadAllUsers()
        {
            var database = SessionManager.Connect();
            var collection=database.GetCollection<User>("users");
            var users = await collection.Find(new BsonDocument()).ToListAsync();
            return users;
        }
        public static async Task<User> ReadUserByUsername(string username)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<User>("users");
            var user = await collection.Find(user => user.UserName.Equals(username)).FirstOrDefaultAsync();
            return user;
        }
        public static async Task CreateUser(User user)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<User>("users");
            await collection.InsertOneAsync(user);
        }
        public static async Task<User> ReadUser(ObjectId id)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<User>("users");
            var user = await collection.Find(user => user.Id.Equals(id)).FirstOrDefaultAsync();
            return user;
        }
        public static async Task UpdateUser(User user)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<User>("users");
            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            var result = await collection.ReplaceOneAsync(filter, user);
        }
        public static async Task DeleteUser(ObjectId id)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<User>("users");
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            await collection.DeleteOneAsync(filter);
        }
        #endregion
        #region Project
        public static async Task<Project> ReadProject(ObjectId id)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Project>("projects");
            var project = await collection.Find(project => project.Id.Equals(id)).FirstOrDefaultAsync();
            return project;
        }
        public static async Task CreateProject(Project project)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Project>("projects");
            await collection.InsertOneAsync(project);
        }
        public static async Task UpdateProject(Project project)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Project>("projects");
            var filter = Builders<Project>.Filter.Eq(p => p.Id, project.Id);
            var result = await collection.ReplaceOneAsync(filter, project);
        }
        public static async Task DeleteProject(ObjectId id)
        {
            var database = SessionManager.Connect();
            var collectionR = database.GetCollection<Release>("releases");
            var forDeleteR = Builders<Release>.Filter.Eq(r => r.ProjectId, id.ToString());
            await collectionR.DeleteManyAsync(forDeleteR);

            var collectionS = database.GetCollection<Sprint>("sprints");
            var forDeleteS = Builders<Sprint>.Filter.Eq(s => s.ProjectId, id.ToString());
            await collectionS.DeleteManyAsync(forDeleteS);

            var collectionT = database.GetCollection<Ticket>("tickets");
            var forDeleteT = Builders<Ticket>.Filter.Eq(t => t.ProjectId, id.ToString());
            await collectionT.DeleteManyAsync(forDeleteT);

            var collection = database.GetCollection<Project>("projects");
            var filter = Builders<Project>.Filter.Eq(p => p.Id, id);
            await collection.DeleteOneAsync(filter);
        }
        public static async Task<List<Project>> ReadAllProjectsForUser(string userId)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Project>("projects");
            var projects = await collection.Find(project => project.UsersIds.Contains(userId)).ToListAsync();
            return projects;
        }

        public static async Task<ObjectId> FindProjectIdForUser(string userId, DateTime date)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Project>("projects");
            var project = await collection.Find(project => project.ProjectOwnerId.Equals(userId) && project.StartDate.Equals(date)).FirstOrDefaultAsync();
            return project.Id;
        }
        #endregion
        #region Sprint
        public static async Task<Sprint> ReadSprint(ObjectId id)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Sprint>("sprints");
            var sprint = await collection.Find(sprint => sprint.Id.Equals(id)).FirstOrDefaultAsync();
            return sprint;
        }
        public static async Task<List<Sprint>> ReadAllSprints()
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Sprint>("sprints");
            var sprints = await collection.Find(new BsonDocument()).ToListAsync();
            return sprints;
        }
        public static async Task CreateSprint(Sprint sprint)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Sprint>("sprints");
            await collection.InsertOneAsync(sprint);
        }
        public static async Task UpdateSprint(Sprint sprint)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Sprint>("sprints");
            var filter = Builders<Sprint>.Filter.Eq(s => s.Id, sprint.Id);
            var result = await collection.ReplaceOneAsync(filter, sprint);
        }
        public static async Task DeleteSprint(ObjectId id)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Sprint>("sprints");
            var filter = Builders<Sprint>.Filter.Eq(s => s.Id, id);
            await collection.DeleteOneAsync(filter);
        }
        public static async Task<List<Sprint>> ReadAllSprintsForProject(string project_id)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Sprint>("sprints");
            var sprints = await collection.Find(sprint => sprint.ProjectId.Equals(project_id)).ToListAsync();
            return sprints;
        }
        public static async Task<int> FindNextSprintNumberForProject(string project_id)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Sprint>("sprints");
            var sprints = await collection.Find(sprint => sprint.ProjectId.Equals(project_id)).ToListAsync();
            if (sprints.Count == 0)
                return 1;
            var max = sprints.OrderByDescending(x => x.SprintNumber).First().SprintNumber;
            return max + 1;
        }
        #endregion
        #region Ticket
        public static async Task<List<Ticket>> ReadAllTicketsForRelease(ObjectId releaseId)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Ticket>("tickets");
            var tickets = await collection.Find(ticket => ticket.ReleaseId.Equals(releaseId)).ToListAsync();
            return tickets;
        }
        public static async Task<List<Ticket>> ReadAllTickets()
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Ticket>("tickets");
            var tickets = await collection.Find(new BsonDocument()).ToListAsync();
            return tickets;
        }
        public static async Task<List<Ticket>> ReadAllTicketsForSprint(ObjectId sprintId)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Ticket>("tickets");
            var tickets = await collection.Find(ticket=>ticket.SprintId.Equals(sprintId)).ToListAsync();
            return tickets;
        }
        public static async Task<List<Ticket>> ReadAllTicketsForProject(string projectId)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Ticket>("tickets");
            var tickets = await collection.Find(ticket => ticket.ProjectId.Equals(projectId)).ToListAsync();
            return tickets;
        }
        public static async Task CreateTicket(Ticket ticket)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Ticket>("tickets");
            await collection.InsertOneAsync(ticket);
        }
        public static async Task<Ticket> ReadTicket(ObjectId id)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Ticket>("tickets");
            var ticket = await collection.Find(user => user.Id.Equals(id)).FirstOrDefaultAsync();
            return ticket;
        }
        public static async Task UpdateTicket(Ticket ticket)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Ticket>("tickets");
            var filter = Builders<Ticket>.Filter.Eq(tic => tic.Id, ticket.Id);
            var result = await collection.ReplaceOneAsync(filter, ticket);
        }
        public static async Task DeleteTicket(ObjectId id)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Ticket>("tickets");
            var filter = Builders<Ticket>.Filter.Eq(tic => tic.Id, id);
            await collection.DeleteOneAsync(filter);
        }
        #endregion
        #region Release
        public static async Task<List<Release>> ReadAllReleases()
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Release>("releases");
            var releases = await collection.Find(new BsonDocument()).ToListAsync();
            return releases;
        }
        public static async Task<Release> ReadRelease(ObjectId id)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Release>("releases");
            var release = await collection.Find(project => project.Id.Equals(id)).FirstOrDefaultAsync();
            return release;
        }
        public static async Task CreateRelease(Release release)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Release>("releases");
            var projects = database.GetCollection<Project>("projects");
            release.Id = ObjectId.GenerateNewId();
            var project = await projects.Find(project => project.Id.Equals(new ObjectId(release.ProjectId))).FirstOrDefaultAsync();
            project.ReleasesIds.Add(release.Id.ToString());
            await collection.InsertOneAsync(release);
            await UpdateProject(project);
        }
        public static async Task UpdateRelease(Release release)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Release>("releases");
            var filter = Builders<Release>.Filter.Eq(rel => rel.Id, release.Id);
            var result = await collection.ReplaceOneAsync(filter, release);
        }
        public static async Task DeleteRelease(ObjectId id)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Release>("releases");
            var filter = Builders<Release>.Filter.Eq(rel => rel.Id, id);
            await collection.DeleteOneAsync(filter);
        }
        public static async Task<List<Release>> ReadReleaseByProjectId(ObjectId projectId)
        {
            var database = SessionManager.Connect();
            var collection = database.GetCollection<Release>("releases");
            var releases = await collection.Find(release => release.ProjectId.Equals(projectId)).ToListAsync();
            foreach (var release in releases)
            {
                var listOfTickets = await ReadAllTicketsForRelease(release.Id);
                foreach (var ticket in listOfTickets)
                {
                    if (ticket.CurrentTicketState.Equals("TO DO"))
                        release.ToDo++;
                    else if (ticket.CurrentTicketState.Equals("IN PROGRESS"))
                        release.InProgress++;
                    else if (ticket.CurrentTicketState.Equals("DONE"))
                        release.Done++;
                    release.BrojTiketa++;
                }
            }
            return releases;
        }
        #endregion
    }



}
