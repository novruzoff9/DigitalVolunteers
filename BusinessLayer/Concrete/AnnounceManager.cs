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
    public class AnnounceManager : IMainService<Announce>
    {
        IRepository<Announce> _repository;

        public AnnounceManager(IRepository<Announce> repository)
        {
            _repository = repository;
        }

        public void Add(Announce item)
        {
            _repository.Insert(item);
        }

        public void Delete(Announce item)
        {
            _repository.Delete(item);
        }

        public void DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public Announce GetByID(int id)
        {
            return _repository.Get(x => x.AnnounceID == id);
        }

        public List<Announce> GetList()
        {
            return _repository.List();
        }

        public void Update(Announce item)
        {
            _repository.Update(item);
        }
    }
}
