using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Announce
    {
        [Key]
        public int AnnounceID { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime WritingTime { get; set; }
        public int WriterID { get; set; }
        public virtual User Writer { get; set; }
    }
}
