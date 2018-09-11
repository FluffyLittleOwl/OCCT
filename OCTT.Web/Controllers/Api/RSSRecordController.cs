using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OCTT.Domain.Abstract;
using OCTT.Web.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace OCTT.Web.Controllers.Api
{
    [Route("api/[controller]")]
    public class RSSRecordController : Controller
    {
        private readonly IRSSRecordRepository _repository;

        public RSSRecordController(IRSSRecordRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [HttpPost]
        public async Task<JsonResult> Get(int draw = 0, int start = 0, int length = 10, int feed = 0, string column = "PubDate")
        {
            var records = _repository.RSSRecords.Include(p => p.Feed).AsQueryable();
            if(feed != 0) {
                records = records.Where(r => r.Feed.FeedId == feed); 
            }
            switch (column) {
                case "PubDate": records = records.OrderByDescending(p => p.PubDate); break;
                case "Source": records = records.OrderByDescending(p => p.Feed.Name).ThenByDescending(p => p.PubDate); break; 
                default: break;
            }
            
            var recordsTotal = records.Count();
            records = records.Skip(start).Take(length);
            var recordsTaken = records.Count();

            /// TODO: make it right
            return Json(new
            {
                draw = draw,
                recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = records.ToList().Select(p => p.ToStringArray()).ToArray()
            });
        }
    }
}