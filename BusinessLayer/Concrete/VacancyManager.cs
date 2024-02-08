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
    public class VacancyManager : IMainService<Vacancy>
    {
        IRepository<Vacancy> _vacancyRepository;

        public VacancyManager(IRepository<Vacancy> vacancyRepository)
        {
            _vacancyRepository = vacancyRepository;
        }

        public void Add(Vacancy item)
        {
            _vacancyRepository.Insert(item);
        }

        public void Delete(Vacancy item)
        {
            _vacancyRepository.Delete(item);
        }

        public void DeleteById(int id)
        {
            var vacancy = _vacancyRepository.Get(x => x.VacancyID == id);
            _vacancyRepository.Delete(vacancy);
        }

        public Vacancy GetByID(int id)
        {
            return _vacancyRepository.Get(x=>x.VacancyID == id);
        }

        public List<Vacancy> GetList()
        {
            return _vacancyRepository.List();
        }

        public void Update(Vacancy item)
        {
            _vacancyRepository.Update(item);
        }
    }
}
