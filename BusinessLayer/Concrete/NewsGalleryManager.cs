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
    public class NewsGalleryManager : IMainService<NewsGallery>
    {
        IRepository<NewsGallery> _repository;

        public NewsGalleryManager(IRepository<NewsGallery> repository)
        {
            _repository = repository;
        }

        public void Add(NewsGallery item)
        {
            _repository.Insert(item);
        }

        public void Delete(NewsGallery item)
        {
            _repository.Delete(item);
        }

        public void DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public NewsGallery GetByID(int id)
        {
            return _repository.Get(x => x.ImageID == id);
        }

        public List<NewsGallery> GetList()
        {
            return _repository.List();
        }

        public void Update(NewsGallery item)
        {
            _repository.Update(item);
        }
    }
}
