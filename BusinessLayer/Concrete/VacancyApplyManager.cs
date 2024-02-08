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
    public class VacancyApplyManager : IMainService<VacancyApply>
    {
        IRepository<VacancyApply> _repository;

        public VacancyApplyManager(IRepository<VacancyApply> repository)
        {
            _repository = repository;
        }

        public void Add(VacancyApply item)
        {
            _repository.Insert(item);
        }

        public void Delete(VacancyApply item)
        {
            _repository.Delete(item);
        }

        public void DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public VacancyApply GetByID(int id)
        {
            return _repository.Get(x => x.ApplyID == id);
        }

        public List<VacancyApply> GetList()
        {
            return _repository.List();
        }

        public void Update(VacancyApply item)
        {
            _repository.Update(item);
        }

        public List<VacancyApply> AppliesOfUser(int id)
        {
            return _repository.List().Where(x => x.UserID == id).ToList();
        }

        public List<VacancyApply> AppliesOfVacancy(int id)
        {
            return _repository.List().Where(x => x.VacancyID == id).ToList();
        }

    }
}
