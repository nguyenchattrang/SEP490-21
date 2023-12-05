using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly RecruitXpressContext _context;
    private readonly IMapper _mapper;

    public NotificationRepository(RecruitXpressContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<NotificationDTO>> GetNotificationByAccountId(int accountId) =>
        _mapper.Map<List<NotificationDTO>>(await _context.Notifications.Where(n => n.ReceiverId == accountId)
            .ToListAsync());

    public async Task<NotificationDTO> SaveNotification(NotificationDTO notificationDto)
    {
        var notification = _mapper.Map<Notification>(notificationDto);
        notification.Seen = false;
        notification.CreatedDate = DateTime.Now;
        notification.Status = 1;
        _context.Entry(notification).State = EntityState.Added;
        await _context.SaveChangesAsync();
        return notificationDto;
    }

    public async Task<bool> UpdateNotificationAfterSeen(int notificationId)
    {
        var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.NotificationId == notificationId);
        if (notification == null)
        {
            throw new Exception("Không tìm thấy thông báo!");
        }

        notification.Seen = true;
        _context.Entry(notification).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteNotification(int notificationId)
    {
        var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.NotificationId == notificationId);
        if (notification == null)
        {
            throw new Exception("Không tìm thấy thông báo!");
        }

        _context.Entry(notification).State = EntityState.Deleted;
        await _context.SaveChangesAsync();
        return true;
    }
}