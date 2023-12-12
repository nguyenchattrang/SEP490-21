namespace RecruitXpress_BE.DTO;

public class NotificationDTO
{
    public int NotificationId { get; set; }
    public int ApplicationId { get; set; }
    public string? Title { get; set; }
    public bool? Seen { get; set; }
    public int? SenderId { get; set; }
    public int? ReceiverId { get; set; }
    public string? TargetUrl { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedDate { get; set; }
    public int? Status { get; set; }
}