using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Notficiation
    {

        [Key]
        public int NotficiationID { get; set; }
        public int RecieverID { get; set; }
        public virtual User Reciever { get; set; }
        public int UserID { get; set; }
        public virtual User User { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime WritingTime { get; set; }
        public int WriterID { get; set; }
        public virtual User Writer { get; set; }
        public bool Seen { get; set; }
    }
}
