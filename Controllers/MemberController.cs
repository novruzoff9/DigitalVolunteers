using EntityLayer.Concrete;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;

namespace Web_DigitalVolunteers.Controllers
{
    public class MemberController : Controller
    {
        UserManager UserM = new UserManager(new EfUserDAL());
        EventManager EventM = new EventManager(new EfEventDAL());
        NewsManager NewsM = new NewsManager(new EfNewsDal());
        EventGalleryManager EventGalleyM = new EventGalleryManager(new EfEventGalleryDAL());
        EventRegistrationManager EventRegistrationM = new EventRegistrationManager(new EfEventRegistrationDAL());
        ActivityPointManager PointM = new ActivityPointManager(new EfActivityPointDAL());
        DailyLoginManager LoginM = new DailyLoginManager(new EfDailyLoginDAL());
        // GET: Member
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Home()
        {
            ViewBag.Review = "false";
            int userid = (int)Session["UserID"];
            var lastregistration = EventRegistrationM.GetList().LastOrDefault(x => x.UserID == userid && x.Participated == true);
            var lastevent = EventM.GetByID(lastregistration.EventID);
            var user = UserM.GetByID(userid);
            if(user.LastOnline < lastevent.DateTime.AddHours(1))
            {
                ViewBag.Review = "true";
            }
            user.LastOnline = DateTime.Now;
            UserM.Update(user);
            return View();
        }

        public PartialViewResult EventReview()
        {
            int userid = (int)Session["UserID"];
            var lastregistration = EventRegistrationM.GetList().LastOrDefault(x => x.UserID == userid || x.Participated == true);
            return PartialView(lastregistration);
        }

        public JsonResult ApplyReview(int registrationid, int star, string comment)
        {
            var registration = EventRegistrationM.GetByID(registrationid);
            registration.Rating = star;
            registration.Review = comment;
            EventRegistrationM.Update(registration);
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Events()
        {
            int userid = (int)Session["UserID"];
            var events = EventM.GetList();
            var registrations = EventRegistrationM.GetList().Where(x=>x.UserID == userid).ToList();
            return View(registrations);
        }

        public ActionResult Reviews()
        {
            int userid = (int)Session["UserID"];
            var events = EventM.GetList();
            var registrations = EventRegistrationM.GetList().Where(x => x.UserID == userid).ToList();
            return View(registrations);
        }

        public JsonResult EventByID(int id)
        {
            var eventinfo = EventM.GetByID(id);
            eventinfo.Caption = System.Web.HttpUtility.HtmlDecode(eventinfo.Caption);
            return Json(eventinfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserProfile()
        {
            int sessionid = (int)Session["UserID"];
            var user = UserM.GetByID(sessionid);
            int currentsessionid = Convert.ToInt16(Session["UserID"]);
            if (!Session["UserID"].Equals(sessionid))
            {
                user = UserM.GetByID(currentsessionid);
            }

            return View(user);
        }

        public ActionResult QRCode()
        {
            string username = Session["UserName"].ToString();
            int userid = Convert.ToInt32(Session["UserID"]);
            var user = UserM.GetByID(userid);

            using(MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qrgen = new QRCodeGenerator();
                QRCodeGenerator.QRCode qrcode = qrgen.CreateQrCode(username, QRCodeGenerator.ECCLevel.H);
                using (Bitmap image = qrcode.GetGraphic(240))
                {
                    image.Save(ms, ImageFormat.Png);
                    ViewBag.qrcodeimg = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }

            return View(user);
        }

        public PartialViewResult ActivityPoints(int id)
        {
            var points = PointM.GetList().Where(x => x.UserID == id).ToList();
            return PartialView(points);
        }

        public ActionResult Settings()
        {
            var user = SessionUser();
            return View(user);
        }

        public JsonResult UpdatePassword(string password, string newpassword)
        {
            var user = SessionUser();
            if (password != user.Password)
            {
                return Json("Wrong password", JsonRequestBehavior.AllowGet);
            }
            else if (password == user.Password)
            {
                user.Password = newpassword;
                UserM.Update(user);
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            return Json("Salaaam", JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateImage(string password)
        {
            var user = SessionUser();
            HttpPostedFileBase file = Request.Files["imageFile"];

            string filename = Path.GetFileName(file.FileName);
            string address = "~/Images/ProfilePictures/" + user.UserID + "_" + filename;
            Request.Files[0].SaveAs(Server.MapPath(address));
            user.UserImage = "/Images/ProfilePictures/" + user.UserID + "_" + filename;

            UserM.Update(user);

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        private User SessionUser()
        {
            int userid = (int)Session["UserID"];
            var user = UserM.GetByID(userid);
            return user;
        }

    }
}