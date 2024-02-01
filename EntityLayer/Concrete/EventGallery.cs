using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class EventGallery
    {
        [Key]
        public int ImageID { get; set; }
        public int EventID { get; set; }
        public string ImageURL { get; set; }
    }
}
