using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EntityLayer.Concrete
{
    public class News
    {
        [Key]
        public int NewsID { get; set; }
        public string NewsImage { get; set; }
        public string NewsTitle { get; set; }

        [AllowHtml]
        public string NewsCaption { get; set; }
        public DateTime NewsCreated { get; set; }
        public int Reading { get; set; }
        public virtual ICollection<NewsGallery> Images { get; set; }

    }
}
