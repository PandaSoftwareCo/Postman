using Postman.Interfaces;

namespace Postman.Models
{
    public class AppSettings : IAppSettings
    {
        public string Url { get; set; }
        public string AuthUrl { get; set; }
        public int Count { get; set; }
        public bool UseToken { get; set; }
        public bool UseBasic { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
