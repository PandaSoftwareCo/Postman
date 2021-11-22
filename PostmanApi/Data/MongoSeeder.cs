using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using PostmanApi.Models;

namespace PostmanApi.Data
{
    public static class MongoSeeder
    {
        public static IConfiguration Configuration { get; set; }
        //const string connectionString = "mongodb://user:password@ds047095.mongolab.com:47095/dbname";
        const string connectionString = "mongodb://user:password@ds047095.mongolab.com:47095";

        public static void SeedData(IApplicationBuilder builder, IConfiguration config)
        {
            Configuration = config;
            string connectionString1 = Configuration["MongoDB:DefaultConnection"];
            /*
            //var settings = MongoClientSettings.FromConnectionString(Configuration["Mongo:DefaultConnection"]);
            //settings.SslSettings = new SslSettings()
            //{
            //    EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
            //};
            //settings.ConnectTimeout = TimeSpan.FromMinutes(5);
            //var client1 = new MongoClient(settings);

            //var client = new MongoClient();
            //var client = new MongoClient("mongodb://localhost:27017");
            //var client = new MongoClient("mongodb://guest:guest@localhost:27017");
            //var client = new MongoClient("mongodb://localhost:27017,localhost:27018,localhost:27019");
            var client = new MongoClient(Configuration["Mongo:DefaultConnection"]);

            var database = client.GetDatabase("postman");

            //database.DropCollection("contacts");
            IMongoCollection<MongoContact> contacts = database.GetCollection<MongoContact>("contacts");

            //if (contacts.Count() == 0)
            //{

            //}
            var contact = new MongoContact
            {
                ContactId = 1,
                Name = "James Bond",
                Phone = "604-555-5555",
                Email = "007@mi6.gov.uk"
            };
            contacts.InsertOne(contact);
            */

            ////var client = new MongoClient();
            //var client = new MongoClient("mongodb://localhost:27017");
            ////var client = new MongoClient("mongodb://localhost:27017,localhost:27018,localhost:27019");
            var client = new MongoClient("mongodb://test:test@localhost:27017");
            var databaseNames = client.ListDatabaseNames();
            client.DropDatabase("foo");
            var database = client.GetDatabase("foo");
            var collection = database.GetCollection<BsonDocument>("bar");
            var document = new BsonDocument
            {
                { "name", "MongoDB" },
                { "type", "Database" },
                { "count", 1 },
                { "info", new BsonDocument
                {
                    { "x", 203 },
                    { "y", 102 }
                }}
            };
            collection.InsertOne(document);
            //await collection.InsertOneAsync(document);
            var documents = Enumerable.Range(0, 1000).Select(i => new BsonDocument("counter", i));
            collection.InsertMany(documents);
            //await collection.InsertManyAsync(documents);

            client.DropDatabase("contacts");
            database = client.GetDatabase("contacts");
            var contactCollection = database.GetCollection<MongoContact>("contacts");
            var contacts = Enumerable.Range(1, 10000).Select(i => new MongoContact
            {
                ContactId = i,
                Name = $"James Bond {i}",
                Phone = $"604-555-{i:0000}",
                Email = $"007.{i:0000}@mi6.gov.uk"
            });
            contactCollection.InsertMany(contacts);
        }
    }
}
