using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO;

public class JobPostingSearchDTO
{
    public int? JobId { get; set; }
    public string? SearchString { get; set; }
    public string? Location { get; set; }
    public string? EmploymentType { get; set; }
    public string? Industry { get; set; }
    public int? LocationId { get; set; }
    public int? EmploymentTypeId { get; set; }
    public int? IndustryId { get; set; }
    public double? MinSalary { get; set; }
    public double? MaxSalary { get; set; }
    public DateTime? ApplicationDeadline { get; set; }
    public int? status { get; set; }

    public string? SortBy { get; set; }

    private readonly bool? _isSortAscending;

    public bool IsSortAscending
    {
        get => _isSortAscending ?? false;
        init => _isSortAscending = value;
    }

    public int? Page { get; set; }
    public int? Size { get; set; }
}

public class JobPostingPrepareSearch
{
    public List<City> Cities { get; set; } = null!;
    public List<EmploymentType> EmploymentTypes { get; set; } = null!;
    public List<Industry> Industries { get; set; } = null!;
}