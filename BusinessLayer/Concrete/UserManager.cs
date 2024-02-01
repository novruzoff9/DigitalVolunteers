using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class UserManager : IMainService<User>
    {

        IRepository<User> _UserDal;

        public UserManager(IRepository<User> userDal)
        {
            _UserDal = userDal;
        }

        public void Add(User item)
        {
            _UserDal.Insert(item);
        }

        public void Delete(User item)
        {
            _UserDal.Delete(item); 
        }

        public void DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public User GetByID(int id)
        {
            var user = _UserDal.Get(x => x.UserID == id);
            //if (user.Faculty == "Digital Economy") { user.Faculty = "Rəqəmsal İqtisadiyyat fakültəsi"; }
            //else if (user.Faculty == "Russian Economy") { user.Faculty = "Rus İqtisad Məktəbi"; }
            //else if (user.Faculty == "Business") { user.Faculty = "Biznes və Menecement"; }
            //else if (user.Faculty == "Sabah") { user.Faculty = "SABAH mərkəzi"; }
            //else if (user.Faculty == "ISE") { user.Faculty = "Beynəlxalq İqtisadiyyat Məktəbi"; }
            //else if (user.Faculty == "Design") { user.Faculty = "UNEC Dizayn Məktəbi"; }
            //else if (user.Faculty == "Finance") { user.Faculty = "Maliyyə və Mühasibat fəkültəsi"; }
            //else if (user.Faculty == "TUDIFAK") { user.Faculty = "Türk Dünyası İqtisad fəkültəsi"; }
            //else if (user.Faculty == "Engineering") { user.Faculty = "Mühəndislik fakültəsi"; }
            //else if (user.Faculty == "Economy") { user.Faculty = "İqtisadiyyat və İdarəetmə fakültəsi"; }
            return user;
        }

        public List<User> GetList()
        {
            var userlist = _UserDal.List();
            foreach (var item in userlist)
            {
                if (item.Role == "Secretary") item.Role = "Təşkilat katibi";
            }
            return userlist;
        }

        public void Update(User item)
        {
            _UserDal.Update(item);
        }
    }
}
