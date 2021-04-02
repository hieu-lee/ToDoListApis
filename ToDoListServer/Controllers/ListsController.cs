using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoListServer.Models;
using ToDoListServer.Validations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ToDoListServer.Controllers
{
    [Route("[controller]")]
    [ApiKeyAuth]
    [ApiController]
    public class ListsController : ControllerBase
    {
        private readonly IMongoCollection<Account> accounts;
        private readonly IMongoCollection<TodoList> lists;
        private readonly ILogger<ListsController> _logger;

        public ListsController(IMongoClient client, ILogger<ListsController> logger)
        {
            var database = client.GetDatabase("TodoDb");
            accounts = database.GetCollection<Account>("accounts");
            lists = database.GetCollection<TodoList>("lists");
        }

        [HttpGet("{username}")]
        public IActionResult GetListName(string username)
        {
            var myacc = accounts.Find(s => s.username == username).FirstOrDefault();
            if (myacc is null)
            {
                return NotFound();
            }
            return Ok(myacc.lists);
        }

        [HttpGet("{username}/{listid}")]
        public IActionResult GetItems(string username, string listid)
        {
            var res = lists.Find(s => s.id == listid).FirstOrDefault();
            if (res is null)
            {
                return NotFound();
            }
            if (res.owner != username)
            {
                return Unauthorized();
            }
            return Ok(res);
        }

        [HttpPost("{username}")]
        public async Task<IActionResult> CreateNewList(string username, [FromBody] TodoList list)
        {
            var task = lists.InsertOneAsync(list);
            var filteracc = Builders<Account>.Filter.Eq("username", username);
            var myacc = accounts.Find(filteracc).FirstOrDefault();
            myacc.lists.Add(list.name);
            var updateacc = Builders<Account>.Update.Set("lists", myacc.lists);
            accounts.UpdateOne(filteracc, updateacc);
            await task;
            return Ok();
        }

        [HttpPut("{username}/{listid}")]
        public IActionResult UpdateList(string username, string listid, [FromBody] TodoList list)
        {
            var filterlist = Builders<TodoList>.Filter.Eq("_id", listid);
            var oldlist = lists.Find(filterlist).FirstOrDefault();
            if (oldlist is null)
            {
                return NotFound();
            }
            if (oldlist.owner != username)
            {
                return Unauthorized();
            }
            lists.FindOneAndReplace(filterlist, list);
            return Ok();
        }

        [HttpDelete("{username}/{listid}/{itemid}")]
        public IActionResult DeleteItem(string username, string listid, string itemid)
        {
            var filterlist = Builders<TodoList>.Filter.Eq("_id", listid);
            var list = lists.Find(filterlist).FirstOrDefault();
            if (list is null)
            {
                return NotFound();
            }
            if (list.owner != username)
            {
                return Unauthorized();
            }
            list.items.Remove(new TodoItem() { Id = itemid });
            var updatelist = Builders<TodoList>.Update.Set("items", list.items);
            lists.UpdateOne(filterlist, updatelist);
            return Ok();
        }

        [HttpDelete("{username}/{listid}")]
        public IActionResult DeleteList(string username, string listid)
        {
            var filter = Builders<TodoList>.Filter.Eq("_id", listid);
            var res = lists.Find(filter).FirstOrDefault();
            if (res is null)
            {
                return NotFound();
            }
            if (res.owner != username)
            {
                return Unauthorized();
            }
            lists.DeleteOne(filter);
            return Ok();
        }
    }
}
