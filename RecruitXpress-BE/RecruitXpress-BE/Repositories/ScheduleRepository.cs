using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Helper;
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

    public async Task<ScheduleResponse> GetListSchedules(int accountId, DateTime? startDate,
        DateTime? endDate)
    {
        try
        {
            startDate ??= DateTime.Parse("1/1/1753");
            endDate ??= DateTime.Parse("31/12/9999");
            var role = _context.Accounts.Find(accountId).Role.RoleId;
            var query = _context.Schedules
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
                    ScheduleDetails = s.ScheduleDetails.Where(sd => sd.StartDate > startDate && sd.EndDate < endDate).ToList()
                })
                .Where(s => s.ScheduleDetails.Count > 0)
                .AsQueryable();
            switch (role)
            {
                case Constant.ROLE.INTERVIEWER:
                    query = query.Where(s =>
                        s.Interviewers.Select(i => i.InterviewerNavigation.ProfileId).Contains(accountId));
                    break;
                case Constant.ROLE.CANDIDATE:
                    query = query.Where(s =>
                        s.ScheduleDetails.Select(sd => sd.Candidate.ProfileId).Contains(accountId));
                    break;
                default:
                    query = query.Where(s => s.HumanResource.ProfileId == accountId);
                    break;
            }

            var scheduleDTOResult = await query.ToListAsync();
            var scheduleAdditionData = new Dictionary<int, Dictionary<int, Dictionary<int, List<ScheduleAdditionDataDTO>>>>();
            List<ScheduleAdditionDataYear> scheduleAdditionDataResult = new List<ScheduleAdditionDataYear>();

            foreach (var scheduleDto in scheduleDTOResult)
            {
                var scheduleAdditionDataDTO = new ScheduleAdditionDataDTO()
                {
                    HumanResourceName = scheduleDto.HumanResource.Name,
                    InterviewerName = scheduleDto.Interviewers.Select(i => i.InterviewerNavigation.Name).ToList()
                };
                foreach (var scheduleDetail in scheduleDto.ScheduleDetails)
                {
                    scheduleAdditionDataDTO.CandidateName = scheduleDetail.Candidate.Name;
                    scheduleAdditionDataDTO.type = scheduleDetail.ScheduleType ?? (scheduleAdditionDataDTO.InterviewerName.Count > 0 ? 1 : 2);
                    var scheduleDate = scheduleDetail.StartDate.Value;
                    if (!scheduleAdditionData.ContainsKey(scheduleDate.Year))
                    {
                        scheduleAdditionData.Add(
                            scheduleDate.Year,
                            new Dictionary<int, Dictionary<int, List<ScheduleAdditionDataDTO>>>()
                            );
                    }
                    
                    if (!scheduleAdditionData[scheduleDate.Year].ContainsKey(scheduleDate.Month))
                    {
                        scheduleAdditionData[scheduleDate.Year].Add(
                            scheduleDate.Month,
                            new Dictionary<int, List<ScheduleAdditionDataDTO>>()
                        );
                    }
                    
                    if (!scheduleAdditionData[scheduleDate.Year][scheduleDate.Month].ContainsKey(scheduleDate.Day))
                    {
                        scheduleAdditionData[scheduleDate.Year][scheduleDate.Month].Add(
                            scheduleDate.Day,
                            new List<ScheduleAdditionDataDTO>()
                        );
                    }

                    scheduleAdditionData[scheduleDate.Year][scheduleDate.Month][scheduleDate.Day].Add(scheduleAdditionDataDTO);
                }
            }

            return new ScheduleResponse()
            {
                ScheduleDTOs = scheduleDTOResult,
                ScheduleAdditionData = scheduleAdditionDataResult
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
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