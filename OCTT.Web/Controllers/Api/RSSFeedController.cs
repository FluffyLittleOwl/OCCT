using Microsoft.AspNetCore.Mvc;
using OCTT.Domain.Abstract;
using System.Linq;
using System.Threading.Tasks;

namespace OCTT.Web.Controllers.Api
{
    [Route("api/[controller]")]
    public class RSSFeedController : Controller
    {
        private readonly IRSSFeedRepository _repository;

        public RSSFeedController(IRSSFeedRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [HttpPost]
        public async Task<JsonResult> Get()
        {
            var rssFeeds = _repository.RSSFeeds.AsQueryable();
            var rssDictionary = rssFeeds.ToDictionary(f => f.FeedId, f => f.Name);
            return Json(new {
                data = rssDictionary
            });
        }
        
    }
}