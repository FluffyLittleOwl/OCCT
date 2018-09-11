using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OCTT.Domain.Entities
{
    public class RSSFeed
    {
        [Key]
        public int FeedId { get; set; }

        public string Uri { get; set; }

        public string Name { get; set; }

        public virtual ICollection<RSSRecord> Records { get; set; }
    }
}
