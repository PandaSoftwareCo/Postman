using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace PostmanApi.Models
{
    public class MongoContact
    {
        [BsonId]
        [JsonIgnore]
        public ObjectId DocumentId { get; set; }

        [BsonElement]
        [JsonProperty("contact_id")]
        public int ContactId { get; set; }

        [BsonElement]
        [JsonProperty("contact_name")]
        public string Name { get; set; }

        [BsonElement]
        public string Phone { get; set; }

        [BsonElement]
        public string Email { get; set; }
    }
}
