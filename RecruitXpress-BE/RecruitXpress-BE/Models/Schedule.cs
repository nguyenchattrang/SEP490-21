using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class Schedule
    {
        public Schedule()
        {
            Interviewers = new HashSet<Interview>();
            ScheduleDetails = new HashSet<ScheduleDetail>();
        }

        public int ScheduleId { get; set; }
        public int? HumanResourceId { get; set; }
        public int? ExamId { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public Account? HumanResource { get; set; }
        public SpecializedExam? SpecializedExam { get; set; }
        public ICollection<Interview> Interviewers { get; set; }
        public ICollection<ScheduleDetail> ScheduleDetails { get; set; }
    }
}
