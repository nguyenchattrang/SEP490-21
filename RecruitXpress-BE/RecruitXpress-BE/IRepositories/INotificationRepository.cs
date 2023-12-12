using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories;

public interface INotificationRepository
{
    Task<List<NotificationDTO>> GetNotificationByAccountId(int accountId);
    Task<NotificationDTO> SaveNotification(NotificationDTO notification);
    Task<bool> UpdateNotificationAfterSeen(int notificationId);
    Task<bool> DeleteNotification(int notificationId);
}