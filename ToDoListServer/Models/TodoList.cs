using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoListServer.Models
{
    public class TodoList
    {
        [BsonId]
        public string id { get; set; } = ObjectId.GenerateNewId().ToString();
        [BsonElement("name")]
        public string name { get; set; }
        [BsonElement("owner")]
        public string owner { get; set; }
        [BsonElement("image")]
        public byte[] image { get; set; }
        [BsonElement("items")]
        public HashSet<TodoItem> items { get; set; } = new();
    }
}
