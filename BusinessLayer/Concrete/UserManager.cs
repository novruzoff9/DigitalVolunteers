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
            return _UserDal.Get(x => x.UserID == id);
        }

        public User GetByUserName(string username)
        {
            return _UserDal.Get(x => x.UserName == username);
        }

        public List<User> GetList()
        {
            return _UserDal.List();
        }

        public void Update(User item)
        {
            _UserDal.Update(item);
        }

        public User FindProfile(string username, string password)
        {
            return _UserDal.Get(x => x.UserName == username && x.Password == password);
        }
    }
}
