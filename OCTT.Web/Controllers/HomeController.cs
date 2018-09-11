using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OCTT.Domain.Abstract;
using OCTT.Web.Models;
using System.Linq;

namespace ShopData.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRSSFeedRepository _feedRepository;

        private readonly IRSSRecordRepository _recordRepository;

        public HomeController(IRSSFeedRepository feedRepository, IRSSRecordRepository recordRepository)
        {
            _feedRepository = feedRepository;
            _recordRepository = recordRepository;
        }

        [Route("")]
        [Route("Home")]
        [Route("Home/Index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("ViewPostMethod")]
        public IActionResult ViewPostMethod(int feed = 0, string column = "PubDate", int page = 1)
        {
            var rssFeeds = _feedRepository.RSSFeeds.AsQueryable();
            var rssDictionary = rssFeeds.ToDictionary(f => f.FeedId, f => f.Name);

            var records = _recordRepository.RSSRecords.Include(p => p.Feed).AsQueryable();
            if (feed != 0) {
                records = records.Where(r => r.Feed.FeedId == feed);
            }
            switch (column) {
                case "PubDate": records = records.OrderByDescending(p => p.PubDate); break;
                case "Source": records = records.OrderByDescending(p => p.Feed.Name).ThenByDescending(p => p.PubDate); break; 
                default: break;
            }

            PageControl pageControl = new PageControl {
                RecordsTotal = records.Count(),
                CurrentPage = page,
            };
            records = records.Skip((pageControl.CurrentPage - 1) * pageControl.RecordsPerPage).Take(pageControl.RecordsPerPage);

            var model = new ViewPostMethodModel {
                Feeds = rssDictionary,
                PageControl = pageControl,
                Records = records.ToList()
            };
            return View(model);
        }

        [Route("ViewAjaxMethod")]
        public IActionResult ViewAjaxMethod()
        {
            return View();
        }
    }
}
