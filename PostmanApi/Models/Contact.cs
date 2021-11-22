using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostmanApi.Models
{
    [Table("Contacts", Schema = "Contacts")]
    public class Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ContactId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Name")]
        [JsonProperty(PropertyName = "name")]
        //[JsonPropertyName("name")]
        public string Name { get; set; }

        [EmailAddress]
        [StringLength(100)]
        [JsonProperty(PropertyName = "email")]
        //[JsonPropertyName("email")]
        public string Email { get; set; }

        [Phone]
        [StringLength(20)]
        [JsonProperty(PropertyName = "phone")]
        public string Phone { get; set; }

        //[NotMapped]
        [Range(1, 150, ErrorMessage = "Age invalid (1-150).")]
        [JsonProperty(PropertyName = "age")]
        public int? Age { get; set; }

        //[NotMapped]
        //[JsonPropertyName("birthday")]
        [JsonProperty(PropertyName = "birthday")]
        public DateTime? Birthday { get; set; }

        //[NotMapped]
        //[JsonPropertyName("isAdult")]
        //[Range(typeof(bool), "true", "true",
        //    ErrorMessage = "This form disallows unapproved ships.")]
        public bool IsAdult { get; set; }

        //[NotMapped]
        [EnumDataType(typeof(Gender))]
        public Gender? Gender { get; set; } = null;

        //[NotMapped]
        [EnumDataType(typeof(Position))]
        public Position? Position { get; set; } = null;

        //[NotMapped]
        [StringLength(100)]
        //[JsonPropertyName("imageName")]
        public string ImageName { get; set; }

        //[NotMapped]
        [StringLength(100)]
        //[JsonPropertyName("contentType")]
        public string ContentType { get; set; }

        //[NotMapped]
        //[JsonPropertyName("imageSize")]
        public long? Size { get; set; }

        //[NotMapped]
        public byte[] Image { get; set; }

        //[NotMapped]
        //[JsonPropertyName("imageWidth")]
        public int? ImageWidth { get; set; }

        //[NotMapped]
        //[JsonPropertyName("imageHeight")]
        public int? ImageHeight { get; set; }
    }

    public enum Gender
    {
        Male,
        Female
    }

    public enum Position
    {
        Tinker,
        Taylor,
        Soldier,
        Spy
    }
}
