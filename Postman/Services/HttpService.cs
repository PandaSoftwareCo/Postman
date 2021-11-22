using System;
using Postman.Interfaces;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Postman.Models;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Threading;
using System.Windows;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Mime;

namespace Postman.Services
{
    public class HttpService : IHttpService
    {
        //private readonly IHttpClientFactory _clientFactory;
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpService> _logger;
        public IConfigurationRoot Configuration { get; }
        private readonly IAppSettings _settings;

        public HttpService(HttpClient client, IHttpClientFactory clientFactory, ILogger<HttpService> logger, IAppSettings settings, IOptions<AppSettings> options, IConfigurationRoot config)
        {
            //_clientFactory = clientFactory;
            //_httpClient = _clientFactory.CreateClient("HttpClient");
            _httpClient = client ?? throw new ArgumentNullException(nameof(client));
            //_httpClient.Timeout = TimeSpan.FromSeconds(100);
            _httpClient.Timeout = TimeSpan.FromMinutes(30);
            //_httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Configuration = config ?? throw new ArgumentNullException(nameof(config));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        //[DebuggerStepThrough]
        public async Task<string> AuthTokenAsync(string url)
        {
            var userName = _settings.UserName;// Configuration["AppSettings:UserName"];
            var password = _settings.Password;// Configuration["AppSettings:Password"];
            var authToken = Encoding.ASCII.GetBytes($"{userName}:{password}");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            var tokenString = await _httpClient.GetStringAsync(url);
            var tokenModel = JsonSerializer.Deserialize<TokenModel>(tokenString);
            var tokenParts = tokenModel.token.Split('.');
            var header = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(tokenParts[0]));
            _logger.LogInformation($"TOKEN: {tokenModel.token}");
            return tokenModel.token;
        }

        //[DebuggerStepThrough]
        public async Task<string> AuthBasicAsync(string url)
        {
            var userName = _settings.UserName;
            var password = _settings.Password;

            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(ACCESS_TOKEN);
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ACCESS_TOKEN);
            //var byteArray = Encoding.ASCII.GetBytes("username:password1234");
            //_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            //var formContent = new FormUrlEncodedContent(new[]
            //{
            //    new KeyValuePair<string, string>("userName", userName),
            //    new KeyValuePair<string, string>("password", password)
            //});
            //var response = await _httpClient.PostAsync(url, formContent);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent($"userName={userName}&password={password}", Encoding.UTF8,
                "application/x-www-form-urlencoded" /*MediaTypeNames.Text.Plain*/);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{password}")));

            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded; charset=UTF-8"));
            //_httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            //_httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            //_httpClient.DefaultRequestHeaders.Add("X-MicrosoftAjax", "Delta=true");
            //_httpClient.DefaultRequestHeaders.Add("Accept", "*/*");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var tokenString = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"TOKEN: {tokenString}");
            return tokenString;
        }

        public async Task<string> PostAsync(string url, string requestString, bool useToken, string token, bool useBasic)
        {
            //if (!_httpClient.DefaultRequestHeaders.Contains("Authorization") || string.IsNullOrWhiteSpace(token))
            //{
            //    MessageBox.Show("Not Authorized");
            //    return null;
            //}

            StringContent requestContent = new StringContent(requestString, Encoding.UTF8, "application/json");

            //var httpClient = new HttpClient();
            //httpClient.BaseAddress = new Uri(Url);
            //httpClient.DefaultRequestHeaders.Accept.Clear();
            //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //_httpClient.DefaultRequestHeaders.Add("apikey", apikey);
            //_httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            if (useToken && (!_httpClient.DefaultRequestHeaders.Contains("Authorization") || _httpClient.DefaultRequestHeaders.Authorization.Scheme != JwtBearerDefaults.AuthenticationScheme))
            {
                //_httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
                //_httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkbWl0cnlAb3JkZXJib3QuY29tIiwianRpIjoiZWM5NzA2OWYtN2IzOS00Njk0LWE2OTgtNDBjNDIxYmYwZGNiIiwib2JkYXRhIjoiYTdydk5ibzJJaXFIZHMvbzRYU1dYNlovL25OeWRDNk9KWjhlVWZINFd5Rjd1M2RoL1NxTWZRZXNsSWJJcldLSCIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6ImFkbWluaXN0cmF0b3IiLCJuYmYiOjE2MTk0ODI2NDUsImV4cCI6MTYyNzI1ODY0NSwiaXNzIjoiT3JkZXJib3RTb2Z0d2FyZSIsImF1ZCI6Ik9yZGVyYm90QXBpVXNlcnMifQ.xZiQk0dtitwbooPgld7Gv6knn5A3o9n_nqyqFY2jKnc");
            }

            if (useBasic)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_settings.UserName}:{_settings.Password}")));
            }

            //HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, Url);
            //message.Content = requestContent;
            //var response = await _httpClient.SendAsync(message);

            var response = await _httpClient.PostAsync(url, requestContent);
            string responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                //ResponseModel responseModel = JsonSerializer.Deserialize<ResponseModel>(responseString);
            }
            else
            {
                _logger.LogInformation($"Response: {response.ReasonPhrase}");
            }
            var threadId = Thread.CurrentThread.ManagedThreadId;

            _logger.LogInformation($"Response: {responseString}, THREAD: {threadId}");
            return responseString;
        }
    }
}
