using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO;

public partial class ScheduleDetailDTO
{
    public int ScheduleDetailId { get; set; }
    public int? ScheduleId { get; set; }
    public int? CandidateId { get; set; }
    public string? CandidateName { get; set; }
    public int? ApplicationId { get; set; }
    public int? ScheduleType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Note { get; set; }
    public DateTime? CreatedTime { get; set; }
    public DateTime? UpdatedTime { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public int? Status { get; set; }
    public string? Location { get; set; }
    public string? Strength { get; set; }
    public string? Imperfection { get; set; }
    public int? Evaluate { get; set; }

}