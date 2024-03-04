using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Telegram.Bot;

namespace Web_DigitalVolunteers.Controllers
{
    public class TelegramBotController : Controller
    {

        // GET: TelegramBot
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SendMessqage()
        {
            
            return RedirectToAction("Index", "Home");
        }
    }
}