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
    public class NewsReadingManager : IMainService<NewsReading>
    {
        IRepository<NewsReading> _newsReadingdal;

        public NewsReadingManager(IRepository<NewsReading> newsReadingdal)
        {
            _newsReadingdal = newsReadingdal;
        }

        public void Add(NewsReading item)
        {
            _newsReadingdal.Insert(item);
        }

        public void Delete(NewsReading item)
        {
            _newsReadingdal.Delete(item);
        }

        public void DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public NewsReading GetByID(int id)
        {
            return _newsReadingdal.Get(x => x.ReadingID == id);
        }

        public List<NewsReading> GetList()
        {
            return _newsReadingdal.List();
        }

        public void Update(NewsReading item)
        {
            _newsReadingdal.Update(item);
        }
    }
}
