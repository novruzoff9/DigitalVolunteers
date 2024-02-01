using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Objects
{
    public class DataChange
    {
        public string Name { get; set; }
        public int NowValue { get; set; }
        public int PreviusValue { get; set; }
        public string Change { get; set; }
    }
}
