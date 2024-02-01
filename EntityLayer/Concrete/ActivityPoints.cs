using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class ActivityPoints
    {
        [Key]
        public int PointID { get; set; }
        public int UserID { get; set; }
        public virtual User User { get; set; }
        public int WriterID { get; set; }
        public string Activity { get; set; }
        public int Point { get; set; }
        public bool Verified { get; set; }
        public DateTime Date { get; set; }
    }
}
