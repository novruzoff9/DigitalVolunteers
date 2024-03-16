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
using System.Management.Instrumentation;

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
        AnnounceManager AnnounceM = new AnnounceManager(new EfAnnounceDal());
        NotficiationManager NotficiationM = new NotficiationManager(new EfNotficiationDal());
        // GET: Member

        public ActionResult Index()
        {
            return View();
        }

        #region Home
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

        public PartialViewResult AnnouncesPW()
        {
            var announces = AnnounceM.GetList();
            announces = Enumerable.Reverse(announces).ToList();
            announces = announces.GetRange(0, 3);
            return PartialView(announces);
        }

        public PartialViewResult NotficiationsPW()
        {
            var notficiations = NotficiationM.NotficiationsofUser(SessionUser().UserID);
            if (notficiations.Count >= 3)
            {
                notficiations = notficiations.GetRange(notficiations.Count - 3, 3);
            }
            notficiations = Enumerable.Reverse(notficiations).ToList();
            return PartialView(notficiations);
        }

        public ActionResult Announces()
        {
            var announces = AnnounceM.GetList();
            announces = Enumerable.Reverse(announces).ToList();
            return View(announces);
        }
        public JsonResult AnnounceByID(int announceid)
        {
            var announce = AnnounceM.GetByID(announceid);
            if(announce.WriterID != 0)
            {
                var writer = UserM.GetByID(announce.WriterID);
                announce.Writer = writer;
            }
            announce.Text = announce.Text.Replace("\n", "<br />");
            return Json(announce, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Notficiations()
        {
            int userid = (int)Session["UserID"];
            var notficiations = NotficiationM.NotficiationsofUser(userid);
            notficiations = Enumerable.Reverse(notficiations).ToList();
            foreach (var item in notficiations)
            {
                item.Text = item.Text.Replace("\n", "<br>");
                item.Writer = UserM.GetByID(item.WriterID);
            }
            return View(notficiations);
        }
        public JsonResult NotficiationByID(int notficiationid)
        {
            var notficiation = NotficiationM.GetByID(notficiationid);
            if (notficiation.WriterID != 0)
            {
                var writer = UserM.GetByID(notficiation.WriterID);
                notficiation.Writer = writer;
            }
            notficiation.Text = notficiation.Text.Replace("\n", "<br />");
            return Json(notficiation, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Events
        public ActionResult Events()
        {
            int userid = (int)Session["UserID"];
            var events = EventM.GetList();
            var registrations = EventRegistrationM.GetList().Where(x => x.UserID == userid).ToList();
            registrations = Enumerable.Reverse(registrations).ToList();
            return View(registrations);
        }

        public JsonResult EventByID(int id)
        {
            var eventinfo = EventM.GetByID(id);
            eventinfo.Caption = eventinfo.Caption.Replace("\n", "<br />");
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
            var registrations = EventRegistrationM.GetList().Where(x => x.UserID == userid && x.Rating > 0).ToList();
            registrations = Enumerable.Reverse(registrations).ToList();
            return View(registrations);
        }
        #endregion

        #region Vacancies
        public ActionResult Vacancies()
        {
            var vacancies = VacancyM.GetList().Where(x => x.Primary == false && (x.Deadline > DateTime.Now ||
                x.Deadline.Year == 2005)).ToList();
            var user = SessionUser();
            List<Vacancy> selectedvacancies = new List<Vacancy>();
            foreach (var item in vacancies)
            {
                if (item.Department.StartsWith("FS") && item.Department.EndsWith(user.Faculty))
                {
                    selectedvacancies.Add(item);
                }
                else if(!item.Department.StartsWith("FS"))
                {
                    selectedvacancies.Add(item);
                }
            }
            return View(selectedvacancies);
        }

        public PartialViewResult PrimaryVacancies()
        {
            var vacancies = VacancyM.GetList().Where(x => x.Primary == true && (x.Deadline > DateTime.Now || 
                x.Deadline.Year == 2005)).ToList();
            var user = SessionUser();
            var selectedvacancies = vacancies;
            foreach (var item in vacancies)
            {
                if (item.Department.StartsWith("FS"))
                {
                    if (!item.Department.EndsWith(user.Faculty))
                    {
                        selectedvacancies.Remove(item);
                    }
                }
            }
            return PartialView(selectedvacancies);
        }

        public PartialViewResult AppliedVacancies()
        {
            int userid = (int)Session["UserID"];
            var vacancies = VacancyApplyM.AppliesOfUser(userid);

            return PartialView(vacancies);
        }

        public ActionResult VacancyDetails(int id)
        {
            var user = SessionUser();
            var vacancy = VacancyM.GetByID(id);
            var apply = user.VacancyApplies.FirstOrDefault(x => x.VacancyID == id);
            vacancy.Description = vacancy.Description.Replace("\n", "<br />");
            ViewBag.applied = "false";
            ViewData["Interview"] = false;
            ViewData["Enter"] = false;
            ViewData["Note"] = null;
            if (apply != null)
            {
                ViewBag.applied = "true";
                ViewData["ApplyID"] = apply.ApplyID;
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
            var user = SessionUser();
            Vacancy vacancy = VacancyM.GetByID(vacancyid);
            if (user.VacancyApplies.FirstOrDefault(x => x.VacancyID == vacancyid) != null)
            {
                return Json("applied", JsonRequestBehavior.AllowGet);
            }
            VacancyApply apply = new VacancyApply();
            apply.VacancyID = vacancyid;
            apply.UserID = userid;
            apply.ApplierNote = note;
            apply.ApplyDateTime = DateTime.Now;
            apply.InterviewDateTime = DateTime.Now;
            apply.EnteringDateTime = DateTime.Now;
            VacancyApplyM.Add(apply);
            Notficiation notficiation = new Notficiation();
            notficiation.UserID = userid;
            notficiation.Title = "Vakansiya müraciəti.";
            notficiation.Text = vacancy.Title + " adlı vakansiyaya olan müraciətiniz uğurla qeydə alındı." +
                "\n 'Müraciətlərim' bölməsindən baxa bilərsiniz.";
            notficiation.WriterID = 2463;
            notficiation.WritingTime = DateTime.Now;
            NotficiationM.Add(notficiation);
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteApply(int applyid)
        {
            var apply = VacancyApplyM.GetByID(applyid);
            int userid = (int)Session["UserID"];
            if(apply.UserID == userid)
            {
                VacancyApplyM.Delete(apply);
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Certificates
        public ActionResult Certificates()
        {
            return View();
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
            var userparticipates = EventRegistrationM.GetList().Where(x => x.UserID == sessionid).ToList();
            ViewBag.comments = userparticipates.Where(x=>x.Rating > 0 && x.Participated == true).Count();
            ViewBag.noncomments = userparticipates.Where(x=>x.Rating == 0 && x.Participated == true).Count();
            ViewBag.nonparticipate = userparticipates.Where(x=>x.Participated == false).Count();
            return View(user);
        }

        public ActionResult QRCode()
        {
            string username = Session["UserName"].ToString();
            int userid = (int)Session["UserID"];
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
                Notficiation notficiation = new Notficiation();
                notficiation.UserID = SessionUser().UserID;
                notficiation.Title = "Profil məlumatlarında yenilik.";
                notficiation.Text = "Şifrəniz yeniləndi.";
                notficiation.WriterID = 2463;
                notficiation.WritingTime = DateTime.Now;
                NotficiationM.Add(notficiation);
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            return Json("Salaaam", JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateImage()
        {
            var user = SessionUser();
            HttpPostedFileBase file = Request.Files["imageFile"];

            string filename = Path.GetFileName(file.FileName);
            string address = "~/Images/ProfilePictures/" + user.UserID + "_" + filename;
            Request.Files[0].SaveAs(Server.MapPath(address));
            user.UserImage = "/Images/ProfilePictures/" + user.UserID + "_" + filename;

            UserM.Update(user);
            Notficiation notficiation = new Notficiation();
            notficiation.UserID = SessionUser().UserID;
            notficiation.Title = "Profil məlumatlarında yenilik.";
            notficiation.Text = "Profil şəkliniz yeniləndi.";
            notficiation.WriterID = 0;
            notficiation.WritingTime = DateTime.Now;
            NotficiationM.Add(notficiation);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult Feedbacks()
        {
            return View();
        }

        public JsonResult SendFeedback(string text)
        {
            Notficiation feedback = new Notficiation();
            feedback.Title = "Geri Donus";
            feedback.Text = text;
            feedback.WriterID = SessionUser().UserID;
            feedback.UserID = 2463;
            feedback.Seen = false;
            feedback.WritingTime = DateTime.Now;
            NotficiationM.Add(feedback);
            return Json("success", JsonRequestBehavior.AllowGet);
        }

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