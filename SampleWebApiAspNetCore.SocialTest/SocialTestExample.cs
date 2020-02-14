using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SampleWebApiAspNetCore.Entities;
using SampleWebApiAspNetCore.Repositories;
using System;
using System.Net;
using System.Net.Http;
using Xunit;

namespace SampleWebApiAspNetCore.SocialTest
{
    public class SocialTestExample
    {
        public static readonly string APP_DIRECTORY_PATH = "SampleWebApiAspNetCore";

        [Fact]
        public void TestExample()
        {
            var dbOptions = new DbContextOptionsBuilder<FoodDbContext>()
                .UseInMemoryDatabase(databaseName: "MyTestDatabase")
                .Options;

            using (var context = new FoodDbContext(dbOptions))
            {
                context.FoodItems.Add(new FoodEntity() { Id = 999, Calories = 1300, Type = "Starter", Name = "Pizza", Created = DateTime.Now });
                context.SaveChanges();

                var host = new WebHostBuilder()
                    .UseContentRoot(GetApplicationRoot())
                    .UseStartup<StartupFake>()
                    .ConfigureServices(services =>
                         {
                             services.AddSingleton(context);
                         });

                using (var server = new TestServer(host))
                {
                    var httpClient = server.CreateClient();

                    var response = httpClient.GetAsync("/api/v1/foods/999").Result;

                    response.EnsureSuccessStatusCode();
                }
            }
        }


        private static string GetApplicationRoot()
        {
            string contentRoot = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

            contentRoot = contentRoot.Substring(0, contentRoot.IndexOf("SampleWebApiAspNetCore.SocialTest"));

            return $"{contentRoot}{APP_DIRECTORY_PATH}";
        }
    }
}
