using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web_DigitalVolunteers.Controllers
{
    public class StaffController : Controller
    {
        // GET: Staff
        UserManager UserM = new UserManager(new EfUserDAL());
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult Presidents()
        {
            var users = UserM.GetList();
            List<User> managers = new List<User>();
            foreach (var item in users)
            {
                if(item.Department == "Management")
                {
                    managers.Add(item);
                }
            }
            //var presidents = users.Where(x => x.Role == "President").ToList();
            //var vicepresidents = users.Where(x => x.Role == "Vice-President").ToList();
            //presidents.AddRange(vicepresidents);
            //presidents.Add(users.FirstOrDefault(x => x.Role == "Secretary"));
            return PartialView(managers);
        }

        public PartialViewResult Leaders()
        {
            var chairmans = UserM.GetList().Where(x => x.Role == "Leader").ToList();
            return PartialView(chairmans);
        }


        public PartialViewResult FacultyChairmans()
        {
            var chairmans = UserM.GetList().Where(x => x.Role == "Chairman").ToList();
            var nihad = UserM.GetByID(7);
            chairmans.Add(nihad);
            return PartialView(chairmans);
        }
    }
}