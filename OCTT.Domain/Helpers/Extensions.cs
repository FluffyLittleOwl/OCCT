using Microsoft.Extensions.Configuration;
using OCTT.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OCTT.Domain.Helpers
{
    public static class Extensions
    {
        public static IConfiguration BuildOCTT(this ConfigurationBuilder builder, string basePath = null, string filePath = "appsettings.json")
        {
            if (basePath == null) basePath = System.IO.Directory.GetCurrentDirectory().ToString();
            builder
                .SetBasePath(basePath)     // Microsoft.Extensions.Configuration.FileExtensions
                .AddEnvironmentVariables() // Microsoft.Extensions.Configuration.EnvironmentVariables
                .AddJsonFile(filePath);    // Microsoft.Extensions.Configuration.Json
            return builder.Build();
        }

        public static IQueryable<RSSFeed> ReadRSSFeeds(this IConfiguration configuration)
        {
            var feeds = new List<RSSFeed>();
            var feedsSection = configuration.GetSection("Feeds");
            
            foreach (IConfigurationSection section in feedsSection.GetChildren())
            {
                var feed = new RSSFeed
                {
                    Name = section.GetValue<string>("Name"),
                    Uri = section.GetValue<string>("Uri")
                };
                feeds.Add(feed);
            }

            return feeds.AsQueryable<RSSFeed>();
        }
    }
}
