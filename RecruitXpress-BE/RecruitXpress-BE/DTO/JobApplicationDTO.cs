
using Newtonsoft.Json;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{
    public class JobApplicationDTO
    {
        public int ApplicationId { get; set; }
        public int? JobId { get; set; }
        public int? ProfileId { get; set; }
        public int? TemplateId { get; set; }
        public int? Status { get; set; }

        public virtual JobDTO? Job { get; set; }

        public virtual LoggingDTO? Profile { get; set; }

        public virtual CvtemplateDTO? Template { get; set; }

    }
    public partial class LoggingDTO
    {
        public int? AccountId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }

    }
    public partial class CvtemplateDTO
    {
        public int TemplateId { get; set; }
        public string? Url { get; set; }

    }
    public partial class JobDTO{
    public int JobId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Company { get; set; }
    public string? Location { get; set; }
    public string? EmploymentType { get; set; }
    public string? Industry { get; set; }
    public string? Requirements { get; set; }
    public string? SalaryRange { get; set; }
    public DateTime? ApplicationDeadline { get; set; }
    public DateTime? DatePosted { get; set; }
    public string? ContactPerson { get; set; }
    public string? ApplicationInstructions { get; set; }
    public int? Status { get; set; }
    public bool IsPreferred { get; set; }
    }
}
