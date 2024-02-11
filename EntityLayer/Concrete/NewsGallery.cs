using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class NewsGallery
    {
        [Key]
        public int ImageID { get; set; }
        public int NewsID { get; set; }
        public virtual News News { get; set; }
        public string ImageURL { get; set; }
    }
}
