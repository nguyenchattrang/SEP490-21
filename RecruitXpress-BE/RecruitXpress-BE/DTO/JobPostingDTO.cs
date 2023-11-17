using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO;

public class JobPostingDTO
{
    public JobPostingDTO()
    {
        JobApplications = new HashSet<JobApplication>();
    }

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
    public int TotalCount  { get; set; }

    public virtual ICollection<JobApplication> JobApplications { get; set; }
}