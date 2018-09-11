using OCTT.Domain.Entities;

namespace OCTT.Web.Helpers
{
    public static class DataTableEntitiesConverter
    {
        /// also see http://stackoverflow.com/questions/13944816/convert-a-class-into-an-array for the alternative solution
        public static string[] ToStringArray(this RSSFeed feed)
        {
            return new string[] {
                feed.FeedId.ToString(),
                feed.Name.ToString(),
                feed.Uri.ToString()
            };
        }

        /// технически плохое(или не самое лучшее) долгосрочное решение тк важен порядок, в котором передаются поля в таблицу, 
        public static string[] ToStringArray(this RSSRecord record)
        {
            return new string[] {
                record.RecordId.ToString(),
                record.Feed.Name.ToString(),
                record.Title.ToString(),
                record.Summary?.ToString(),
                record.PubDate.ToString(),
            };
        }
    }
}
