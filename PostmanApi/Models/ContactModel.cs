using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace PostmanApi.Models
{
    [Table("Contacts", Schema = "Contacts")]
    [Serializable]
    [XmlRoot("Contact")]
    public class ContactModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [XmlAttribute("ContactId")]
        public long ContactId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Name")]
        [JsonProperty(PropertyName = "name")]
        [XmlElement("Name")]
        //[JsonPropertyName("name")]
        public string Name { get; set; }

        [EmailAddress]
        [StringLength(100)]
        [JsonProperty(PropertyName = "email")]
        [XmlElement("Email")]
        //[JsonPropertyName("email")]
        public string Email { get; set; }

        [Phone]
        [StringLength(20)]
        [JsonProperty(PropertyName = "phone")]
        [XmlElement("Phone")]
        public string Phone { get; set; }

        //[NotMapped]
        [Range(1, 150, ErrorMessage = "Age invalid (1-150).")]
        [JsonProperty(PropertyName = "age")]
        [XmlElement("Age")]
        public int? Age { get; set; }

        //[NotMapped]
        //[JsonPropertyName("birthday")]
        [JsonProperty(PropertyName = "birthday")]
        [XmlElement("Birthday")]
        public DateTime? Birthday { get; set; }

        //[NotMapped]
        //[JsonPropertyName("isAdult")]
        //[Range(typeof(bool), "true", "true",
        //    ErrorMessage = "This form disallows unapproved ships.")]
        [XmlElement("IsAdult")]
        public bool IsAdult { get; set; }

        //[NotMapped]
        [EnumDataType(typeof(Gender))]
        [XmlIgnore]
        public Gender? Gender { get; set; } = null;

        //[NotMapped]
        [EnumDataType(typeof(Position))]
        [XmlIgnore]
        public Position? Position { get; set; } = null;

        //[NotMapped]
        [StringLength(100)]
        //[JsonPropertyName("imageName")]
        [XmlElement("ImageName")]
        public string ImageName { get; set; }

        //[NotMapped]
        [StringLength(100)]
        //[JsonPropertyName("contentType")]
        [XmlElement("ContentType")]
        public string ContentType { get; set; }

        //[NotMapped]
        //[JsonPropertyName("imageSize")]
        [XmlElement("ImageSize")]
        public long? Size { get; set; }

        //[NotMapped]
        [XmlIgnore]
        public byte[] Image { get; set; }

        //[NotMapped]
        //[JsonPropertyName("imageWidth")]
        [XmlElement("ImageWidth")]
        public int? ImageWidth { get; set; }

        //[NotMapped]
        //[JsonPropertyName("imageHeight")]
        [XmlElement("ImageHeight")]
        public int? ImageHeight { get; set; }
    }
}
