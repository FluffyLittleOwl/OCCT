using OCTT.Domain.Entities;
using System.Collections.Generic;

namespace OCTT.Web.Models
{
    public class ViewPostMethodModel
    {
        public Dictionary<int, string> Feeds { get; set; }

        public PageControl PageControl { get; set; }

        public IEnumerable<RSSRecord> Records { get; set; }
    }
}
