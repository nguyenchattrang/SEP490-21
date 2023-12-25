using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Helper;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Hub;

using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class JobApplicationStatusHub : Hub
{
    private readonly IHubContext<HubContext> _hubContext;
    private readonly INotificationRepository _notificationRepository;

    public JobApplicationStatusHub(IHubContext<HubContext> hubContext, INotificationRepository notificationRepository)
    {
        _hubContext = hubContext;
        _notificationRepository = notificationRepository;
    }

    public async Task NotifyStatusUpgrade(JobApplication? jobApplication, int newStatus, int oldStatus)
    {
        try
        {
            var notification = Constant.APPLICAION_STATUS_NOTIFICATION[new StatusChange()
            {
                NewStatus = newStatus, OldStatus = oldStatus
            }];
            if (notification.Description!.Contains("@candidateName"))
            {
                notification.Description =
                    notification.Description?.Replace("@candidateName", jobApplication?.Profile?.Account?.FullName);
            }
            
            if (notification.Description!.Contains("@industry"))
            {
                notification.Description =
                    notification.Description?.Replace("@industry", jobApplication?.Job?.IndustryNavigation?.IndustryName);
            }
            
            if (notification.Description!.Contains("@company"))
            {
                notification.Description =
                    notification.Description?.Replace("@company", jobApplication?.Job?.Company);
            }

            if (jobApplication != null)
            {
                await _notificationRepository.SaveNotification(new NotificationDTO()
                {
                    Title = notification.Title,
                    Description = notification.Description,
                    Seen = false,
                    Status = 1,
                    ApplicationId = jobApplication.ApplicationId,
                    CreatedDate = DateTime.Now,
                    TargetUrl = notification.TargetUrl,
                    ReceiverId = jobApplication.Profile?.AccountId
                });
                await _hubContext.Clients.All.SendAsync("StatusChanged", jobApplication?.Profile?.Account?.AccountId, newStatus,
                    notification.Title,
                    notification.Description);
            }
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine(ex);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}