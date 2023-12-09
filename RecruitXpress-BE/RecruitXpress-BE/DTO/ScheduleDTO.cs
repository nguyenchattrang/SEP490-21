using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO;

public class ScheduleDTO
{
    public ScheduleDTO()
    {
        Interviewers = new HashSet<InterviewDTO>();
        ScheduleDetails = new HashSet<ScheduleDetailDTO>();
    }

    public int ScheduleId { get; set; }
    public int? HumanResourceId { get; set; }
    public string? HumanResourceName { get; set; }
    public int? SpecializedExamId { get; set; }
    public int? Status { get; set; }
    public DateTime? CreatedTime { get; set; }
    public DateTime? UpdatedTime { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public SpecializedExamDTO? SpecializedExam { get; set; }

    public virtual ICollection<InterviewDTO> Interviewers { get; set; }
    public virtual ICollection<ScheduleDetailDTO> ScheduleDetails { get; set; }
}

public class ScheduleAdditionDataDTO
{
    public int? HumanResourceId { get; set; }
    public string? HumanResourceName { get; set; }
    public int? SpecializedExamId { get; set; }
    public SpecializedExamDTO? SpecializedExam { get; set; }
    public List<CandidateSchedule?> Candidates { get; set; } = new();
    public List<InterviewerSchedule> Interviewers { get; set; } = new();
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? type { get; set; }
    public string? note { get; set; }
}

public class CandidateSchedule
{
    public int? CandidateId { get; set; }
    public int? ApplicationId { get; set; }
    public string? CandidateName { get; set; }
    public string? CandidateEmail { get; set; }
}

public class InterviewerSchedule
{
    public int? InterviewerId { get; set; }
    public string? InterviewerName { get; set; }
}