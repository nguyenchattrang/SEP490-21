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

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var other = (StatusChange)obj;
        return OldStatus == other.OldStatus && NewStatus == other.NewStatus;
    }


    public override int GetHashCode()
    {
        return HashCode.Combine(OldStatus, NewStatus);
    }
}