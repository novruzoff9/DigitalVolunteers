using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web_DigitalVolunteers.Controllers
{
    public class EventsController : Controller
    {
        // GET: Events

        //GORULECEK ISLERRR!!!
        //Novbeti tedbir ucun bir basa uzv qeydiyyati
        //Tedbirlere Qalareya artirmag

        EventManager EventM = new EventManager(new EfEventDAL());
        EventGalleryManager GalleryM = new EventGalleryManager(new EfEventGalleryDAL());
        EventRegistrationManager RegistrationM = new EventRegistrationManager(new EfEventRegistrationDAL());

        public ActionResult Index(int page = 1)
        {
            var allevents = Enumerable.Reverse(EventM.GetList().OrderBy(x=>x.DateTime));
            var events = allevents.ToPagedList(page, 8);
            return View(events);
        }
        public PartialViewResult BestEvent()
        {
            var bestevent = EventM.GetList().OrderBy(x => x.Participant).Last();
            var gallery = GalleryM.GetList().Where(x => x.EventID == bestevent.EventID).ToList();
            ViewBag.image = gallery.First().ImageURL;
            bestevent.Caption = bestevent.Caption.Replace("\n", "<br />");
            return PartialView(bestevent);
        }


        public PartialViewResult NextEvent()
        {
            var nextevent = EventM.GetList().FirstOrDefault(x=>x.DateTime > DateTime.Now);
            if (nextevent != null)
            {
                nextevent.Caption = nextevent.Caption.Replace("\n", "<br/>");
                return PartialView(nextevent);
            }
            else
            {
                return PartialView("Empty");
            }
        }

        public ActionResult Event(int id)
        {
            var eventdetails = EventM.GetByID(id);
            eventdetails.Caption = eventdetails.Caption.Replace("\n", "<br/>");
            return View(eventdetails);
        }

        public PartialViewResult EventGallery(int id)
        {
            var gallery = GalleryM.GetList().Where(x => x.EventID == id).ToList();
            return PartialView(gallery);
        }

        [HttpPost]
        public JsonResult Registration(int eventid) {
            var currentevent = EventM.GetByID(eventid);
            EventRegistration registration = new EventRegistration();
            registration.EventID = eventid;
            registration.UserID = (int)Session["UserID"];
            registration.RegistrationTime = DateTime.Now;
            var registrations = RegistrationM.ParticipiantsOfEvent(eventid);
            if(currentevent.Deadline < DateTime.Now)
            {
                return Json("Registration Closed", JsonRequestBehavior.AllowGet);
            }
            if(registrations.Count >= currentevent.ParticipantLimit)
            {
                return Json("Registration Closed", JsonRequestBehavior.AllowGet);
            }
            foreach (var item in registrations)
            {
                if(item.UserID == registration.UserID)
                {
                    return Json("Alredy registrated", JsonRequestBehavior.AllowGet);
                }
            }
            RegistrationM.Add(registration);
            return Json(registration, JsonRequestBehavior.AllowGet);
        }
    }
}