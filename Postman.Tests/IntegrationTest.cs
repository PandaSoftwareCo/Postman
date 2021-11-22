using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Postman.Tests
{
    public class IntegrationTest : IDisposable
    {
        public IConfigurationRoot Configuration { get; }
        public TestServer TestServer { get; }
        public HttpClient Client { get; }

        public IntegrationTest()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
                //.AddUserSecrets<App>();
            Configuration = builder.Build();

            var webHostBuilder = new WebHostBuilder()
                .UseConfiguration(Configuration)
                //.UseContentRoot(Directory.GetCurrentDirectory())
                //.UseContentRoot(CalculateRelativeContentRootPath())
                .UseEnvironment("Development")
                .UseStartup<PostmanApi.Startup>();

            TestServer = new TestServer(webHostBuilder);
            Client = TestServer.CreateClient();

            //string CalculateRelativeContentRootPath() =>
            //    Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
            //        @"..\..\..\..\PostmanApi");
        }

        public void Dispose()
        {

        }

        [Fact]
        public async Task Test1()
        {
            HttpResponseMessage response = await Client.GetAsync("/api/Values");
            response.EnsureSuccessStatusCode();

            string responseHtml = await response.Content.ReadAsStringAsync();

            Assert.Contains("value1", responseHtml);
        }
    }
}
