using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PostmanApi.Models;

namespace PostmanApi.Data
{
    public static class DataSeeder
    {
        public static void SeedData(IApplicationBuilder builder)
        {
            using (IServiceScope scope = builder.ApplicationServices.CreateScope())
            {
                DataContext context = scope.ServiceProvider.GetService<DataContext>();
                //bool deleted = context.Database.EnsureDeleted();
                //bool created = context.Database.EnsureCreated();
                if (context.Database.GetPendingMigrations().Any())
                {
                    //context.Database.Migrate();
                }

                if (!context.Contacts.Any())
                {
                    context.Contacts.AddRange(
                        new Contact
                        {
                            Name = "James Bond",
                            Phone = "604-555-5555",
                            Email = "007@mi6.gov.uk",
                            Age = 40,
                            Birthday = DateTime.Today.AddYears(-40),
                            IsAdult = true,
                            Gender = Gender.Male,
                            Position = Position.Spy
                        },
                        new Contact
                        {
                            Name = "Jason Bourne",
                            Phone = "604-555-5555",
                            Email = "bourne@threadstone.com",
                            Age = 30,
                            Birthday = DateTime.Today.AddYears(-30),
                            IsAdult = true,
                            Gender = Gender.Male,
                            Position = Position.Soldier
                        });
                    context.SaveChanges();
                }
            }
        }

        public static void Seed(string jsonData,
            IServiceProvider serviceProvider)
        {
            var settings = new JsonSerializerSettings
            {
                //ContractResolver = new PrivateSetterContractResolver()
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateFormatString = "yyyy-MM-dd HH:mm:ss",
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var contacts =
                JsonConvert.DeserializeObject<List<Contact>>(
                    jsonData, settings);
            using (var serviceScope = serviceProvider
                    .GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope
                    .ServiceProvider.GetService<DataContext>();
                if (!context.Contacts.Any())
                {
                    context.AddRange(contacts);
                    context.SaveChanges();
                }
            }
        }
    }
}
