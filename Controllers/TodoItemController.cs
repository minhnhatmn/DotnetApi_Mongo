using ApiTodo.Model;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using SharpCompress.Archives;
using SharpCompress.Common;
using Newtonsoft.Json;

namespace ApiTodo.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TodoItemController : ControllerBase
    {
        private readonly IConfiguration _config;
        private string? connectionString;
        IMongoDatabase db;

        public TodoItemController(IConfiguration config)
        {
            _config = config;
            connectionString = _config.GetValue<string>("todoDatabase:ConnectionString");
            MongoClient dbClient = new MongoClient(connectionString);
            db = dbClient.GetDatabase("Todo");
        }

        /// <summary>
        /// Retrieves a list of TodoItems.
        /// </summary>
        /// <returns>The list of TodoItems.</returns>
        [HttpGet(template: "GetTodoList")]
        public IActionResult GetTodoList()
        {
            var collection = db.GetCollection<TodoItem>("TodoItem");
            var listTodoItem = collection.AsQueryable().ToList();
            return Ok(listTodoItem);
        }

        [HttpGet]
        public IActionResult GetColorList()
        {
            var collection = db.GetCollection<Color>("Color");
            var list = collection.Find(new BsonDocument()).ToList();
            return Ok(list);
        }

        [HttpPost]
        public IActionResult AddTodoItem(TodoItem value)
        {
            var collection = db.GetCollection<TodoItem>("TodoItem");
            collection.InsertOneAsync(value);
            return Ok();
        }

        [HttpPost]
        public IActionResult GetTodoItem(ObjectId value)
        {
            var collection = db.GetCollection<TodoItem>("TodoItem");
            var item = collection.Find(t => t.Id == value).FirstOrDefault();
            return Ok(item);
        }

        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = false)]
        public IActionResult AddFileToTranscription(IFormFile file)
        {
            using (var sr = file.OpenReadStream())
            {
                using (var archive = ArchiveFactory.Open(sr))
                {
                    foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                    {
                        var jsonEntries = archive.Entries
                            .Where(entry => !entry.IsDirectory && entry.Key.EndsWith(".json", StringComparison.OrdinalIgnoreCase));

                        foreach (var jsonEntry in jsonEntries)
                        {
                            using (var jsonStream = jsonEntry.OpenEntryStream())
                            using (var reader = new StreamReader(jsonStream))
                            {
                                string jsonContent = reader.ReadToEnd();
                                var jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<TestParent>(jsonContent);
                                return Ok("Done");
                            }
                        }
                    }
                }
            }
            return Ok();
        }

        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid file");
            }

            // Process the file or save the chunk to storage
            // For demonstration, just print the length of each chunk
            Console.WriteLine($"Received chunk: {file.Length} bytes");

            return Ok();
        }
    }
    public class TestImage
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Data { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public int SortOrder { get; set; }
        public string Action { get; set; }
        public string Guid { get; set; }
        public int MasterId { get; set; }
    }
    public class TestParent
    {
        public List<TestImage> Images { get; set; }
    }
}