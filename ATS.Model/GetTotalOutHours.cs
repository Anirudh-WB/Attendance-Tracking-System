using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ATS.Model
{
    public class GetTotalOutHours
    {
        public DateTime OutTime { get; set; }
        public DateTime? InTime { get; set; }
        public string TotalOutHours { get; set; }
    }
}
