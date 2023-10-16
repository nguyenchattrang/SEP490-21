using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class Schedule
    {
        public Schedule()
        {
            Interviewers = new HashSet<Interviewer>();
            ScheduleDetails = new HashSet<ScheduleDetail>();
        }

        public int ScheduleId { get; set; }
        public DateTime? Date { get; set; }
        public int? Round { get; set; }
        public string? Hr { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<Interviewer> Interviewers { get; set; }
        public virtual ICollection<ScheduleDetail> ScheduleDetails { get; set; }
    }
}
