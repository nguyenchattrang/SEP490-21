﻿using RecruitXpress_BE.Helper;

namespace RecruitXpress_BE.Hub;

using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class JobApplicationStatusHub : Hub
{
    private readonly IHubContext<HubContext> _hubContext;

    public JobApplicationStatusHub(IHubContext<HubContext> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyStatusChange(int jobApplicationId, int newStatus)
    {
        try
        {
            var notification = Constant.APPLICAION_STATUS_NOTIFICATION[1];
            await _hubContext.Clients.All.SendAsync("StatusChanged", jobApplicationId, newStatus, notification[0], notification[1]);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}