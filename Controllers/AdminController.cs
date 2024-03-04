using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using PagedList;
using PagedList.Mvc;
using System.Collections;
using EntityLayer.Objects;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Helpers;
using System.Net.Mail;
using System.Net;
using Microsoft.Ajax.Utilities;
using System.Web.WebPages;
using System.Globalization;
using System.Configuration;
using System.Data.SqlClient;
using System.Management.Instrumentation;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;

namespace DigitalVolunteers.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        UserManager UserM = new UserManager(new EfUserDAL());
        EventManager EventM = new EventManager(new EfEventDAL());
        NewsManager NewsM = new NewsManager(new EfNewsDal());
        EventGalleryManager EventGalleyM = new EventGalleryManager(new EfEventGalleryDAL());
        NewsGalleryManager NewsGalleryM = new NewsGalleryManager(new EfNewsGalleryDal());
        EventRegistrationManager EventRegistrationM = new EventRegistrationManager(new EfEventRegistrationDAL());
        ActivityPointManager PointM = new ActivityPointManager(new EfActivityPointDAL());
        DailyLoginManager LoginM = new DailyLoginManager(new EfDailyLoginDAL());
        VacancyManager VacancyM = new VacancyManager(new EfVacancyDAL());
        VacancyApplyManager VacancyApplyM = new VacancyApplyManager(new EfVacancyApplyDAL());
        AnnounceManager AnnounceM = new AnnounceManager(new EfAnnounceDal());
        NotficiationManager NotficiationM = new NotficiationManager(new EfNotficiationDal());

        // GET: Admin

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
            if (user.Role == "Member")
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

        public ActionResult Index()
        {
            return View();
        }

        #region Dashboard
        [Authorize]
        public ActionResult Dashboard()
        {
            //DefaultCulture();
            if (!CheckAdmin() && !CheckRector())
            {
                return RedirectToAction("NoPermission", "Home");
            }
            return View();
        }

        public PartialViewResult ActiveUsers()
        {
            var users = UserM.GetList();
            users = users.OrderBy(x => x.ActivityPoint).ToList();
            users = Enumerable.Reverse(users).ToList();
            users = users.GetRange(0, 5);
            return PartialView(users);
        }

        public PartialViewResult Statistics(string table)
        {
            DataChange data = new DataChange();
            DateTime today = DateTime.Today;
            if (table == "user")
            {
                data.Name = "İstifadəçi";
                data.NowValue = UserM.GetList().Count();
                int untillastmonth = UserM.GetList().Where(x => x.SignDate.Month != today.Month || x.SignDate.Year != today.Year).Count();
                double thismonths = UserM.GetList().Where(x => x.SignDate.Month == today.Month && x.SignDate.Year == today.Year).Count();
                double lastmonths = UserM.GetList().Where(x => x.SignDate.Month == today.Month - 1 && x.SignDate.Year == today.Year).Count();
                data.PreviusValue = untillastmonth;
                double difference = thismonths - lastmonths;
                double change = difference / lastmonths * 100;
                if (change.ToString().Length >= 6)
                {
                    data.Change = change.ToString().Remove(5);
                }
                else
                {
                    data.Change = change.ToString();
                }
            }
            else if (table == "news")
            {
                data.Name = "Xəbər";
                data.NowValue = NewsM.GetList().Count();
                int untillastmonth = NewsM.GetList().Where(x => x.NewsCreated.Month != today.Month || x.NewsCreated.Year != today.Year).Count();
                double thismonths = NewsM.GetList().Where(x => x.NewsCreated.Month == today.Month && x.NewsCreated.Year == today.Year).Count();
                double lastmonths = NewsM.GetList().Where(x => x.NewsCreated.Month == today.Month - 1 && x.NewsCreated.Year == today.Year).Count();
                data.PreviusValue = untillastmonth;
                double difference = thismonths - lastmonths;
                double change = difference / lastmonths * 100;
                if (change.ToString().Length >= 6)
                {
                    data.Change = change.ToString().Remove(5);
                }
                else
                {
                    data.Change = change.ToString();
                }
            }
            else if (table == "event")
            {
                data.Name = "Tədbir";
                data.NowValue = EventM.GetList().Count();
                int untillastmonth = EventM.GetList().Where(x => x.DateTime.Month != today.Month || x.DateTime.Year != today.Year).Count();
                double thismonths = EventM.GetList().Where(x => x.DateTime.Month == today.Month && x.DateTime.Year == today.Year).Count();
                double lastmonths = EventM.GetList().Where(x => x.DateTime.Month == today.Month - 1 && x.DateTime.Year == today.Year).Count();
                data.PreviusValue = untillastmonth;
                double difference = thismonths - lastmonths;
                double change = difference / lastmonths * 100;
                if (change.ToString().Length >= 6)
                {
                    data.Change = change.ToString().Remove(5);
                }
                else
                {
                    data.Change = change.ToString();
                }
            }
            return PartialView(data);
        }

        public JsonResult ChartLists(string list)
        {
            List<DataChange> datas = new List<DataChange>();
            if (list == "events")
            {
                var events = EventM.GetList();
                DataChange first = new DataChange();
                DataChange second = new DataChange();
                DataChange third = new DataChange();
                DataChange fourth = new DataChange();

                first.Name = "İçərişəhər";
                second.Name = "Gənclik";
                third.Name = "Nizami";
                fourth.Name = "Semaşko";

                first.NowValue = events.Where(x => x.Location == "First" || x.Location == "FirstHA" 
                || x.Location == "FirstBig" || x.Location == "FirstSmall").Count();
                second.NowValue = events.Where(x => x.Location == "SecondBig" || x.Location == "SecondSmall").Count();
                third.NowValue = events.Where(x => x.Location == "ThirdBig" || x.Location == "ThirdSmall").Count();
                fourth.NowValue = events.Where(x => x.Location == "Fourth").Count();

                datas.Add(first);
                datas.Add(second);
                datas.Add(third);
                datas.Add(fourth);
                return Json(datas, JsonRequestBehavior.AllowGet);
            }
            else if (list == "logins")
            {
                List<DataChange> logins = new List<DataChange>();
                for (int i = 9; i >= 0; i--)
                {
                    {
                        DataChange dataChange = new DataChange();
                        DateTime day = DateTime.Today.AddDays(-i);
                        dataChange.Name = day.ToString("dd MMM");
                        dataChange.NowValue = LoginM.GetList().Where(x => x.LoginDateTime == day).Count();
                        logins.Add(dataChange);
                    }
                }
                return Json(logins, JsonRequestBehavior.AllowGet);
            }
            else if (list == "news")
            {
                List<DataChange> newsreadings = new List<DataChange>();
                var news = Enumerable.Reverse(NewsM.GetList().OrderBy(x => x.Reading).ToList()).ToList();
                var top5news = news.GetRange(0, 5);
                foreach (var item in top5news)
                {
                    DataChange dataChange = new DataChange();
                    dataChange.Name = item.NewsTitle;
                    dataChange.NowValue = item.Reading;
                    newsreadings.Add(dataChange);
                }
                return Json(newsreadings, JsonRequestBehavior.AllowGet);
            }
            return Json(datas, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult LastActivities()
        {
            var points = PointM.GetList().Where(x => x.Verified == true).ToList();
            points = Enumerable.Reverse(points).ToList().GetRange(0, 5);
            return PartialView(points);

        }
        #endregion

        #region Users
        public ActionResult Users(int page = 1)
        {
            var users = UserM.GetList().ToPagedList(page, 10);
            return View(users);
        }

        [HttpGet]
        public ActionResult AddUser()
        {
            var sessionuser = SessionUser();
            bool havepermission = false;
            if (sessionuser.Department == "HR" && sessionuser.DepartmentStaff == true) {
                havepermission = true;
            }
            else if (CheckAdmin())
            {
                havepermission = true;
            }
            if (!havepermission)
            {
                return RedirectToAction("NoPermission", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddUser(User item)
        {
            var users = UserM.GetList();
            int newid;
            try
            {
                newid = users.Last().UserID + 1;
            }
            catch (Exception)
            {
                newid = 1;
            }

            //Istifadeci adinin hazirlanmasi
            string username = item.Surname.First() + "." + item.Name;
            username = username.ToLower();
            int quantity = users.Where(x => x.UserName.StartsWith(username) &&
                (x.UserName.Substring(username.Length).IsInt() || x.UserName.Length == username.Length)).ToList().Count();
            if (quantity > 0)
            {
                username = username + (quantity + 1).ToString();
            }
            username = username.Replace("ə", "e");
            username = username.Replace("ü", "u");
            username = username.Replace("ö", "o");
            username = username.Replace("ç", "c");
            username = username.Replace("ş", "s");
            username = username.Replace("ı", "i");
            item.UserName = username;

            //Sifrenin hazirlanmasi
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz123456789";

            Random random = new Random();
            char[] randomArray = new char[8];

            for (int i = 0; i < 8; i++)
            {
                randomArray[i] = chars[random.Next(chars.Length)];
            }
            item.Password = new string(randomArray);

            //Profil seklinin qoyulmasi
            string filename = Path.GetFileName(Request.Files[0].FileName);
            string address = "~/Images/ProfilePictures/" + newid + "_" + filename;
            Request.Files[0].SaveAs(Server.MapPath(address));
            item.UserImage = "/Images/ProfilePictures/" + newid + "_" + filename;
            if (filename.Length < 1)
            {
                item.UserImage = "/Images/ProfilePictures/defaultPP.jpg";
            }

            item.ActivityPoint = 0;
            item.SignDate = DateTime.Now;
            item.LastOnline = DateTime.Now;
            if (item.BirthDate.Date == DateTime.Parse("01/01/0001")) { item.BirthDate = new DateTime(2000, 01, 01); }


            //Mail gonderilmesi
            string viewPath = "~/Views/Login/RegistrationEmail.cshtml";
            string body = RenderViewToString(viewPath, item);
            string mailSubject = "Rəqəmsal könüllülər təşkilatına qeydiyyatınız tamamlandı!";
            string mailBody = "Rəqəmsal könüllülər təşkilatı platforması üçün profil məlumatlarınız:\n" +
                "Giriş üçün istifadəçi adınız: " + item.UserName + "\n" +
                "Giriş üçün parolunuz: " + item.Password + "\n" +
                "Sizi təşkilatımızda gördüyümüzə şad olduq.\n" +
                "Hörmətlə, Rəqəmsal Könüllülər Təşkilatı.";

            UserM.Add(item);

            SendMail(item.EMail, mailSubject, body);

            string macaddress = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only consider Ethernet network interfaces
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    nic.OperationalStatus == OperationalStatus.Up)
                {
                    macaddress = nic.GetPhysicalAddress().ToString();
                }
            }

            string tgmessage = $"İstifadəçilər cədvəlinə '{SessionUser().Name} {SessionUser().Surname}' tərəfindən " +
                $"'{item.Name} {item.Surname}' adlı istifadəçi artırıldı.\n" +
                $"İstifadəçi adı : {item.UserName} \n" +
                $"MAC address: {macaddress}";
            SendTgDatabaseMessage(tgmessage, "", "");

            return RedirectToAction("SelectedUsers");
        }
        [HttpGet]
        public ActionResult UpdateUser(int id)
        {
            var sessionuser = SessionUser();
            bool havepermission = false;
            if (sessionuser.Department == "HR" && sessionuser.DepartmentStaff == true)
            {
                havepermission = true;
            }
            else if (CheckAdmin())
            {
                havepermission = true;
            }
            if (!havepermission)
            {
                return RedirectToAction("NoPermission", "Home");
            }
            var user = UserM.GetByID(id);
            return View(user);
        }
        [HttpPost]
        public ActionResult UpdateUser(User item)
        {
            var pastuser = UserM.GetByID(item.UserID);
            if (item.UserImage != null)
            {
                string filename = Path.GetFileName(Request.Files[0].FileName);
                string address = "~/Images/ProfilePictures/" + pastuser.UserID + "_" + filename;
                Request.Files[0].SaveAs(Server.MapPath(address));
                pastuser.UserImage = "/Images/ProfilePictures/" + pastuser.UserID + "_" + filename;
            }
            pastuser.UserName = item.UserName;
            pastuser.Name = item.Name;
            pastuser.Surname = item.Surname;
            pastuser.BirthDate = item.BirthDate;

            pastuser.PhoneNumber = item.PhoneNumber;
            pastuser.EMail = item.EMail;
            pastuser.Group = item.Group;
            pastuser.EntranceYear = item.EntranceYear;


            pastuser.Faculty = item.Faculty;
            pastuser.Profession = item.Profession;
            pastuser.Gender = item.Gender;
            pastuser.DepartmentStaff = item.DepartmentStaff;
            pastuser.FacultyStaff = item.FacultyStaff;
            pastuser.Role = item.Role;
            pastuser.Department = item.Department;
            UserM.Update(pastuser);
            return RedirectToAction("SelectedUsers");
        }

        public ActionResult SelectedUsers(int page = 1, string search = "none" , string list = "all")
        {
            ViewBag.list = list;
            ViewBag.search = search;
            var users = UserM.GetList().ToPagedList(page, 20);

            if(search == "role")
            {
                users = UserM.GetList().Where(x=>x.Role == list).ToPagedList(page, 20);
            }
            else if (search == "faculty")
            {
                users = UserM.GetList().Where(x => x.Faculty == list).ToPagedList(page, 20);
            }
            else if (search == "department")
            {
                users = UserM.GetList().Where(x => x.Department == list).ToPagedList(page, 20);
            }
            else if(search == "username")
            {
                users = UserM.GetList().Where(x => x.UserName.Contains(list)).ToPagedList(page, 20);
            }
            return View(users);
        }

        public JsonResult UserByID(int id)
        {
            var user = UserM.GetByID(id);
            user.Password = user.BirthDate.ToString("dd MMMM yyyy");
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UsersByUsername(string name)
        {
            var users = UserM.GetList();
            users = users.Where(x => x.UserName.Contains(name)).ToList();
            if(users.Count > 10) { users = users.GetRange(0, 20); }
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddPoint(int userid, string text, int point)
        {
            ActivityPoints activityPoint = new ActivityPoints();
            activityPoint.Activity = text;
            activityPoint.Point = point;
            activityPoint.UserID = userid;
            activityPoint.Date = DateTime.Now;
            activityPoint.WriterID = (int)Session["UserID"];
            activityPoint.Verified = false;
            if (CheckAdmin() || CheckRector())
            {
                PointM.Add(activityPoint);
            }
            else
            {
                return Json("Sizin aktivlik balı yazmağa icazəniz yoxdur.", JsonRequestBehavior.AllowGet);
            }
            User user = UserM.GetByID(userid);
            string macaddress = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only consider Ethernet network interfaces
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    nic.OperationalStatus == OperationalStatus.Up)
                {
                    macaddress = nic.GetPhysicalAddress().ToString();
                }
            }

            string tgmessage = $"Aktivlik balı cədvəlinə '{SessionUser().Name} {SessionUser().Surname}' tərəfindən " +
                $"'{user.Name} {user.Surname}' adlı istifadəçinin balı {activityPoint.Point} vahid artırıldı.\n" +
                $"MAC address: {macaddress}";
            SendTgDatabaseMessage(tgmessage, "", "");
            return Json("Aktivlik balı uğurla artırıldı.");
        }

        [HttpPost]
        public JsonResult SendInform(int userid, string text)
        {
            Notficiation notficiation = new Notficiation();
            notficiation.UserID = userid;
            notficiation.Text = text;
            notficiation.Seen = false;
            notficiation.Title = "Fərdi Mesaj";
            notficiation.WriterID = SessionUser().UserID;
            notficiation.WritingTime = DateTime.Now;
            if (CheckAdmin() || CheckRector())
            {
                NotficiationM.Add(notficiation);
            }
            else
            {
                return Json("Sizin fərdi mesaj göndərməyə icazəniz yoxdur.", JsonRequestBehavior.AllowGet);
            }
            return Json("Fərdi mesaj uğurla göndərildi.");
        }

        #endregion

        #region UserProfile
        public ActionResult UserProfile()
        {
            int sessionid = (int)Session["UserID"];
            var user = UserM.GetByID(sessionid);
            ViewBag.congratulations = "false";
            if (user.BirthDate.DayOfYear == DateTime.Today.DayOfYear && 
                user.LastOnline.Date != DateTime.Today.Date)
            {
                ViewBag.congratulations = "true";
            }
            user.LastOnline = DateTime.Now;
            UserM.Update(user);
            return View(user);
        }

        public ActionResult Rectorship()
        {
            int sessionid = (int)Session["UserID"];
            var rector = UserM.GetByID(sessionid);
            //ViewBag.congratulations = "false";
            //if (user.BirthDate.Day == DateTime.Today.Day && user.BirthDate.Month == DateTime.Today.Month &&
            //    rector.LastOnline.Date != DateTime.Today.Date)
            //{
            //    ViewBag.congratulations = "true";
            //}
            rector.LastOnline = DateTime.Now;
            UserM.Update(rector);
            return View(rector);
        }

        public PartialViewResult UserActivityPoints(int id)
        {
            var points = PointM.GetList().Where(x => x.UserID == id && x.Verified == true).ToList();
            points = Enumerable.Reverse(points).ToList();
            return PartialView(points);
        }

        public PartialViewResult FacultyStaff(string faculty)
        {
            var staff = UserM.GetList().Where(x => x.Faculty == faculty && x.FacultyStaff == true).ToList();
            var chairman = staff.FirstOrDefault(x => x.Role == "Chairman");
            if (chairman == null)
            {
                chairman = UserM.GetByID(7);
            }
            staff.Remove(chairman);
            staff.Reverse();
            int vicechairmans = staff.Where(x => x.Role == "Vice-Chairman").Count();
            for (int i = 0; i < vicechairmans; i++)
            {
                var vicechairman = staff.FirstOrDefault(x => x.Role == "Vice-Chairman");
                staff.Remove(vicechairman);
                staff.Add(vicechairman);
            }
            staff.Add(chairman);
            staff.Reverse();
            return PartialView(staff);
        }

        public PartialViewResult DepartmentStaff(string department)
        {
            var staff = UserM.GetList().Where(x => x.Department == department && x.DepartmentStaff == true).ToList();
            var leader = staff.FirstOrDefault(x => x.Role == "Leader");
            staff.Remove(leader);
            staff.Reverse();
            int viceleaders = staff.Where(x => x.Role == "Vice-Leader").Count();
            for (int i = 0; i < viceleaders; i++)
            {
                var viceleader = staff.FirstOrDefault(x => x.Role == "Vice-Leader");
                staff.Remove(viceleader);
                staff.Add(viceleader);
            }
            if (!(leader == null)) { staff.Add(leader); }
            staff.Reverse();
            return PartialView(staff);
        }

        public ActionResult Settings()
        {
            var user = SessionUser();
            return View(user);
        }

        public JsonResult UpdatePassword(string password, string newpassword)
        {
            var user = SessionUser();
            if(password != user.Password)
            {
                return Json("Wrong password", JsonRequestBehavior.AllowGet);
            }
            else if(password == user.Password)
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

        //QRCode
        public ActionResult UserQRCode()
        {
            string username = Session["UserName"].ToString();
            int userid = Convert.ToInt32(Session["UserID"]);
            var user = UserM.GetByID(userid);

            using (MemoryStream ms = new MemoryStream())
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

        #endregion

        #region Staffs
        //Heyət üzvləri
        public ActionResult Staffs()
        {
            var users = UserM.GetList();
            return View(users);
        }

        public ActionResult StaffofFaculty(string faculty)
        {
            var users = UserM.GetList().Where(x => x.FacultyStaff == true).ToList();
            users = users.Where(x => x.Faculty == faculty).ToList();
            var chairman = users.FirstOrDefault(x => x.Role == "Chairman");
            var nihad = UserM.GetByID(7);
            users.Remove(chairman);
            users.Remove(nihad);
            users.Reverse();
            int vicechairmans = users.Where(x => x.Role == "Vice-Chairman").Count();
            for (int i = 0; i < vicechairmans; i++)
            {
                var vicechairman = users.FirstOrDefault(x => x.Role == "Vice-Chairman");
                users.Remove(vicechairman);
                users.Add(vicechairman);
            }
            if (chairman != null)
            {
                users.Add(chairman);
            }
            if (faculty == "Economy") { users.Add(nihad); }
            users.Reverse();
            return View(users);
        }

        public ActionResult StaffofDepartment(string department)
        {
            var users = UserM.GetList().Where(x => x.DepartmentStaff == true).ToList();
            users = users.Where(x => x.Department == department).ToList();
            var leader = users.FirstOrDefault(x => x.Role == "Leader");
            users.Remove(leader);
            users.Reverse();
            int viceleaders = users.Where(x => x.Role == "Vice-Leader").Count();
            for (int i = 0; i < viceleaders; i++)
            {
                var viceleader = users.FirstOrDefault(x => x.Role == "Vice-Leader");
                users.Remove(viceleader);
                users.Add(viceleader);
            }
            if (leader != null)
            {
                users.Add(leader);
            }
            users.Reverse();
            return View(users);
        }

        #endregion

        #region Events
        //Tədbirlər səhifəsi
        public ActionResult Events()
        {
            var events = EventM.GetList().OrderBy(x => x.DateTime).ToList();
            events = Enumerable.Reverse(events).ToList();
            return View(events);
        }

        public JsonResult EventByID(int id)
        {
            var eventinfo = EventM.GetByID(id);
            eventinfo.Caption = System.Web.HttpUtility.HtmlDecode(eventinfo.Caption);
            return Json(eventinfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EventGallery(int id)
        {
            var images = EventGalleyM.GetList().Where(x => x.EventID == id).ToList();
            ViewBag.EventID = id;
            return View(images);
        }

        public ActionResult EventParticipiants(int id)
        {
            ViewBag.EventName = EventM.GetByID(id).Title;
            var participiantsofevent = EventRegistrationM.ParticipiantsOfEvent(id);
            List<User> participiants = new List<User>();
            foreach (var item in participiantsofevent)
            {
                var participiant = UserM.GetByID(item.UserID);
                participiants.Add(participiant);
            }
            return View(participiants);
        }

        public ActionResult EventReviews(int id)
        {
            var registrations = EventRegistrationM.GetList().Where(x => x.EventID == id).ToList();
            return View(registrations);
        }
        public ActionResult QRScaner(int id)
        {
            ViewBag.EventId = id;
            var eventname = EventM.GetByID(id).Title;
            ViewBag.EventName = eventname;
            return View();
        }

        public JsonResult UserByUserName(string username, int eventid)
        {
            var user = UserM.GetList().FirstOrDefault(x => x.UserName == username);
            var participatings = EventRegistrationM.GetList().Where(x => x.UserID == user.UserID).ToList();
            user.Password = "None";
            ViewBag.registrationID = 0;
            if (participatings.FirstOrDefault(x => x.EventID == eventid) != null)
            {
                user.Password = "Yes";
                ViewBag.registrationID = participatings.FirstOrDefault(x => x.EventID == eventid).RegistrationID;
            }
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ConfirmParticipating(int registrationid)
        {
            var registration = EventRegistrationM.GetByID(registrationid);
            registration.Participated = true;
            EventRegistrationM.Update(registration);
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEvent()
        {
            var sessionuser = SessionUser();
            bool havepermission = false;
            if (sessionuser.Department == "SMM" && sessionuser.DepartmentStaff == true)
            {
                havepermission = true;
            }
            else if (CheckAdmin())
            {
                havepermission = true;
            }
            if (!havepermission)
            {
                return RedirectToAction("NoPermission", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddEvent(Event item)
        {
            var events = EventM.GetList();
            int newid;
            try
            {
                newid = events.Last().EventID + 1;
            }
            catch (Exception)
            {
                newid = 1;
            }
            string filename = Path.GetFileName(Request.Files[0].FileName);
            string address = "~/Images/EventCovers/" + newid + "_" + filename;
            Request.Files[0].SaveAs(Server.MapPath(address));
            item.Image = "/Images/EventCovers/" + newid + "_" + filename;
            EventM.Add(item);
            Announce eventannounce = new Announce();
            eventannounce.Title = item.Title + " adlı tədbir keçiriləcək";
            eventannounce.Text = item.Caption;
            eventannounce.WritingTime = DateTime.Now;
            eventannounce.WriterID = 2463; // System ID = 2463
            AnnounceM.Add(eventannounce);

            string macaddress = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only consider Ethernet network interfaces
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    nic.OperationalStatus == OperationalStatus.Up)
                {
                    macaddress = nic.GetPhysicalAddress().ToString();
                }
            }


            string tgmessage = $"Tədbirlər cədvəlinə '{SessionUser().Name} {SessionUser().Surname}' tərəfindən " +
                $"'{item.Title}' başlıqlı tədbir artırıldı.\n" +
                $"MAC address: {macaddress}";
            SendTgDatabaseMessage(tgmessage, "Tədbirə bax", $"https://www.digitalvolunteers.xyz/Events/Event/{newid}");

            return RedirectToAction("Events");
        }

        [HttpGet]
        public ActionResult UpdateEvent(int id)
        {
            var sessionuser = SessionUser();
            bool havepermission = false;
            if (sessionuser.Department == "SMM" && sessionuser.DepartmentStaff == true)
            {
                havepermission = true;
            }
            else if (CheckAdmin())
            {
                havepermission = true;
            }
            if (!havepermission)
            {
                return RedirectToAction("NoPermission", "Home");
            }
            var eventdetails = EventM.GetByID(id);
            return View(eventdetails);
        }

        [HttpPost]
        public ActionResult UpdateEvent(Event item)
        {
            var pastevent = EventM.GetByID(item.EventID);
            pastevent.Title = item.Title;
            pastevent.Caption = item.Caption;
            pastevent.DateTime = item.DateTime;
            if (item.Image != null)
            {
                string filename = Path.GetFileName(Request.Files[0].FileName);
                string address = "~/Images/EventCovers/" + pastevent.EventID + "_" + filename;
                Request.Files[0].SaveAs(Server.MapPath(address));
                pastevent.Image = "/Images/EventCovers/" + pastevent.EventID + "_" + filename;
            }
            pastevent.Location = item.Location;
            pastevent.Deadline = item.Deadline;
            pastevent.ParticipantLimit = item.ParticipantLimit;
            EventM.Update(pastevent);
            return RedirectToAction("Events");
        }

        [HttpPost]
        public ActionResult AddEventImage(IEnumerable<HttpPostedFileBase> files, int EventID)
        {
            var sessionuser = SessionUser();
            bool havepermission = false;
            if (sessionuser.Department == "SMM" && sessionuser.DepartmentStaff == true)
            {
                havepermission = true;
            }
            else if (CheckAdmin())
            {
                havepermission = true;
            }
            if (!havepermission)
            {
                return RedirectToAction("NoPermission", "Home");
            }
            var eventimagess = EventGalleyM.GetList().Where(x => x.EventID == EventID).ToList();
            int newid;
            try
            {
                newid = eventimagess.Last().ImageID + 1;
            }
            catch (Exception)
            {
                newid = 1;
            }
            if (files != null && files.Any())
            {
                foreach (var file in files)
                {

                    if (file != null && file.ContentLength > 0)
                    {
                        EventGallery item = new EventGallery();
                        string filename = Path.GetFileName(file.FileName);
                        string address = "~/Images/EventGallery/" + EventID + "_" + newid + "_" + filename;
                        file.SaveAs(Server.MapPath(address));
                        item.ImageURL = "/Images/EventGallery/" + EventID + "_" + newid + "_" + filename;
                        item.EventID = EventID;
                        EventGalleyM.Add(item);
                        newid += 1;
                    }
                }
            }
            return RedirectToAction("Events");
        }

        #endregion

        #region News
        //Xəbərlər səhifəsi
        public ActionResult News()
        {
            var news = NewsM.GetList().OrderBy(x => x.NewsCreated).ToList();
            news = Enumerable.Reverse(news).ToList();
            return View(news);
        }

        public JsonResult NewsByID(int id)
        {
            var newsinfo = NewsM.GetByID(id);
            newsinfo.NewsCaption = System.Web.HttpUtility.HtmlDecode(newsinfo.NewsCaption);
            return Json(newsinfo, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddNews()
        {
            var sessionuser = SessionUser();
            bool havepermission = false;
            if (sessionuser.Department == "SMM" && sessionuser.DepartmentStaff == true)
            {
                havepermission = true;
            }
            else if (CheckAdmin())
            {
                havepermission = true;
            }
            if (!havepermission)
            {
                return RedirectToAction("NoPermission", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddNews(News item)
        {
            var news = NewsM.GetList();
            int newid;
            try
            {
                newid = news.Last().NewsID + 1;
            }
            catch (Exception)
            {
                newid = 1;
            }
            item.NewsCreated = DateTime.Now;
            string filename = Path.GetFileName(Request.Files[0].FileName);
            string address = "~/Images/NewsCovers/" + newid + "_" + filename;
            Request.Files[0].SaveAs(Server.MapPath(address));
            item.NewsImage = "/Images/NewsCovers/" + newid + "_" + filename;
            NewsM.Add(item);

            string macaddress = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only consider Ethernet network interfaces
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    nic.OperationalStatus == OperationalStatus.Up)
                {
                    macaddress = nic.GetPhysicalAddress().ToString();
                }
            }

            string tgmessage = $"Xəbərlər cədvəlinə {SessionUser().Name} {SessionUser().Surname} tərəfindən " +
                $"{item.NewsTitle} başlıqlı xəbər artırıldı.\n" +
                $"MAC address: {macaddress}";
            SendTgDatabaseMessage(tgmessage, "Xəbərə bax", $"https://www.digitalvolunteers.xyz/News/NewsDetails/{newid}");

            return RedirectToAction("News");
        }

        [HttpGet]
        public ActionResult UpdateNews(int id)
        {
            var newsdetails = NewsM.GetByID(id);
            return View(newsdetails);
        }

        [HttpPost]
        public ActionResult UpdateNews(News item)
        {
            var sessionuser = SessionUser();
            bool havepermission = false;
            if (sessionuser.Department == "SMM" && sessionuser.DepartmentStaff == true)
            {
                havepermission = true;
            }
            else if (CheckAdmin())
            {
                havepermission = true;
            }
            if (!havepermission)
            {
                return RedirectToAction("NoPermission", "Home");
            }
            var pastnews = NewsM.GetByID(item.NewsID);
            if (item.NewsImage != null)
            {
                string filename = Path.GetFileName(Request.Files[0].FileName);
                string address = "~/Images/NewsCovers/" + pastnews.NewsID + "_" + filename;
                Request.Files[0].SaveAs(Server.MapPath(address));
                pastnews.NewsImage = "/Images/NewsCovers/" + pastnews.NewsID + "_" + filename;
            }
            pastnews.NewsTitle = item.NewsTitle;
            pastnews.NewsCaption = item.NewsCaption;
            NewsM.Update(pastnews);
            return RedirectToAction("News");
        }

        public ActionResult NewsGallery(int id)
        {
            var images = NewsGalleryM.GetList().Where(x => x.NewsID == id).ToList();
            ViewBag.NewsID = id;
            return View(images);
        }


        [HttpPost]
        public ActionResult AddNewsImage(IEnumerable<HttpPostedFileBase> files, int NewsID)
        {
            var sessionuser = SessionUser();
            bool havepermission = false;
            if (sessionuser.Department == "SMM" && sessionuser.DepartmentStaff == true)
            {
                havepermission = true;
            }
            else if (CheckAdmin())
            {
                havepermission = true;
            }
            if (!havepermission)
            {
                return RedirectToAction("NoPermission", "Home");
            }
            var newsimagess = NewsGalleryM.GetList().Where(x => x.NewsID == NewsID).ToList();
            int newid;
            try
            {
                newid = newsimagess.Last().ImageID + 1;
            }
            catch (Exception)
            {
                newid = 1;
            }
            if (files != null && files.Any())
            {
                foreach (var file in files)
                {

                    if (file != null && file.ContentLength > 0)
                    {
                        NewsGallery item = new NewsGallery();
                        string filename = Path.GetFileName(file.FileName);
                        string address = "~/Images/NewsGallery/" + NewsID + "_" + newid + "_" + filename;
                        file.SaveAs(Server.MapPath(address));
                        item.ImageURL = "/Images/NewsGallery/" + NewsID + "_" + newid + "_" + filename;
                        item.NewsID = NewsID;
                        NewsGalleryM.Add(item);
                        newid += 1;
                    }
                }
            }
            return RedirectToAction("News");
        }
        #endregion

        #region Logins
        public ActionResult DailyLogins(string date, string membertype)
        {
            ViewBag.date = date;
            if (string.IsNullOrEmpty(membertype))
            {
                membertype = "All";
                ViewBag.membertype = "All";
            }
            else { ViewBag.membertype = membertype; }
            var alllogins = LoginM.GetList();
            DateTime dateTime = new DateTime();
            ViewBag.today = DateTime.Today.Day + " " + DateTime.Today.ToString("MMMM", CultureInfo.CreateSpecificCulture("az")) 
                + " " + DateTime.Today.Year;
            string fullMonthName = DateTime.Today.ToString("MMMM", CultureInfo.CreateSpecificCulture("az"));
            ViewBag.ThisMonth = fullMonthName;
            if (string.IsNullOrEmpty(date))
            {
                dateTime = DateTime.Today;
                string datestr = dateTime.Year + "-" + dateTime.Month + "-" + dateTime.Day;
                ViewBag.date = datestr;
            }
            else
            {
                int year = date.Substring(0, 4).AsInt();
                int month = date.Substring(5, 2).AsInt();
                int day = date.Substring(8, 2).AsInt();
                dateTime = new DateTime(year, month, day);
            }
            var logins = alllogins.Where(x => x.LoginDateTime == dateTime).ToList();
            if(membertype == "Staff")
            {
                logins = logins.Where(x => x.User.Role != "Member").ToList();
            }
            if (membertype == "Member")
            {
                logins = logins.Where(x => x.User.Role == "Member").ToList();
            }

            ViewBag.daily = alllogins.Where(x => x.LoginDateTime == DateTime.Today).Count();

            int dayofweek = (int)DateTime.Today.DayOfWeek;
            int daysToSubtract = (int)dayofweek - (int)DayOfWeek.Sunday;

            if(daysToSubtract == 0) { daysToSubtract = 7; }
            DateTime beginningOfWeek = DateTime.Today.AddDays(-daysToSubtract + 1);
            ViewBag.thisweek = beginningOfWeek.Day + " " + beginningOfWeek.ToString("MMMM", CultureInfo.CreateSpecificCulture("az")) 
                + " " + beginningOfWeek.Year;
            ViewBag.weekly = alllogins.Where(x => x.LoginDateTime >= beginningOfWeek &&
                x.LoginDateTime <= beginningOfWeek.AddDays(6)).Count();

            string dayofmonth = DateTime.Today.Day.ToString();
            ViewBag.monthly = alllogins.Where(x => x.LoginDateTime.Year == DateTime.Today.Year &&
                x.LoginDateTime.Month == DateTime.Today.Month).Count();
            //ViewBag.weekly = alllogins.Where(x => x.LoginDateTime == DateTime.Now.da);
            return View(logins);
        }

        public JsonResult DailyLoginChart()
        {
            List<DataChange> logins = new List<DataChange>();
            var loginslist = LoginM.GetList();
            for (int i = 29; i >= 0; i--)
            {
                DataChange dataChange = new DataChange();
                DateTime day = DateTime.Today.AddDays(-i);
                dataChange.Name = day.ToString("dd MMM");
                foreach (var item in loginslist)
                {
                    if(item.LoginDateTime < day) { continue; }
                    if(item.LoginDateTime > day) { break; }
                    if(item.User.Role == "Member") { dataChange.NowValue++; }
                    else { dataChange.PreviusValue++; }
                }
                //dataChange.NowValue = loginslist.Where(x => x.LoginDateTime == day && x.User.Role == "Member").Count();
                //dataChange.PreviusValue = loginslist.Where(x => x.LoginDateTime == day && x.User.Role != "Member").Count();
                logins.Add(dataChange);
            }
            return Json(logins, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region ActivityPoints

        public ActionResult ActivityPoints(int page = 1)
        {
            var points = PointM.GetList().Where(x => x.Verified == true).ToList();
            ViewBag.all = points.Count();
            ViewBag.increment = points.Where(x => x.Point > 0).Count();
            ViewBag.decrement = points.Where(x => x.Point < 0).Count();
            var pagedpoints = Enumerable.Reverse(points).ToPagedList(page, 12);
            return View(pagedpoints);
        }

        //public PartialViewResult LastActivities()
        //{
        //    return PartialView();
        //    // ERROR
        //    // ERROR
        //    // ERROR
        //}

        public PartialViewResult TopActivityLists(string list)
        {
            var users = UserM.GetList().OrderBy(x => x.ActivityPoint).ToList();
            if (list == "actives")
            {
                users = users.GetRange(users.Count - 5, 5);
                users = Enumerable.Reverse(users).ToList();
            }
            else if (list == "passives")
            {
                users = users.GetRange(0, 5);
            }
            return PartialView(users);
        }

        public ActionResult VerifyActivityPoints()
        {
            var user = SessionUser();
            if (user.UserID != 3 && user.UserID != 1)
            {
                return RedirectToAction("NoPermission", "Home");
            }
            var points = PointM.GetList().Where(x => x.Verified == false).ToList();
            ViewBag.all = points.Count();
            ViewBag.increment = points.Where(x => x.Point > 0).Count();
            ViewBag.decrement = points.Where(x => x.Point < 0).Count();
            points = Enumerable.Reverse(points).ToList();
            return View(points);
        }
        public ActionResult VerifyActivityPoint(int id)
        {
            var point = PointM.GetByID(id);
            point.Verified = true;
            PointM.Update(point);
            var user = UserM.GetByID(point.UserID);
            user.ActivityPoint = user.ActivityPoint + point.Point;
            UserM.Update(user);
            return RedirectToAction("VerifyActivityPoints");
        }


        #endregion

        #region Vacancies
        public ActionResult Vacancies()
        {
            var user = SessionUser();
            bool havepermission = false;
            if(user.Role == "Vice-Chairman" || user.Role == "Vice-Leader" || user.Role == "HR")
            {
                havepermission = true;
            }
            else if (CheckAdmin())
            {
                havepermission = true;
            }
            if (!havepermission)
            {
                return RedirectToAction("NoPermssion", "Home");
            }
            var vacancies = VacancyM.GetList();
            vacancies = Enumerable.Reverse(vacancies).ToList();
            if (user.Role != "President" && user.Role != "Vice-President" && user.UserID != 1 && user.UserID != 7 && user.DepartmentStaff)
            {
                vacancies = vacancies.Where(x => x.Department == user.Department).ToList();
            }
            else if (user.Role != "President" && user.Role != "Vice-President" && user.UserID != 1 && user.UserID != 7 && user.FacultyStaff)
            {
                string fsfaculty = "FS-" + user.Faculty;
                vacancies = vacancies.Where(x => x.Department == fsfaculty).ToList();
            }
            return View(vacancies);
        }

        public ActionResult AppliesofVacancy(int id)
        {
            var applies = VacancyApplyM.AppliesOfVacancy(id);
            return View(applies);
        }

        public ActionResult ApplyDetails(int id)
        {
            var apply = VacancyApplyM.GetByID(id);
            return View(apply);
        }

        [HttpPost]
        public ActionResult UpdateApply(VacancyApply changedValues)
        {
            var apply = VacancyApplyM.GetByID(changedValues.ApplyID);
            int applyid = changedValues.ApplyID;
            if (changedValues.InterviewDateTime != null)
            {
                apply.InterviewDateTime = changedValues.InterviewDateTime;
            }

            if (changedValues.Interview)
            {
                apply.Interview = changedValues.Interview;
            }

            if (changedValues.Entered)
            {
                apply.Entered = true;
                apply.EnteringDateTime = DateTime.Now;
            }

            if (changedValues.Note != apply.Note)
            {
                apply.Note = changedValues.Note;
            }
            VacancyApplyM.Update(apply);

            Notficiation notficiation = new Notficiation();
            notficiation.UserID = apply.UserID;
            notficiation.Title = "Vakansiya müraciətində yenilik.";
            notficiation.Text = apply.Vacancy.Title + " adlı vakansiyaya olan müraciətinizdə yenilik var. " + 
                "\n 'Müraciətlərim' bölməsindən baxa bilərsiniz.";
            notficiation.WriterID = 2463;
            notficiation.WritingTime = DateTime.Now;
            NotficiationM.Add(notficiation);
            return RedirectToAction("ApplyDetails", "Admin", new { id = applyid });
        }

        [HttpGet]
        public ActionResult AddVacancy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddVacancy(Vacancy item)
        {
            if(item.Deadline.Date == DateTime.Parse("01/01/0001")) { item.Deadline = new DateTime(2005, 09, 23); }
            item.CreatedTime = DateTime.Now;
            VacancyM.Add(item);

            string macaddress = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only consider Ethernet network interfaces
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    nic.OperationalStatus == OperationalStatus.Up)
                {
                    macaddress = nic.GetPhysicalAddress().ToString();
                }
            }

            string tgmessage = $"Vakansiyalar cədvəlinə {SessionUser().Name} {SessionUser().Surname} tərəfindən " +
                $"{item.Title} başlıqlı vakansiya artırıldı.\n" +
                $"MAC address: {macaddress}";
            SendTgDatabaseMessage(tgmessage, "", "");

            return RedirectToAction("Vacancies", "Admin");
        }

        [HttpGet]
        public ActionResult UpdateVacancy(int id)
        {
            var vacancy = VacancyM.GetByID(id);
            return View(vacancy);
        }

        [HttpPost]
        public ActionResult UpdateVacancy(Vacancy item)
        {
            var pastvacancy = VacancyM.GetByID(item.VacancyID);
            pastvacancy.Title = item.Title;
            pastvacancy.Description = item.Description;
            pastvacancy.Deadline = item.Deadline;
            pastvacancy.Department = item.Department;
            pastvacancy.Primary = item.Primary;
            VacancyM.Update(pastvacancy);
            return RedirectToAction("Vacancies", "Admin");
        }

        #endregion

        #region Announce
        public ActionResult Announces()
        {
            var user = SessionUser();
            bool havepermission = false;
            if (user.Role == "Vice-Chairman" || user.Role == "Vice-Leader" || user.Role == "HR")
            {
                havepermission = true;
            }
            else if (CheckAdmin())
            {
                havepermission = true;
            }
            if (!havepermission)
            {
                return RedirectToAction("NoPermssion", "Home");
            }
            var announces = Enumerable.Reverse(AnnounceM.GetList()).ToList();
            return View(announces);
        }

        [HttpGet]
        public ActionResult AddAnnounce()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddAnnounce(Announce item)
        {
            item.WritingTime = DateTime.Now;
            item.WriterID = SessionUser().UserID;
            AnnounceM.Add(item);
            return RedirectToAction("Announces", "Admin");
        }

        [HttpGet]
        public ActionResult UpdateAnnounce(int id)
        {
            var announce = AnnounceM.GetByID(id);
            return View(announce);
        }

        [HttpPost]
        public ActionResult UpdateAnnounce(Announce item)
        {
            var pastannounce = AnnounceM.GetByID(item.AnnounceID);
            pastannounce.Title = item.Title;
            pastannounce.Text = item.Text;
            AnnounceM.Update(pastannounce);
            return RedirectToAction("Announces", "Admin");
        }
        #endregion

        #region Notficiation
        public ActionResult Notficiations()
        {
            int userid = (int)Session["UserID"];
            var notficiations = NotficiationM.NotficiationsofUser(userid);
            notficiations = Enumerable.Reverse(notficiations).ToList();
            foreach (var item in notficiations)
            {
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

        //public ActionResult UpdateAllUsers()
        //{
        //    var users = UserM.GetList();
        //    List<User> usersstack = new List<User>();
        //    foreach (var item in users)
        //    {

        //        if (item.UserID >= 3182)
        //        {
        //            //string mailSubject = "Rəqəmsal könüllülər təşkilatına profil qeydiyyatınız tamamlandı!";
        //            //string mailBody = "Hörmətli, " + item.Name + ".\n" +
        //            //    "Rəqəmsal könüllülər təşkilatı platforması üçün profil məlumatlarınız:\n" +
        //            //    "Giriş üçün istifadəçi adınız: " + item.UserName + "\n" +
        //            //    "Giriş üçün parolunuz: " + item.Password + "\n" +
        //            //    "Sizi təşkilatımızda gördüyümüzə şad olduq.\n" +
        //            //    "Hörmətlə, Rəqəmsal Könüllülər Təşkilatı.";

        //            //SendMail(item.EMail, mailSubject, mailBody);

        //            //item.Name = item.Name.Replace("?", "ə");
        //            //item.Surname = item.Surname.Replace("?", "ə");
        //            //item.Role = item.Role.Replace("?", "ə");
        //            //item.Department = item.Department.Replace("?", "ə");
        //            //item.Profession = item.Profession.Replace("?", "ə");

        //            //const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz123456789";

        //            //Random random = new Random();
        //            //char[] randomArray = new char[8];

        //            //for (int i = 0; i < 8; i++)
        //            //{
        //            //    randomArray[i] = chars[random.Next(chars.Length)];
        //            //}
        //            //item.Password = new string(randomArray);
        //            //item.UserImage = "/Images/ProfilePictures/defaultPP.jpg";

        //            string username = item.Surname.First() + "." + item.Name;
        //            username = username.ToLower();
        //            username = username.Replace("ə", "e");
        //            username = username.Replace("ü", "u");
        //            username = username.Replace("ö", "o");
        //            username = username.Replace("ç", "c");
        //            username = username.Replace("ş", "s");
        //            username = username.Replace("ı", "i");
        //            int quantity = usersstack.Where(x => x.UserName.StartsWith(username) &&
        //                (x.UserName.Substring(username.Length).IsInt() || x.UserName.Length == username.Length)).ToList().Count();
        //            if (quantity > 0)
        //            {
        //                username = username + (quantity + 1).ToString();
        //            }
        //            item.UserName = username;

        //            /*
        //            //if (item.Department == "Təskilati islər departamenti")
        //            //{
        //            //    item.Department = "Organizer";
        //            //}
        //            //else if (item.Department == "Insan Resursları departamenti")
        //            //{
        //            //    item.Department = "HR";
        //            //}
        //            //else if (item.Department == "Ictimaiyyətlə əlaqələr departamenti")
        //            //{
        //            //    item.Department = "PR";
        //            //}
        //            //else if (item.Department == "SMM departamenti")
        //            //{
        //            //    item.Department = "SMM";
        //            //}
        //            //else if (item.Department == "Üzvlərlə is departamenti")
        //            //{
        //            //    item.Department = "WWM";
        //            //}
        //            //else if (item.Department == "Fakültələrlə is departamenti")
        //            //{
        //            //    item.Department = "WWF";
        //            //}
        //            //else if (item.Department == "Beynəlxalq Əməkdaşlıq departamenti")
        //            //{
        //            //    item.Department = "ICD";
        //            //}
        //            //else if (item.Department == "Nəzarət Təftis Departamenti")
        //            //{
        //            //    item.Department = "Security";
        //            //}
        //            //else
        //            //{
        //            //    item.Department = "FS";
        //            //}

        //            //if (item.Role == "Üzv")
        //            //{
        //            //    item.Role = "Member";
        //            //}
        //            //else if (item.Role == "Rəhbər")
        //            //{
        //            //    item.Role = "Leader";
        //            //}
        //            //else if (item.Role == "Sədr Müavini")
        //            //{
        //            //    item.Role = "Vice-Chairman";
        //            //}
        //            //else if (item.Role == "Sədr")
        //            //{
        //            //    item.Role = "Chairman";
        //            //}
        //            //else if (item.Role == "Insan Resurslari")
        //            //{
        //            //    item.Role = "HR";
        //            //}
        //            //else if (item.Role == "Ictimaiyyətlə əlaqələr")
        //            //{
        //            //    item.Role = "PR";
        //            //}
        //            //else if (item.Role == "Informasiya Texnologiyaları")
        //            //{
        //            //    item.Role = "IT";
        //            //}
        //            //else if (item.Role == "SMM")
        //            //{
        //            //    item.Role = "SMM";
        //            //}
        //            //else if (item.Role == "Layihə Meneceri")
        //            //{
        //            //    item.Role = "PM";
        //            //}
        //            //else if (item.Role == "Copywriter")
        //            //{
        //            //    item.Role = "CW";
        //            //}
        //            //else if (item.Role == "Üzvlərlə is")
        //            //{
        //            //    item.Role = "WWM";
        //            //}
        //            //else if (item.Role == "Fotoqraf")
        //            //{
        //            //    item.Role = "Photograph";
        //            //}
        //            //else if (item.Role == "Fotoqraf/Videoqraf")
        //            //{
        //            //    item.Role = "Photograph/Videograph";
        //            //}
        //            //else if (item.Role == "Koordinator")
        //            //{
        //            //    item.Role = "Coordinator";
        //            //}
        //            //else if (item.Role == "Nəzarət və Təftis")
        //            //{
        //            //    item.Role = "Security";
        //            //}
        //            //else if (item.Role == "Qrafik Dizayner")
        //            //{
        //            //    item.Role = "GDesigner";
        //            //}
        //            //else if (item.Role == "Katib")
        //            //{
        //            //    item.Role = "Secretary";
        //            //}
        //            //else if (item.Role == "Müsavir")
        //            //{
        //            //    item.Role = "Advisor";
        //            //}
        //            //else
        //            //{
        //            //    item.Role = "Staff";
        //            //}
        //            */

        //            //if(item.Department == "FS")
        //            //{
        //            //    item.FacultyStaff = true;
        //            //}
        //            //else
        //            //{
        //            //    item.DepartmentStaff = true;
        //            //}



        //            //string name = item.Name;
        //            //if(item.Name.EndsWith("zadə"))
        //            //{
        //            //    item.Name = item.Surname;
        //            //    item.Surname = name;
        //            //}

        //            //item.Name = item.Name.First().ToString().ToUpper() + item.Name.Substring(1);
        //            //item.Surname = item.Surname.First().ToString().ToUpper() + item.Surname.Substring(1);

        //            //if (item.Name.StartsWith("I"))
        //            //{
        //            //    item.Name = "İ" + item.Name.Substring(1);
        //            //}
        //            //if (item.Surname.StartsWith("I"))
        //            //{
        //            //    item.Surname = "İ" + item.Surname.Substring(1);
        //            //}
        //            //if (item.Surname.Contains(" "))
        //            //{
        //            //    int startind = item.Surname.IndexOf(" ");
        //            //    item.Surname = item.Surname.Remove(startind);
        //            //}
        //            UserM.Update(item);
        //        }
        //        usersstack.Add(item);
        //    }
        //    return RedirectToAction("Index", "Home");
        //}

        private void SendMail(string to, string subject, string body)
        {
            using (MailMessage EMail = new MailMessage("digitalvolunteers.it@gmail.com", to))
            {
                EMail.Subject = subject;
                EMail.Body = body;
                EMail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    string emailfrom = "digitalvolunteers.it@gmail.com";
                    string password = "ywqyjakjgzczunhd";
                    NetworkCredential cred = new NetworkCredential(emailfrom, password);
                    smtp.Credentials = cred;
                    smtp.Port = 587;

                    smtp.Send(EMail);
                }
            }
        }

        private string RenderViewToString(string viewPath, object model)
        {
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewPath);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, new ViewDataDictionary(model), new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }

        private User SessionUser()
        {
            int userid = 0;
            try
            {
                userid = (int)Session["UserID"];
            }
            catch
            {
                RedirectToAction("UserLogin", "Login");
            }
            var user = UserM.GetByID(userid);
            return user;
        }
        private bool CheckAdmin()
        {
            var user = SessionUser();
            if (user.Role != "President" && user.Role != "Vice-President" && user.Role != "Leader" && user.Role != "Chairman"
                && user.Role != "Admin")
            {
                return false;
            }
            return true;
        }
        private bool CheckRector()
        {
            var user = SessionUser();
            if (user.Department == "Vice-Rector" || user.Department == "Rectorship" )
            {
                return true;
            }
            return false;
        }

        public void SendTgDatabaseMessage(string message, string buttonText, string buttonUrl)
        {
            string botToken = "6698675787:AAGyk8ndgzA1zms0UAyopwg8G6klU23rz04";
            string chatId = "1054410384";
            string apiUrl = $"https://api.telegram.org/bot{botToken}/sendMessage?chat_id={chatId}&text={message}";

            var keyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithUrl(buttonText, buttonUrl)
                }
            });
            var payload = new
            {
                chat_id = chatId,
                text = message,
                reply_markup = JsonConvert.SerializeObject(keyboard)
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = new HttpClient().PostAsync(apiUrl, content).Result;

        }
    }
}