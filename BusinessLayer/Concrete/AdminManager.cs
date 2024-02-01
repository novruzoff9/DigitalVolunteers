using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class AdminManager : IMainService<Admin>
    {
        IRepository<Admin> _AdminDal;

        public AdminManager(IRepository<Admin> adminDal)
        {
            _AdminDal = adminDal;
        }

        public void Add(Admin item)
        {
            _AdminDal.Insert(item);
        }

        public void Delete(Admin item)
        {
            _AdminDal.Delete(item);
        }

        public void DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public Admin GetByID(int id)
        {
            return _AdminDal.Get(x => x.AdminID == id);
        }

        public List<Admin> GetList()
        {
            return _AdminDal.List();
        }

        public void Update(Admin item)
        {
            _AdminDal.Update(item);
        }
    }
}
