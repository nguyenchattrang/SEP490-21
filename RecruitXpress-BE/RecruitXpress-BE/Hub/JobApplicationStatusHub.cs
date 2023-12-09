using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Helper;
using RecruitXpress_BE.IRepositories;

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

    public async Task NotifyStatusChange(int jobApplicationId, int newStatus)
    {
        try
        {
            var notification = Constant.APPLICAION_STATUS_NOTIFICATION[newStatus];
            if (notification.Title!.Contains("[candidateName]"))
            {
                var candidate = 
                notification.Title = notification.Title.Replace("[candidateName]", "");
            }
            await _notificationRepository.SaveNotification(new NotificationDTO()
            {
                Title = notification.Title,
                Description = notification.Description,
                Seen = false,
                Status = 1,
                ApplicationId = jobApplicationId,
                CreatedDate = DateTime.Now,
                TargetUrl = notification.TargetUrl
            });
            await _hubContext.Clients.All.SendAsync("StatusChanged", jobApplicationId, newStatus, notification.Title, notification.Description);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}