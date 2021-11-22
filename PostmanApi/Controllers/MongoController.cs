using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using PostmanApi.Models;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using AutoMapper;
using PostmanApi.Interfaces;
using Microsoft.Extensions.Options;

//using Microsoft.EntityFrameworkCore;

//https://hub.docker.com/_/mongo
//docker pull mongo
//docker run -d --name some-mongo -p 27017:27017 -e MONGO_INITDB_ROOT_USERNAME=test -e MONGO_INITDB_ROOT_PASSWORD=test mongo
//docker run -d --name mongodb -p 27017:27017 -e MONGO_INITDB_ROOT_USERNAME=test -e MONGO_INITDB_ROOT_PASSWORD=test mongo
//docker run -it -d mongo
//docker run --name some-mongo -d mongo:tag
//docker exec -it some-mongo bash
//https://docs.mongodb.com/drivers/csharp/
//https://mongodb.github.io/mongo-csharp-driver/2.12/getting_started/quick_tour/
//https://www.tutorialspoint.com/docker/docker_setting_mongodb.htm

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PostmanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MongoController : ControllerBase
    {
        public IConfiguration Configuration { get; }

        private readonly IMongoDBSettings _settings;
        private readonly MongoDBSettings _options;
        private readonly IOptionsMonitor<MongoDBSettings> _optionsDelegate;
        private readonly ILogger<MongoController> _logger;
        private readonly IMapper _mapper;
        public IMongoCollection<MongoContact> Contacts { get; }
        //public IMongoCollection<BsonDocument> Documents { get; }

        public MongoController(IConfiguration config, IMongoDBSettings settings, IOptions<MongoDBSettings> options, IOptionsMonitor<MongoDBSettings> optionsDelegate, ILogger<MongoController> logger, IMapper mapper)
        {
            Configuration = config;
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _optionsDelegate = optionsDelegate ?? throw new ArgumentNullException(nameof(optionsDelegate));
            _logger = logger;
            _mapper = mapper;
            var url = optionsDelegate.CurrentValue.DefaultConnection;// Configuration["MongoDB:DefaultConnection"];

            //var client = new MongoClient();
            //var client = new MongoClient("mongodb://test:test@localhost:27017");
            //var client = new MongoClient("mongodb://localhost:27017,localhost:27018,localhost:27019");
            var client = new MongoClient(url);

            var database = client.GetDatabase(optionsDelegate.CurrentValue.Database);

            Contacts = database.GetCollection<MongoContact>(optionsDelegate.CurrentValue.Collection);
            //Documents = database.GetCollection<BsonDocument>("bar");
            optionsDelegate.OnChange((settings, name) =>
            {

            });
        }

        // GET: api/Mongo
        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [ProducesResponseType(typeof(IEnumerable<MongoContact>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IEnumerable<MongoContact>> Get()
        {
            var filterBuilder = Builders<MongoContact>.Filter;
            var filter = filterBuilder.Gt("ContactId", 50) & filterBuilder.Lte("ContactId", 100);
            //var cursor = Contacts.Find(filter).ToList();
            var cursor = Contacts.Find(filter).ToCursor();
            //var cursor = collection.Find(new MongoContact()).ToCursor();
            foreach (var document1 in cursor.ToEnumerable())
            {
                Debug.WriteLine(document1);
            }

            //return await Documents.AsQueryable().ToListAsync();
            //var contacts = Contacts.AsQueryable().AsEnumerable().Take(1000).ToArray();
            var contacts = Contacts.AsQueryable().AsEnumerable().ToArray();
            return contacts;
            //return new string[] { "value1", "value2" };
            //var documents = await Contacts.Find(new BsonDocument()).ToListAsync();
            //return documents;
        }

        // GET api/<MongoController>/5
        [HttpGet("{id}")]
        [Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [ProducesResponseType(typeof(MongoContact), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<MongoContact> Get(int id)
        {
            //return "value";
            //var filter = Builders<BsonDocument>.Filter.Exists("i");
            var filterBuilder = Builders<MongoContact>.Filter;
            var filter = filterBuilder.Eq("ContactId", id);
            //var cursor = Contacts.Find(filter).ToList();
            var contact = Contacts.Find(filter).SingleOrDefault();
            return contact;
        }

        // POST api/<MongoController>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        public async Task Post([FromBody] MongoContact value)
        {
            //var document = new BsonDocument
            //{
            //    { "text", value },
            //    { "name", "MongoDB" },
            //    { "type", "Database" },
            //    { "count", 1 },
            //    { "info", new BsonDocument
            //    {
            //        { "x", 203 },
            //        { "y", 102 }
            //    }}
            //};
            ////collection.InsertOne(document);
            await Contacts.InsertOneAsync(value);
        }

        // PUT api/<MongoController>/5
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        public async Task Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MongoController>/5
        [HttpDelete("{id}")]
        [Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        public async Task Delete(int id)
        {
        }
    }
}
