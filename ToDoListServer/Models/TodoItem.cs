using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoListServer.Models
{
    public class TodoItem
    {
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public DateTime TimeCreate { get; set; }
        public DateTime TimeDispose { get; set; }
        public string Content { get; set; }

        public override bool Equals(object obj)
        {
            var other = (TodoItem)obj;
            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
