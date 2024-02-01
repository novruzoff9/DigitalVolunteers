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
    public class EventManager : IMainService<Event>
    {
        IRepository<Event> _EventDal;

        public EventManager(IRepository<Event> eventDal)
        {
            _EventDal = eventDal;
        }

        public void Add(Event item)
        {
            _EventDal.Insert(item);
        }

        public void Delete(Event item)
        {
            throw new NotImplementedException();
        }

        public void DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public Event GetByID(int id)
        {
            return _EventDal.Get(x => x.EventID == id);
        }

        public List<Event> GetList()
        {
            return _EventDal.List();
        }

        public void Update(Event item)
        {
            _EventDal.Update(item);
        }
    }
}
