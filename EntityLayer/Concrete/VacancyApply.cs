using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class VacancyApply
    {
        [Key]
        public int ApplyID { get; set; }
        public int VacancyID { get; set; }
        public virtual Vacancy Vacancy { get; set; }
        public int UserID { get; set; }
        public virtual User User { get; set; }
        public DateTime ApplyDateTime { get; set; }
        public string ApplierNote { get; set; }
        public bool Interview { get; set; }
        public DateTime InterviewDateTime { get; set; }
        public bool Entered { get; set; }
        public DateTime EnteringDateTime { get; set; }
        public string Note { get; set; }
    }
}
