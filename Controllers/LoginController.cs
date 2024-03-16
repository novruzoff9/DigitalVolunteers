using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Helpers;
using Telegram.Bot.Types;
using System.Web.WebPages;
using Newtonsoft.Json;
using System.Net.Http;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;

namespace DigitalVolunteers.Controllers
{
    public class LoginController : Controller
    {
        UserManager UserM = new UserManager(new EfUserDAL());
        DailyLoginManager LoginM = new DailyLoginManager(new EfDailyLoginDAL());
        PasswordTokenManager PassTokenM = new PasswordTokenManager(new EfPasswordTokenDAL());

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserLogin()
        {
            return View();
        }
        
        public ActionResult ForgotPassword()
        {
            return View();
        }

        public ActionResult MemberRegistration()
        {
            return View();
        }

        public JsonResult AddRegistration(string FirstName, string LastName, string Email, int PhoneNumber, string Faculty, string Profession, DateTime BirthDate, int EntranceYear, string Group, string Gender)
        {
            var users = UserM.GetList();
            Dictionary<string, object> response = new Dictionary<string, object>();
            response["Status"] = "registered";
            if (users.FirstOrDefault(x=>x.EMail == Email) != null)
            {
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            string username = LastName.First() + "." + FirstName;
            username = username.ToLower();

            username = username.Replace("ə", "e");
            username = username.Replace("ü", "u");
            username = username.Replace("ö", "o");
            username = username.Replace("ç", "c");
            username = username.Replace("ş", "s");
            username = username.Replace("ı", "i");
            username = username.Replace("ğ", "g");

            int quantity = users.Where(x => x.UserName.StartsWith(username) &&
                (x.UserName.Substring(username.Length).IsInt() || x.UserName.Length == username.Length)).ToList().Count();
            if (quantity > 0)
            {
                username = username + (quantity + 1).ToString();
            }

            EntityLayer.Concrete.User newUser = new EntityLayer.Concrete.User
            {
                UserName = username,
                UserImage = "/Images/ProfilePictures/defaultPP.jpg",
                Name = FirstName,
                Surname = LastName,
                BirthDate = BirthDate,
                Faculty = Faculty,
                Department = "Waiting Email",
                Role = "",
                Password = "",
                ActivityPoint = 0,
                SignDate = DateTime.Now,
                FacultyStaff = false,
                DepartmentStaff = false,
                Gender = Gender,
                Profession = Profession,
                EMail = Email,
                Group = Group,
                PhoneNumber = PhoneNumber,
                EntranceYear = EntranceYear,
                LastOnline = DateTime.Now
            };

            UserM.Add(newUser);

            string verificationcode; 
            const string chars = "0123456789";

            Random random = new Random();
            char[] randomArray = new char[6];

            for (int i = 0; i < 6; i++)
            {
                randomArray[i] = chars[random.Next(chars.Length)];
            }
            verificationcode = new string(randomArray);

            users = Enumerable.Reverse(UserM.GetList()).ToList();
            PasswordTokens token = new PasswordTokens();

            var addeduser = users.FirstOrDefault(x => x.UserName == username);

            token.UserID = addeduser.UserID;
            token.CreationDate = DateTime.Now;
            token.Used = false;
            token.Token = verificationcode;

            PassTokenM.Add(token);

            var addedtoken = PassTokenM.GetList().FirstOrDefault(x => x.UserID == addeduser.UserID && x.Used == false && x.Token == token.Token);
            addedtoken.User = addeduser;
            string viewPath = "~/Views/Login/ProfileVerifyCodeEMail.cshtml";
            string mailBody = RenderViewToString(viewPath, addedtoken);

            SendMail(Email, "Qeydiyyat təsdiqi", mailBody);

            string tgmessage = $"İstifadəçilər cədvəlinə '{FirstName} {LastName}' adlı yeni qeydiyyat artırıldı" +
                $"İstifadəçi adı : {username} \n";
            SendTgDatabaseMessage(tgmessage, "İstifadəçi siyahısı", $"https://www.digitalvolunteers.xyz/Admin/SelectedUsers");
            response["Status"] = "success";
            response["UserID"] = token.UserID;
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerifyProfile(int UserID)
        {
            var user = UserM.GetByID(UserID);
            ViewBag.waiting = "false";
            ViewBag.UserID = UserID;

            if (user.Department == "Waiting Email")
            {
                var tokens = PassTokenM.GetList();
                var token = tokens.LastOrDefault(x=>x.UserID == UserID);
                if(token.CreationDate.AddDays(10) <= DateTime.Now) { ViewBag.waiting = "late";  }
                else { ViewBag.waiting = "true"; }
            }
            return View();
        }

        public JsonResult CheckVerifyCode(int userid, string code)
        {
            var token = PassTokenM.GetList().LastOrDefault(x => x.UserID == userid);
            string response;
            if (code == token.Token)
            {
                var user = UserM.GetByID(userid);
                user.Department = "Waiting";
                UserM.Update(user);
                token.Used = true;
                PassTokenM.Update(token);
                response = "success";
                return Json(response, JsonRequestBehavior.AllowGet);
            }
            response = "wrong";
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendLink(string email)
        {
            var user = UserM.GetList().FirstOrDefault(x=>x.EMail == email);
            if(user == null)
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            PasswordTokens token = new PasswordTokens();
            token.UserID = user.UserID;

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            Random random = new Random();
            char[] randomArray = new char[20];

            for (int i = 0; i < 20; i++)
            {
                randomArray[i] = chars[random.Next(chars.Length)];
            }
            token.Token = new string(randomArray);
            while (PassTokenM.GetList().First(x=>x.Token == token.Token) != null)
            {
                for (int i = 0; i < 20; i++)
                {
                    randomArray[i] = chars[random.Next(chars.Length)];
                }

                token.Token = new string(randomArray);
            }


            token.CreationDate = DateTime.Now;
            token.Used = false;

            PassTokenM.Add(token);
            string mailSubject = "Şifrənizi yeniləyin";
            string mailBody = "Aşağıdakı düyməyə klik edərək şifrənizi yeniləyə bilərsiniz<br>" +
                "<a href='https://digitalvolunteers.xyz/Login/ResetPassword?token=" + token.Token + "'>Şifrənizi yeniləyin</a>";

            SendMail(email, mailSubject, mailBody);
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ResetPassword(string token)
        {
            var passtoken = PassTokenM.GetList().FirstOrDefault(x => x.Token == token);
            if(passtoken == null) { 
                return RedirectToAction("UserLogin", "Login");
            }
            int userid = passtoken.UserID;
            ViewBag.userid = userid;
            ViewBag.token = token;
            return View();
        }

        public JsonResult NewPassword(string token, int userid, string password)
        {
            var passtoken = PassTokenM.GetList().FirstOrDefault(x => x.Token == token);
            if(passtoken == null)
            {
                return Json("Wrong data", JsonRequestBehavior.AllowGet);
            }
            if (passtoken.Used == true)
            {
                return Json("Used", JsonRequestBehavior.AllowGet);
            }
            if(passtoken.CreationDate.AddDays(1) < DateTime.Now)
            {
                return Json("Gone", JsonRequestBehavior.AllowGet);
            }
            if(passtoken.UserID != userid)
            {
                return Json("Wrong data", JsonRequestBehavior.AllowGet);
            }
            var user = UserM.GetByID(userid);
            user.Password = password;
            UserM.Update(user);
            passtoken.Used = true;
            PassTokenM.Update(passtoken);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.RemoveAll();
            return RedirectToAction("UserLogin");
        }

        public ActionResult RegistrationEmail(EntityLayer.Concrete.User user)
        {
            return View(user);
        }

        public ActionResult ProfileVerifyCodeEMail(PasswordTokens token)
        {
            return View(token);
        }

        public JsonResult Authentication(string username, string password)
        {
            var user = UserM.FindProfile(username, password);
            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.UserName, false);
                Session["UserName"] = user.UserName;
                Session["UserFirstName"] = user.Name;
                Session["UserSurName"] = user.Surname;
                Session["UserID"] = user.UserID;
                var dailylogins = LoginM.LoginsofToday();                
                if(dailylogins.FirstOrDefault(x=> x.UserID == user.UserID) == null)
                {
                    DailyLogin login = new DailyLogin();
                    login.LoginDateTime = DateTime.Today;
                    login.UserID = user.UserID;
                    LoginM.Add(login);
                }
                if(user.Role == "Member" && user.Department == "Member")
                {
                    return Json("Member", JsonRequestBehavior.AllowGet);
                }
                else if(user.Department == "Rectorship" || user.Department == "Vice-Rector")
                {
                    return Json("Rectorship", JsonRequestBehavior.AllowGet);
                }
                else if(user.Department == "Waiting Email")
                {
                    return Json("Waiting Email", JsonRequestBehavior.AllowGet);
                }
                else if (user.Department == "Waiting")
                {
                    return Json("Waiting", JsonRequestBehavior.AllowGet);
                }
                return Json("Staff", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

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
    }
}