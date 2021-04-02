using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Linq;
using ToDoListServer.Models;
using ToDoListServer.Services;
using ToDoListServer.Validations;

namespace ToDoListServer.Controllers
{
    [Route("[controller]")]
    [ApiKeyAuth]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IMongoCollection<Account> accounts;
        private readonly EncryptionAndCompressService encryptionService;
        private readonly ILogger<AccountsController> _logger;
        public AccountsController(IMongoClient client, ILogger<AccountsController> logger, EncryptionAndCompressService encrypt)
        {
            encryptionService = encrypt;
            _logger = logger;
            var database = client.GetDatabase("TodoDb");
            accounts = database.GetCollection<Account>("accounts");
        }

        [HttpPost("sign-up")]
        public SignResult SignUp([FromBody] Account account)
        {
            try
            {
                accounts.InsertOne(account);
                return new SignResult() { success = true };
            }
            catch(MongoDuplicateKeyException)
            {
                return new SignResult() { success = false, err = "Your username has been taken" };
            }
        }

        [HttpPost("sign-in")]
        public IActionResult SignIn([FromBody] Account account)
        {
            var acc = accounts.Find(s => s.username == account.username).FirstOrDefault();
            if (acc is null)
            {
                return NotFound("Your username does not exist");
            }
            if (acc.password != encryptionService.Encrypt(account.password))
            {
                return Unauthorized("Your password is incorrect");
            }
            return Ok();
        }
    }
}
