using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OCTT.Domain.Entities
{
    public class RSSRecord
    {
        [Key]
        public int RecordId { get; set; }

        public string Title { get; set; }

        public DateTime PubDate { get; set; }

        public string Summary { get; set; }

        public string Link { get; set; }

        public int FeedId { get; set; }
        public virtual RSSFeed Feed { get; set; }
    }
}
