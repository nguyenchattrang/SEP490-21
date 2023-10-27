
namespace RecruitXpress_BE.DTO;

public class JobPostingSearchDTO
{
    public string? SearchString { get; set; }
    public string? Location { get; set; }
    public string? EmploymentType { get; set; }
    public string? Industry { get; set; }
    public string? SalaryRange { get; set; }
    public DateTime? ApplicationDeadline { get; set; }
    
    public string? SortBy { get; set; }

    private bool? _isSortAscending;

    public bool IsSortAscending
    {
        get => _isSortAscending ?? false;
        set => _isSortAscending = value;
    }
}