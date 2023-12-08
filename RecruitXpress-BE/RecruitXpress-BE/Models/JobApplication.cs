using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class JobApplication
    {
        public JobApplication()
        {
            Exams = new HashSet<Exam>();
            ScheduleDetails = new HashSet<ScheduleDetail>();
        }

        public int ApplicationId { get; set; }
        public int? JobId { get; set; }
        public int? ProfileId { get; set; }
        public int? TemplateId { get; set; }
        public int? Status { get; set; }
        public int? AssignedFor { get; set; }
        public int? Shorted { get; set; }
        public string? UrlCandidateCV { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual JobPosting? Job { get; set; }
        public virtual Profile? Profile { get; set; }
        public virtual CandidateCv? Template { get; set; }
        public virtual ICollection<Exam> Exams { get; set; }
        public virtual ICollection<ScheduleDetail> ScheduleDetails { get; set; }
        public virtual ICollection<Calendar> Calendars { get; set; }
        public virtual ICollection<Evaluate> Evaluates { get; set; }
    }
}
