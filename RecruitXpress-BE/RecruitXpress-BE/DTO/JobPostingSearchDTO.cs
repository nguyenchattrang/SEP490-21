
namespace RecruitXpress_BE.DTO;

public class JobPostingSearchDTO
{
    public string? SearchString { get; set; }
    public string? Location { get; set; }
    public string? EmploymentType { get; set; }
    public string? Industry { get; set; }
    public string? SalaryRange { get; set; }
    public DateTime? ApplicationDeadline { get; set; }
}