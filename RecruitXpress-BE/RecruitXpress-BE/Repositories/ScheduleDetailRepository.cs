using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories;

public class ScheduleDetailRepository : IScheduleDetailRepository
{
    private readonly RecruitXpressContext _context;

    public ScheduleDetailRepository(RecruitXpressContext context)
    {
        _context = context;
    }

    public Task<ScheduleDetail> GetScheduleDetails(int scheduleId)
    {
        throw new NotImplementedException();
    }

    public Task<ScheduleDetail> GetScheduleDetail(ScheduleDetail scheduleDetail)
    {
        throw new NotImplementedException();
    }

    public Task<ScheduleDetail> CreateScheduleDetail(ScheduleDetail scheduleDetail)
    {
        throw new NotImplementedException();
    }

    public async Task<ScheduleDetail> UpdateScheduleDetail(int id, ScheduleDetail scheduleDetail)
    {
        try
        {
            var detail = await _context.ScheduleDetails.SingleOrDefaultAsync(sd =>
                sd.ScheduleDetailId == id);
            if (detail == null)
            {
                throw new Exception("Schedule detail not found!");
            }

            if (scheduleDetail.ScheduleType != null)
            {
                detail.ScheduleType = scheduleDetail.ScheduleType;
            }

            if (scheduleDetail is { StartDate: not null, EndDate: not null })
            {
                if (scheduleDetail.EndDate != null && scheduleDetail.StartDate >= scheduleDetail.EndDate)
                {
                    throw new Exception("Start date must greater then end date");
                }

                detail.StartDate = scheduleDetail.StartDate;
                detail.EndDate = scheduleDetail.EndDate;
            }

            if (scheduleDetail.ScheduleType != null)
            {
                detail.ScheduleType = scheduleDetail.ScheduleType;
            }

            if (!string.IsNullOrEmpty(scheduleDetail.Note))
            {
                detail.Note = scheduleDetail.Note;
            }

            detail.UpdatedTime = DateTime.Now;
            detail.UpdatedBy = scheduleDetail.UpdatedBy;

            _context.Entry(detail).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return detail;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteScheduleDetail(int scheduleDetailId)
    {
        try
        {
            var detail = await _context.ScheduleDetails.SingleOrDefaultAsync(sd =>
                sd.ScheduleDetailId == scheduleDetailId);
            if (detail == null)
            {
                throw new Exception("Schedule detail not found!");
            }

            _context.Entry(detail).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}