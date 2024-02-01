using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class NewsManager : IMainService<News>
    {
        IRepository<News> _NewsDal;

        public NewsManager(IRepository<News> newsdal) {
            _NewsDal = newsdal;
        }

        public void Add(News item)
        {
            _NewsDal.Insert(item);
        }

        public void Delete(News item)
        {
            _NewsDal.Delete(item);
        }

        public void DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public News GetByID(int id)
        {
            return _NewsDal.Get(x => x.NewsID == id);
        }

        public List<News> GetList()
        {
            return _NewsDal.List();
        }

        public void Update(News item)
        {
            _NewsDal.Update(item);
        }
    }
}
