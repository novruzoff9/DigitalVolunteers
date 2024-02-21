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
    public class NotficiationManager : IMainService<Notficiation>
    {
        IRepository<Notficiation> _repository;

        public NotficiationManager(IRepository<Notficiation> repository)
        {
            _repository = repository;
        }

        public void Add(Notficiation item)
        {
            _repository.Insert(item);
        }

        public void Delete(Notficiation item)
        {
            _repository.Delete(item);
        }

        public void DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public Notficiation GetByID(int id)
        {
            return _repository.Get(x => x.NotficiationID == id);
        }

        public List<Notficiation> GetList()
        {
            return _repository.List();
        }

        public void Update(Notficiation item)
        {
            _repository.Update(item);
        }
        public List<Notficiation> NotficiationsofUser(int id)
        {
            return _repository.List().Where(x=>x.RecieverID == id).ToList();
        }
    }
}
