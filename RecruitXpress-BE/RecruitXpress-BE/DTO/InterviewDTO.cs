namespace RecruitXpress_BE.DTO;

public class InterviewDTO
{
    public int InterviewerId { get; set; }
    public int ScheduleId { get; set; }
    public string? InterviewerName { get; set; }
    public int? Status { get; set; }
}