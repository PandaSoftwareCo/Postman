using System.Threading.Tasks;

namespace Postman.Interfaces
{
    public interface IHttpService
    {
        Task<string> AuthTokenAsync(string url);
        Task<string> AuthBasicAsync(string url);

        Task<string> PostAsync(string url, string requestString, bool useToken, string token, bool useBasic);
    }
}
