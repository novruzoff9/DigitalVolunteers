using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Vacancy
    {
        [Key]
        public int VacancyID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime Deadline { get; set; }
        public string Department { get; set; }
        public bool Primary { get; set; }
        public virtual ICollection<VacancyApply> Applies { get; set; }
    }
}
