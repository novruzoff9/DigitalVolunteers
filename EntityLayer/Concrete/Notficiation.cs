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
        public Notficiation()
        {
            if (WriterID == 0)
            {
                Writer = new User
                {
                    Name = "Sistem",
                    Surname = "Tərəfindən",
                    UserImage = "/Images/Logo.png",
                    BirthDate = new DateTime(2005, 9, 23),
                    LastOnline = DateTime.Now,
                    SignDate = new DateTime(2005, 9, 23)
                };
            }
        }

        [Key]
        public string NotficiationID { get; set; }
        public int RecieverID { get; set; }
        public virtual User Reciever { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime WritingTime { get; set; }
        public int WriterID { get; set; }
        public virtual User Writer { get; set; }
    }
}
