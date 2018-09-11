using OCTT.Domain.Entities;
using System.Linq;

namespace OCTT.Domain.Abstract
{
    public interface IRSSRecordRepository
    {
        IQueryable<RSSRecord> RSSRecords { get; }
    }
}
