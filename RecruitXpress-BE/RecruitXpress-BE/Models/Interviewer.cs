using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class Interviewer
    {
        public int InterviewerId { get; set; }
        public int? AccountId { get; set; }
        public int? ScheduleId { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Schedule? Schedule { get; set; }
    }
}
