using Microsoft.EntityFrameworkCore;
using OCTT.Domain.Abstract;
using OCTT.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OCTT.Domain.Concrete
{
    public class RSSFeedRepository : IRSSFeedRepository
    {
        private RSSDbContext _context;

        public RSSFeedRepository(RSSDbContext context)
        {
            _context = context;
        }

        public IQueryable<RSSFeed> RSSFeeds
        {
            get
            {
                return _context.RSSFeeds;
            }
        }

        public int AddUniqueRange(IQueryable<RSSFeed> feeds)
        {
            int willBeAdded = 0;
            foreach (var feed in feeds)
            {
                bool isPresent = _context.RSSFeeds.AnyAsync(r => r.Name == feed.Name && r.Uri == feed.Uri).Result;
                if (isPresent == false)
                {
                    feed.Records = new List<RSSRecord>();
                    _context.RSSFeeds.Add(feed);
                    willBeAdded++;
                }
            }
            return willBeAdded;
        }
    }
}
