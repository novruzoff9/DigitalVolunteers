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
    public class DailyLoginManager : IMainService<DailyLogin>
    {
        IRepository<DailyLogin> _DailyLoginDal;

        public DailyLoginManager(IRepository<DailyLogin> dailyLoginDal)
        {
            _DailyLoginDal = dailyLoginDal;
        }

        public void Add(DailyLogin item)
        {
            _DailyLoginDal.Insert(item);
        }

        public void Delete(DailyLogin item)
        {
            throw new NotImplementedException();
        }

        public void DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public DailyLogin GetByID(int id)
        {
            return _DailyLoginDal.Get(x => x.LoginID == id);
        }

        public List<DailyLogin> GetList()
        {
            return _DailyLoginDal.List();
        }

        public void Update(DailyLogin item)
        {
            throw new NotImplementedException();
        }

        public List<DailyLogin> DailyLogins()
        {
            return _DailyLoginDal.List().Where(x => x.LoginDateTime == DateTime.Today).ToList();
        }
    }
}
