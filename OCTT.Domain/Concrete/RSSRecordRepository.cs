using Microsoft.EntityFrameworkCore;
using OCTT.Domain.Abstract;
using OCTT.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OCTT.Domain.Concrete
{
    public class RSSRecordRepository : IRSSRecordRepository
    {
        private RSSDbContext _context;

        public RSSRecordRepository(RSSDbContext context)
        {
            _context = context;
        }

        public IQueryable<RSSRecord> RSSRecords
        {
            get
            {
                return _context.RSSRecords;
            }
        }

        public int AddUniqueRange(IQueryable<RSSRecord> records, RSSFeed rssFeed = null)
        {
            int willBeAdded = 0;
            foreach (var record in records)
            {
                bool isPresent = _context.RSSRecords.AnyAsync(r => r.Title == record.Title && r.PubDate == record.PubDate).Result;
                if (isPresent == false)
                {
                    if(rssFeed != null)
                    {
                        rssFeed.Records.Add(record);
                        _context.RSSRecords.Add(record);
                    }
                    willBeAdded++;
                }
            }
            
            return willBeAdded;
        }
    }
}
