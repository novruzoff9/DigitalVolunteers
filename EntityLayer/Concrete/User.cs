using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [StringLength(50)]
        public string UserName { get; set; }
        public string UserImage { get; set; }

        [StringLength(20)]
        public string Name { get; set; }

        [StringLength(30)]
        public string Surname { get; set; }
        public DateTime BirthDate { get; set; }

        [StringLength(40)]
        public string Faculty { get; set; }

        [StringLength(40)]
        public string Department { get; set; }
        [StringLength(40)]
        public string Role { get; set; }

        [StringLength(50)]
        public string Password { get; set; }
        public int ActivityPoint { get; set; }
        public DateTime SignDate { get; set; }
        public bool FacultyStaff { get; set; }
        public bool DepartmentStaff { get; set; }
        public string Gender { get; set; }
        public string Profession { get; set; }
        public string EMail { get; set; }
        public string Group { get; set; }
        public int PhoneNumber { get; set; }
        public int EntranceYear { get; set; }
        public DateTime LastOnline { get; set; }
        public virtual ICollection<ActivityPoints> Points { get; set; }
    }
}
