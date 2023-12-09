using RecruitXpress_BE.Helper;

namespace RecruitXpress_BE.Hub;

using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class HubContext : Hub
{
    public async Task NotifyStatusChange(int jobApplicationId, int newStatus)
    {
        try
        {
            await Clients.All.SendAsync("StatusChanged", jobApplicationId, newStatus);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}