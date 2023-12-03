using System;
using System.Collections.Generic;

namespace RecruitXpress_BE.Models
{
    public partial class Calendar
    {
        public int Id { get; set; }
        public string? EventName { get; set; }
        public int? Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public int? Interviewer { get; set; }
        public int? Candidate { get; set; }
        public string? Note { get; set; }
        public int? JobApplicationId { get; set; }
        public int? JobId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? Status { get; set; }

        public virtual Account? CandidateNavigation { get; set; }
        public virtual Account? CreatedByNavigation { get; set; }
        public virtual Account? InterviewerNavigation { get; set; }
        public virtual JobPosting? Job { get; set; }
        public virtual JobApplication? JobApplication { get; set; }
    }
}
