using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class PasswordTokens
    {
        [Key]
        public int PasswordTokenID { get; set; }
        public int UserID { get; set; }
        public virtual User User { get; set; }
        [StringLength(20)]
        public string Token { get; set; }
        public DateTime CreationDate { get; set; }
        public bool Used { get; set; }
    }
}
