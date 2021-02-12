using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    
    public class Ticket
    {
        public ObjectId Id { get; set; }
        //public MongoDBRef Project { get; set; }
        //tip tiketa(bug,improvment,task,subtask)
        public string TicketType { get; set; }
        //Naslov tiketa
        public string Summary { get; set; }
        //prioritet tiketa(normal,major,critical,minor,trivial)
        public string Priority { get; set; }
        //id usera kojem je dodeljen tiket
        public string AssigneeId { get; set; }
        //Tiket kojem pripada ovaj trenutni tiket
        public string LinkedTicketId { get; set; }
        //Sprint kojem pripada task
        public string SprintId { get; set; }
        //labela koja se pokazuje uz task i govori koji problem resava(Front-End,Back-end npr)
        public string Label { get; set; }
        //podroban opis taska
        public string Description { get; set; }
        //Release 
        public string ReleaseId { get; set; }

        //tenutno stanje tiketa kad se napravi on je u TO DO stanju
        public string CurrentTicketState { get; set; }

        public string ProjectId { get; set; }
       
    }
}
