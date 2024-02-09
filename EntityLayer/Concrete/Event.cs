using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [StringLength(100)]
        public string Title { get; set; }
        public string Caption { get; set; }
        public string Image { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
        public int Participant { get; set; }
        public DateTime Deadline { get; set;}
        public int ParticipantLimit { get; set; }
        public virtual ICollection<EventRegistration> Registrations { get; set; }
    }
}
