using RecruitXpress_BE.Helper;

namespace RecruitXpress_BE.Hub;

using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class JobApplicationStatusHub : Hub
{
    public async Task NotifyStatusChange(int jobApplicationId, int newStatus)
    {
        var notification = Constant.APPLICAION_STATUS_NOTIFICATION[1];
        await Clients.All.SendAsync("StatusChanged", jobApplicationId, newStatus, notification[0], notification[1]);
    }
}