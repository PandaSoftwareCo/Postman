namespace Postman.Interfaces
{
    public interface IAppSettings
    {
        string Url { get; set; }
        string AuthUrl { get; set; }
        int Count { get; set; }
        bool UseToken { get; set; }
        bool UseBasic { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
    }
}
