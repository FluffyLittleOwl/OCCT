using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OCTT.Domain.Concrete;
using OCTT.Domain.Entities;
using OCTT.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;

namespace OCTT.RSSReader
{
    class Program
    {
        private static IConfiguration Configuration = null;

        static void Main(string[] args)
        {
            /// get context
            Configuration = (new ConfigurationBuilder()).BuildOCTT();
            DbContextOptionsBuilder<RSSDbContext> rssOptionsBuilder = new DbContextOptionsBuilder<RSSDbContext>();
            rssOptionsBuilder.UseSqlServer(Configuration["Data:ConnectionString"]);
            var dbContext = new RSSDbContext(rssOptionsBuilder.Options);
            
            // dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            
            /// update rss feed table
            
            var readRSSFeeds = Configuration.ReadRSSFeeds();
            RSSFeedRepository rssSourceRepository = new RSSFeedRepository(dbContext);
            var willBeAdded = rssSourceRepository.AddUniqueRange(readRSSFeeds);
            dbContext.SaveChanges();
            
            /// read it, save it, show it
            Console.WriteLine("Updating RSS feed storage...");
            var rssFeeds = rssSourceRepository.RSSFeeds.Include(p => p.Records).AsQueryable(); 
            RSSRecordRepository rssRecordRepository = new RSSRecordRepository(dbContext);
            foreach (var rssFeed in rssFeeds.ToList())
            {
                SyndicationFeed feed = GetFeed(rssFeed.Uri);
                if(feed == null)
                {
                    Console.WriteLine($"Reeding {rssFeed.Name} feed failed, moving on...  ");
                    continue;
                }
                var parsedFeed = ParseFeed(feed, rssFeed);
                var totalRecordsRead = parsedFeed.Count();
                var uniqueRecordsRead = rssRecordRepository.AddUniqueRange(parsedFeed, rssFeed);
                dbContext.SaveChanges();

                Console.WriteLine($"Reeding {rssFeed.Name} feed: total of {totalRecordsRead}, saved {uniqueRecordsRead}");
            }            
            Console.WriteLine("Update complete, press any key to exit");
            Console.ReadKey();
        }

        private static IQueryable<RSSRecord> ParseFeed(SyndicationFeed feed, RSSFeed rssFeed)
        {
            List<RSSRecord> parsedFeed = new List<RSSRecord>();
            foreach (SyndicationItem item in feed.Items)
            {
                var record = new RSSRecord
                {
                    Title = item.Title.Text,
                    PubDate = item.PublishDate.DateTime,
                    Summary = item.Summary?.Text,
                    Link = item.Links[0].Uri.AbsoluteUri,
                    Feed = rssFeed
                };
                parsedFeed.Add(record);
            }
            return parsedFeed.AsQueryable();
        }

        /// Интерфакс вставляет \n в первую строчку фида, поэтому просто SyndicationFeed feed = SyndicationFeed.Load(reader); не работает
        /// прообраз позаимствован отсюда https://stackoverflow.com/questions/4919280/troubles-wtih-comments-in-xmlserialzier
        private static SyndicationFeed GetFeed(String url)
        {
            /// this try block here in order to catch exceptions
            /// caused by wrong url and their's server side problems
            /// not big enough deal to make me use try{}catch{}finally{}
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                using (var response = request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    string responseString = reader.ReadToEnd().Trim();

                    var xmlReaderSettings = new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true };
                    using (XmlReader xmlReader = XmlReader.Create(new StringReader(responseString), xmlReaderSettings))
                    {
                        var feed = SyndicationFeed.Load(xmlReader);
                        return feed;
                    }
                }
            }
            catch (Exception ex) {
                return null;
            }
        }
    }
}
