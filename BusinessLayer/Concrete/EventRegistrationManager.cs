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
    public class EventRegistrationManager : IMainService<EventRegistration>
    {
        IRepository<EventRegistration> _repository;

        public EventRegistrationManager(IRepository<EventRegistration> repository)
        {
            _repository = repository;
        }

        public void Add(EventRegistration item)
        {
            _repository.Insert(item);
        }

        public void Delete(EventRegistration item)
        {
            _repository.Delete(item);
        }

        public void DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public EventRegistration GetByID(int id)
        {
            return _repository.Get(x => x.RegistrationID == id);
        }

        public List<EventRegistration> GetList()
        {
            return _repository.List();
        }

        public void Update(EventRegistration item)
        {
            _repository.Update(item);
        }

        public List<EventRegistration> ParticipiantsOfEvent(int id)
        {
            return _repository.List().Where(x=>x.EventID == id).ToList();
        }
    }
}
