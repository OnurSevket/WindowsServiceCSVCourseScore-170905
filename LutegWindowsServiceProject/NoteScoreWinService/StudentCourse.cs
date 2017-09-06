using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteScoreWinService
{
    public class StudentCourse
    {
        public int? ID { get; set; }
        public string Course { get; set; }
        public string StudentNumber { get; set; }
        public string Midterm1 { get; set; }
        public string Midterm2 { get; set; }
        public string Midterm3 { get; set; }
        public string Final { get; set; }
    }
}
