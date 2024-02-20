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

namespace DigitalVolunteers.Controllers
{
    public class LoginController : Controller
    {
        UserManager UserM = new UserManager(new EfUserDAL());
        AdminManager AdminM = new AdminManager(new EfAdminDAL());
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
                if(user.Role == "Member" || user.Department == "Member")
                {
                    return Json("Member", JsonRequestBehavior.AllowGet);
                }
                else if(user.Department == "Rectorship" || user.Department == "Vice-Rector")
                {
                    return Json("Rectorship", JsonRequestBehavior.AllowGet);
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
    }
}