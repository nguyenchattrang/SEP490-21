using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.Hub;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories;

public class JobApplicationRepository : IJobApplicationRepository
{
    private readonly RecruitXpressContext _context;
    // private readonly IHubContext<JobApplicationStatusHub> _hubContext;
    private readonly JobApplicationStatusHub _applicationHubContext;

    public JobApplicationRepository(RecruitXpressContext context, IHubContext<JobApplicationStatusHub> hubContext, JobApplicationStatusHub applicationHubContext)
    {
        _context = context;
        // _hubContext = hubContext;
        _applicationHubContext = applicationHubContext;
    }

    public async Task<JobApplication?> UpdateJobApplicationStatus(int jobApplyId, int? accountId, int? status)
    {
        try
        {
            var detailJob = await _context.JobApplications
                .Include(ja => ja.Profile)
                .ThenInclude(p => p.Account)
                .FirstOrDefaultAsync(x => x.ApplicationId == jobApplyId);
            if (detailJob == null || status == null) return detailJob;
            var oldStatus = detailJob.Status ?? -1;
            detailJob.Status = status;
            if (accountId != null)
            {
                detailJob.AssignedFor = accountId;
            }
            _context.Update(detailJob);
            await _context.SaveChangesAsync();
            // await _hubContext.Clients.All.SendAsync("StatusChanged", jobApplyId, status);
            await _applicationHubContext.NotifyStatusUpgrade(detailJob, (int)status, oldStatus);
            return detailJob;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
    
    public async Task<JobApplication?> AutoAddStatus(int jobApplyId)
    {
        try
        {
            var detailJob = await _context.JobApplications
                .Include(ja => ja.Profile)
                .ThenInclude(p => p.Account)
                .FirstOrDefaultAsync(x => x.ApplicationId == jobApplyId);
            if (detailJob == null) throw new Exception("Không tìm thấy thông tin ứng tuyển!");
            if ( detailJob.Status == null) throw new Exception("Không tìm thấy trạng thái ứng tuyển!");
            var oldStatus = detailJob.Status ?? -1;
            var newStatus = oldStatus + 1;
            // await _hubContext.Clients.All.SendAsync("StatusChanged", jobApplyId, newStatus);
            await _applicationHubContext.NotifyStatusUpgrade(detailJob, newStatus, oldStatus);
            return detailJob;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
    public async Task<JobApplication?> FindJobApplicationAndUpdateStatus(int jobId, int accountId, int status)
    {
        try
        {
            var profile = await _context.Profiles.Where(p => p.AccountId == accountId).FirstOrDefaultAsync();
            if (profile == null)
            {
                throw new Exception("Không có profile");
            }
            var detailJob = await _context.JobApplications.FirstOrDefaultAsync(x => x.JobId == jobId && x.ProfileId == profile.ProfileId);
            if (detailJob == null)
            { throw new Exception("Không tìm được công việc tương ứng"); }
            return await UpdateJobApplicationStatus(detailJob.ApplicationId, null, status);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

}