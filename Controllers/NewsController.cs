using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Web_DigitalVolunteers.Controllers
{
    public class NewsController : Controller
    {
        // GET: News


        NewsManager NewsM = new NewsManager(new EfNewsDal());
        NewsReadingManager ReadingM = new NewsReadingManager(new EfNewsReadingsDAL());
        public ActionResult Index(int page = 1)
        {
            var allnews = Enumerable.Reverse(NewsM.GetList().OrderBy(x => x.NewsCreated));
            var news = allnews.ToPagedList(page, 10);
            return View(news);
        }

        public PartialViewResult Slider()
        {
            var news = NewsM.GetList().OrderBy(x => x.Reading);
            var allnews = Enumerable.Reverse(news).ToList();
            allnews = allnews.GetRange(0, 5).ToList();
            return PartialView(allnews);
        }

        public PartialViewResult TrendNews()
        {
            var news = Enumerable.Reverse(NewsM.GetList().OrderBy(x => x.Reading)).ToList();
            var allnews = news.Where(x => x.NewsCreated.AddDays(31) > DateTime.Now).ToList();
            if (allnews.Count > 5)
            {
                allnews = allnews.GetRange(0, 5).ToList();
            }
            return PartialView(allnews);
        }

        public ActionResult NewsDetails(int id) {
            var newsdetails = NewsM.GetByID(id);
            newsdetails.Reading += 1;
            NewsM.Update(newsdetails);
            newsdetails.NewsCaption = newsdetails.NewsCaption.Replace("\n", "<br />");
            int linkstarts = newsdetails.NewsCaption.IndexOf("http");
            if (linkstarts != -1)
            {
                string link = newsdetails.NewsCaption.Substring(linkstarts);
                newsdetails.NewsCaption = newsdetails.NewsCaption.Insert(linkstarts + link.Length, "</a>");
                newsdetails.NewsCaption = newsdetails.NewsCaption.Insert(linkstarts, "<a href=" + link + ">");
            }
            return View(newsdetails);
        }

        public PartialViewResult Suggestions()
        {
            var news = Enumerable.Reverse(NewsM.GetList().OrderBy(x=>x.Reading)).ToList().GetRange(0,5).ToList();

            return PartialView(news);
        }
    }
}