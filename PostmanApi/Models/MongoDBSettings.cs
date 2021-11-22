using PostmanApi.Interfaces;

namespace PostmanApi.Models
{
    public class MongoDBSettings : IMongoDBSettings
    {
        public string DefaultConnection { get; set; }
        public string Database { get; set; }
        public string Collection { get; set; }
    }
}
