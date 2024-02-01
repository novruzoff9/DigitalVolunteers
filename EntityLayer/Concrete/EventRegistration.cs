using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class EventRegistration
    {
        [Key]
        public int RegistrationID { get; set; }
        public int EventID { get; set; }
        public virtual Event Event { get; set; }
        public int UserID { get; set; }
        public virtual User User { get; set; }
        public DateTime RegistrationTime {  get; set; }
        public bool Participated { get; set; }
        public int Rating { get; set; }
        public string Review { get; set; }
    }
}
