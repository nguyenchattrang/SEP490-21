namespace RecruitXpress_BE.DTO;

public class NotificationMessage
{
    public string? Title { get; set; }
    public string? TargetUrl { get; set; }
    public string? Description { get; set; }
}

public class StatusChange
{
    public int OldStatus { get; set; }
    public int NewStatus { get; set; }
}