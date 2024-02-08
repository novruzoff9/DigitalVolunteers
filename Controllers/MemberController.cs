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
using System.Globalization;

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
        VacancyManager VacancyM = new VacancyManager(new EfVacancyDAL());
        VacancyApplyManager VacancyApplyM = new VacancyApplyManager(new EfVacancyApplyDAL());
        // GET: Member

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Home()
        {
            ViewBag.Review = "false";
            int userid = (int)Session["UserID"];
            var lastevent = EventM.GetList().Last();
            var eventregs = EventRegistrationM.ParticipiantsOfEvent(lastevent.EventID);
            var lastreg = eventregs.FirstOrDefault(x => x.UserID == userid && x.Participated == true);
            var user = UserM.GetByID(userid);
            if(lastreg != null)
            {
                if (user.LastOnline < lastevent.DateTime.AddHours(1))
                {
                    ViewBag.Review = "true";
                    ViewBag.Reviewid = lastreg.RegistrationID;
                }
            }
            user.LastOnline = DateTime.Now;
            UserM.Update(user);
            return View(user);
        }

        #region Events
        public ActionResult Events()
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
        #endregion

        #region Review
        public PartialViewResult EventReview(int id)
        {
            var reg = EventRegistrationM.GetByID(id);
            return PartialView(reg);
        }

        public JsonResult ApplyReview(int registrationid, int star, string comment)
        {
            var registration = EventRegistrationM.GetByID(registrationid);
            registration.Rating = star;
            registration.Review = comment;
            EventRegistrationM.Update(registration);
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reviews()
        {
            int userid = (int)Session["UserID"];
            var events = EventM.GetList();
            var registrations = EventRegistrationM.GetList().Where(x => x.UserID == userid).ToList();
            return View(registrations);
        }
        #endregion

        #region Vacancies
        public ActionResult Vacancies()
        {
            var vacancies = VacancyM.GetList().Where(x => x.Primary == false).ToList();
            return View(vacancies);
        }

        public PartialViewResult PrimaryVacancies()
        {
            var vacancies = VacancyM.GetList().Where(x => x.Primary == true).ToList();
            return PartialView(vacancies);
        }

        public PartialViewResult AppliedVacancies()
        {
            int userid = (int)Session["UserID"];
            var vacancies = VacancyApplyM.AppliesOfUser(userid);

            return PartialView(vacancies);
        }

        public ActionResult VacancyDetails(int id)
        {
            var vacancy = VacancyM.GetByID(id);
            int userid = (int)Session["UserID"];
            var applies = VacancyApplyM.AppliesOfUser(userid);
            var apply = applies.FirstOrDefault(x => x.VacancyID == id);
            ViewBag.applied = "false";
            ViewData["Interview"] = false;
            ViewData["Enter"] = false;
            ViewData["Note"] = null;
            if (apply != null)
            {
                ViewBag.applied = "true";
                ViewData["ApplyDate"] = apply.ApplyDateTime.ToString("dd MMM yyyy");
                ViewData["Interview"] = apply.Interview;
                ViewData["InterviewDate"] = apply.InterviewDateTime.ToString("dd MMM yyyy");
                ViewData["Enter"] = apply.Entered;
                ViewData["EnterDate"] = apply.EnteringDateTime.ToString("dd MMM yyyy");
                ViewData["Note"] = apply.Note;
            }
            return View(vacancy);
        }

        public JsonResult ApplytoVacancy(int vacancyid, int userid, string note)
        {
            VacancyApply apply = new VacancyApply();
            apply.VacancyID = vacancyid;
            apply.UserID = userid;
            apply.ApplierNote = note;
            apply.ApplyDateTime = DateTime.Now;
            apply.InterviewDateTime = DateTime.Now;
            apply.EnteringDateTime = DateTime.Now;
            VacancyApplyM.Add(apply);
            return Json("succes", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Profile
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
        #endregion

        #region Settings

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
        #endregion

        private User SessionUser()
        {
            int userid = (int)Session["UserID"];
            var user = UserM.GetByID(userid);
            return user;
        }

        static void DefaultCulture()
        {
            // Specify the culture for Azerbaijan
            string defaultCultureName = "az-AZ";

            // Create a CultureInfo object for the specified culture
            CultureInfo defaultCulture = CultureInfo.CreateSpecificCulture(defaultCultureName);

            // Set the default culture for the application
            CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
            CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            User user = new User();
            try
            {
                user = SessionUser();
            }
            catch (Exception)
            {
                RedirectToAction("UserLogin", "Login");
            }
            if (user.Role != "Member")
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "NoPermission",
                }));
            }
            DefaultCulture();
            base.OnActionExecuting(filterContext);
        }

    }
}