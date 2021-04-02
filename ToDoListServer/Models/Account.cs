using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoListServer.Models
{
    public class Account
    {
        [BsonId]
        public string username { get; set; }
        [BsonElement("password")]
        public string password { get; set; }
        [BsonElement("lists")]
        public HashSet<string> lists { get; set; } = new();
    }
}
