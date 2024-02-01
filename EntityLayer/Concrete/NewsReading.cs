using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class NewsReading
    {
        [Key]
        public int ReadingID { get; set; }
        public int UserID { get; set; }
        public virtual User User { get; set; }
        public int NewsID { get; set; }
        public virtual News News { get; set; }
    }
}
