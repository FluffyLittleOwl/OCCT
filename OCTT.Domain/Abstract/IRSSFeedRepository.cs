using OCTT.Domain.Entities;
using System.Linq;

namespace OCTT.Domain.Abstract
{
    public interface IRSSFeedRepository
    {
        IQueryable<RSSFeed> RSSFeeds { get; }
    }
}
