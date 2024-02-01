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
    public class ActivityPointManager : IMainService<ActivityPoints>
    {
        IRepository<ActivityPoints> _ActivityPointDal;

        public ActivityPointManager(IRepository<ActivityPoints> ActivityPointDal)
        {
            _ActivityPointDal = ActivityPointDal;
        }

        public void Add(ActivityPoints item)
        {
            _ActivityPointDal.Insert(item);
        }

        public void Delete(ActivityPoints item)
        {
            _ActivityPointDal.Delete(item);
        }

        public void DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public ActivityPoints GetByID(int id)
        {
            return _ActivityPointDal.Get(x => x.PointID == id);
        }

        public List<ActivityPoints> GetList()
        {
            return _ActivityPointDal.List();
        }

        public void Update(ActivityPoints item)
        {
            _ActivityPointDal.Update(item);
        }
    }
}
