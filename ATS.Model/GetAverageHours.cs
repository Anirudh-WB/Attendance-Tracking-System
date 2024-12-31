using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATS.Model
{
    public class GetAverageHours
    {

        public long UserId { get; set; }

        public TimeSpan TotalAvg { get; set; }

        public TimeSpan TotalInAvg { get; set; }

        public TimeSpan TotalOutAvg { get; set; }
    }
}
