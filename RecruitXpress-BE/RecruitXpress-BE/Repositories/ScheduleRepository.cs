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
        return null;
    }

    public async Task<List<ScheduleDTO>> GetListSchedules(int profileId)
    {
        try
        {
            var result = await _context.Schedules
                .Include(s => s.HumanResource)
                .Include(s => s.ScheduleDetails)
                .ThenInclude(sd => sd.Candidate)
                .Include(s => s.Interviewers)
                .ThenInclude(i => i.InterviewerNavigation)
                .Select(s => new ScheduleDTO()
                {
                    ScheduleId = s.ScheduleId,
                    HumanResource = new Profile()
                    {
                        ProfileId = s.HumanResource.ProfileId,
                        Name = s.HumanResource.Name
                    },
                    Interviewers = s.Interviewers.Select(i => new Interviewer
                    {
                        InterviewerNavigation = new Profile()
                        {
                            ProfileId = i.InterviewerNavigation.ProfileId,
                            Name = i.InterviewerNavigation.Name
                        }
                    }).ToList(),
                    ScheduleDetails = s.ScheduleDetails
                })
                .Where(s => s.HumanResource.ProfileId == profileId
                            || s.Interviewers.Select(i => i.InterviewerNavigation.ProfileId).Contains(profileId)
                            || s.ScheduleDetails.Select(sd => sd.Candidate.ProfileId).Contains(profileId)
                            )
                .ToListAsync();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Task<List<Schedule>> GetListScheduleAdvancedSearch(Schedule schedule, int? accountId, int page, int size)
    {
        throw new NotImplementedException();
    }

    public Task<Schedule?> GetSchedule(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ScheduleDTO> AddSchedule(ScheduleDTO scheduleDTO)
    {
        try
        {
            if (!_context.Profiles.Any(p => p.ProfileId == scheduleDTO.HumanResourceId))
            {
                throw new Exception("Human Resource is not exist!");
            }

            var schedule = new Schedule()
                {
                    HumanResourceId = scheduleDTO.HumanResourceId,
                    Status = scheduleDTO.Status,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now,
                    CreatedBy = scheduleDTO.CreatedBy,
                    UpdatedBy = scheduleDTO.UpdatedBy
                };
            _context.Entry(schedule).State = EntityState.Added;
            await _context.SaveChangesAsync();
            
            foreach (var interviewer in scheduleDTO.Interviewers)
            {
                if (!_context.Profiles.Any(p => p.ProfileId == interviewer.InterviewerId))
                {
                    throw new Exception("Interviewer is not exist!");
                }
                
                interviewer.ScheduleId = schedule.ScheduleId;
                if (!_context.Interviewers.Any(i =>
                        i.ScheduleId == interviewer.ScheduleId && i.InterviewerId == interviewer.InterviewerId))
                {
                    _context.Entry(interviewer).State = EntityState.Added;
                }
            }
            
            foreach (var scheduleDetail in scheduleDTO.ScheduleDetails)
            {
                if (!_context.Profiles.Any(p => p.ProfileId == scheduleDetail.CandidateId))
                {
                    throw new Exception("Candidate is not exist!");
                }
                
                var scheduleDetailEntity = new ScheduleDetail
                {
                    ScheduleId = schedule.ScheduleId,
                    CandidateId = scheduleDetail.CandidateId,
                    Status = scheduleDetail.Status,
                    ScheduleType = scheduleDetail.ScheduleType,
                    StartDate = scheduleDetail.StartDate,
                    EndDate = scheduleDetail.EndDate,
                    Note = scheduleDetail.Note,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now,
                    CreatedBy = scheduleDTO.CreatedBy,
                    UpdatedBy = scheduleDTO.UpdatedBy
                };
                _context.Entry(scheduleDetailEntity).State = EntityState.Added;
            }
            await _context.SaveChangesAsync();
            return scheduleDTO;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ScheduleDTO> UpdateSchedules(int id, ScheduleDTO scheduleDTO)
    {
        try
        {
            var schedule = await _context.Schedules.FindAsync(scheduleDTO.ScheduleId);
            
            if (schedule == null)
            {
                throw new Exception("Schedule not found!");
            }
            
            schedule = new Schedule()
            {
                ScheduleId = schedule.ScheduleId,
                HumanResourceId = scheduleDTO.HumanResourceId,
                Status = scheduleDTO.Status,
                CreatedTime = schedule.CreatedTime,
                UpdatedTime = DateTime.Now,
                CreatedBy = schedule.CreatedBy,
                UpdatedBy = scheduleDTO.UpdatedBy
            };
            
            _context.Entry(schedule).State = EntityState.Modified;
            
            foreach (var interviewer in scheduleDTO.Interviewers)
            {
                if (!_context.Profiles.Any(p => p.ProfileId == interviewer.InterviewerId))
                {
                    throw new Exception("Interviewer is not exist!");
                }
                
                interviewer.ScheduleId = schedule.ScheduleId;
                if (!_context.Interviewers.Any(i =>
                        i.ScheduleId == interviewer.ScheduleId && i.InterviewerId == interviewer.InterviewerId))
                {
                    _context.Entry(interviewer).State = EntityState.Added;
                }
            }
            
            foreach (var scheduleDetail in scheduleDTO.ScheduleDetails)
            {
                if (!_context.Profiles.Any(p => p.ProfileId == scheduleDetail.CandidateId))
                {
                    throw new Exception("Candidate is not exist!");
                }
                
                var scheduleDetailEntity = new ScheduleDetail
                {
                    ScheduleId = schedule.ScheduleId,
                    CandidateId = scheduleDetail.CandidateId,
                    Status = scheduleDetail.Status,
                    ScheduleType = scheduleDetail.ScheduleType,
                    StartDate = scheduleDetail.StartDate,
                    EndDate = scheduleDetail.EndDate,
                    Note = scheduleDetail.Note,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now,
                    CreatedBy = scheduleDTO.CreatedBy,
                    UpdatedBy = scheduleDTO.UpdatedBy
                };
                _context.Entry(scheduleDetailEntity).State = EntityState.Added;
            }
            await _context.SaveChangesAsync();
            return scheduleDTO;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Task<bool> DeleteSchedule(int jobId)
    {
        throw new NotImplementedException();
    }
}