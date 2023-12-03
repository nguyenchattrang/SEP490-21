using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{
    public class CalendarDTO
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

        public virtual AccountInformation? CandidateNavigation { get; set; }
        public virtual AccountInformation? CreatedByNavigation { get; set; }
        public virtual AccountInformation? InterviewerNavigation { get; set; }
    }
}
