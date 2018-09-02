using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CFIProjectUWP.Model
{
    public class CFIDetail
    {
        public String CRN { get; set; }
        public String SubjectCode { get; set; }
        public String CompetencyName { get; set; }
        public String StartDate { get; set; }
        public String EndDate { get; set; }
        public String DayOfWeek { get; set; }
        public String Time { get; set; }
        public String Room { get; set; }
        public String Lecturer { get; set; }
        public String Campus { get; set; }
    }
}
