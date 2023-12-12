using Microsoft.AspNetCore.Mvc;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Controllers;

[Route("api/Notification/")]
[ApiController]

public class NotificationController : ControllerBase
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationController(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }
    
    [HttpGet("{accountId:int}")]
    public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetListNotificationByAccountId(int accountId)
    {
        try
        {
            return await _notificationRepository.GetNotificationByAccountId(accountId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "Có lỗi xảy ra trong quá trình nhận thông báo!");
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> SaveNotification(NotificationDTO notificationDto)
    {
        try
        {
            await _notificationRepository.SaveNotification(notificationDto);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "Có lỗi xảy ra trong quá trình gửi thông báo!");
        }
    }
    
    [HttpPut("{notificationId:int}")]
    public async Task<IActionResult> UpdateNotificationAfterSeen(int notificationId)
    {
        try
        {
            await _notificationRepository.UpdateNotificationAfterSeen(notificationId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        return Ok();
    }
    
    [HttpDelete("{notificationId:int}")]
    public async Task<IActionResult> DeleteNotification(int notificationId)
    {
        try
        {
            await _notificationRepository.DeleteNotification(notificationId);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "Có lỗi xảy ra trong quá trình xóa thông báo!");
        }
    }
}