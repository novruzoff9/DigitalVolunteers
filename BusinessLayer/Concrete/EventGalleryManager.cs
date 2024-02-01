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
    public class EventGalleryManager : IMainService<EventGallery>
    {

        IRepository<EventGallery> _EventGaleryDal;

        public EventGalleryManager(IRepository<EventGallery> eventgaleryDal)
        {
            _EventGaleryDal = eventgaleryDal;
        }

        public void Add(EventGallery item)
        {
            _EventGaleryDal.Insert(item);
        }

        public void Delete(EventGallery item)
        {
            _EventGaleryDal.Delete(item);
        }

        public void DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public EventGallery GetByID(int id)
        {
            return _EventGaleryDal.Get(x=>x.ImageID ==  id);
        }

        public List<EventGallery> GetList()
        {
            return _EventGaleryDal.List();
        }

        public void Update(EventGallery item)
        {
            _EventGaleryDal.Update(item);
        }
    }
}
