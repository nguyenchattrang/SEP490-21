using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly RecruitXpressContext _context;

    public ScheduleRepository(RecruitXpressContext context)
    {
        _context = context;
    }


    public async Task<List<Schedule>> GetListSchedules()
    {
        return await _context.Schedules
            .Include(s => s.Interviewers)
            .ThenInclude(i => i.Account)
            .Include(s => s.ScheduleDetails)
            .ThenInclude(sd => sd.Profile)
            .ToListAsync();
    }

    public Task<List<Schedule>> GetListSchedules(string? searchString, string? orderBy, bool? isSortAscending, int? accountId, int? page,
        int? size)
    {
        throw new NotImplementedException();
    }

    public Task<List<Schedule>> GetListScheduleAdvancedSearch(Schedule schedule, int? accountId, int page, int size)
    {
        throw new NotImplementedException();
    }

    public Task<Schedule?> GetSchedule(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Schedule> AddSchedule(Schedule schedule)
    {
        try
        {
            
            _context.Entry(schedule).State = EntityState.Added;
            await _context.SaveChangesAsync();
            foreach (var interviewer in schedule.Interviewers)
            {
                interviewer.ScheduleId = schedule.ScheduleId;
                _context.Entry(interviewer).State = EntityState.Added;
            }
            foreach (var scheduleDetail in schedule.ScheduleDetails)
            {
                scheduleDetail.ScheduleId = schedule.ScheduleId;
                _context.Entry(scheduleDetail).State = EntityState.Added;
            }
            
            await _context.SaveChangesAsync();
            return schedule;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Task<Schedule> UpdateSchedules(int id, Schedule Schedule)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteSchedule(int jobId)
    {
        throw new NotImplementedException();
    }
}