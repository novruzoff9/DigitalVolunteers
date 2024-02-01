using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web_DigitalVolunteers.Controllers
{
    public class HomeController : Controller
    {
        EventManager EventM = new EventManager(new EfEventDAL());
        UserManager UserM = new UserManager(new EfUserDAL());
        NewsManager NewsM = new NewsManager(new EfNewsDal());
        EventGalleryManager GalleryM = new EventGalleryManager(new EfEventGalleryDAL());


        public ActionResult Index()
        {
            ViewBag.UserCount = UserM.GetList().Count();
            ViewBag.EventCount = EventM.GetList().Count();
            ViewBag.StaffCount = UserM.GetList().Where(x=>x.DepartmentStaff == true || x.FacultyStaff == true).Count();
            return View();
        }

        public PartialViewResult News()
        {
            var news = NewsM.GetList().OrderBy(x => x.NewsCreated).ToList();
            news = Enumerable.Reverse(news.OrderBy(x => x.Reading)).ToList();
            news = news.GetRange(0, 5);
            return PartialView(news);
        }

        public PartialViewResult Events()
        {
            var events = EventM.GetList().OrderBy(x => x.DateTime).ToList();
            events = Enumerable.Reverse(events.OrderBy(x=>x.Participant)).ToList();
            if(events.Where(x=>x.DateTime > DateTime.Now).Count() > 0)
            {
                var nextevents = events.Where(x => x.DateTime > DateTime.Now).ToList();
                if(nextevents.Count() > 4)
                {
                    nextevents = nextevents.GetRange(0, 4);
                }
                events = events.GetRange(0, 4 - nextevents.Count());
                events.InsertRange(0, nextevents);
            }
            events = events.GetRange(0, 4);
            return PartialView(events);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult NoPermission()
        {
            return View();
        }

    }
}