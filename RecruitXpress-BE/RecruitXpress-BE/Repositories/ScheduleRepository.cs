using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Helper;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly RecruitXpressContext _context;
    private readonly IJobApplicationRepository _jobApplicationRepository;
    private readonly IEmailTemplateRepository _emailTemplateRepository;

    public ScheduleRepository(RecruitXpressContext context, IJobApplicationRepository jobApplicationRepository,
        IEmailTemplateRepository emailTemplateRepository)
    {
        _context = context;
        _jobApplicationRepository = jobApplicationRepository;
        _emailTemplateRepository = emailTemplateRepository;
    }


    public Task<List<Schedule>>? GetListSchedules()
    {
        return null;
    }

    public async Task<Dictionary<int, Dictionary<int, Dictionary<int, List<ScheduleAdditionDataDTO>>>>> GetListSchedules(int accountId, DateTime? startDate,
        DateTime? endDate)
    {
        try
        {
            startDate ??= DateTime.Parse("1/1/1753");
            endDate ??= DateTime.Parse("31/12/9999");
            var account = _context.Accounts
                .SingleOrDefault(a => a.AccountId == accountId);
            if (account == null)
            {
                throw new Exception("Tài khoản không tồn tại!");
            }

            var query = _context.Schedules
                .Include(s => s.HumanResource)
                .Include(s => s.ScheduleDetails)
                .ThenInclude(sd => sd.Candidate)
                .ThenInclude(ja => ja.Profile)
                .ThenInclude(p => p.Account)
                .Include(s => s.Interviewers)
                .ThenInclude(i => i.InterviewerNavigation)
                .Where(s => s.ScheduleDetails.Count > 0)
                .AsQueryable();
            switch (account.RoleId)
            {
                case Constant.ROLE.INTERVIEWER:
                    query = query.Where(s =>
                        s.Interviewers.Select(i => i.InterviewerNavigation.AccountId).Contains(accountId));
                    break;
                case Constant.ROLE.CANDIDATE:
                    query = query.Where(s =>
                        s.ScheduleDetails.Select(sd => sd.Candidate.Profile.AccountId).Contains(accountId));
                    break;
                default:
                    query = query.Where(s => s.HumanResource != null && s.HumanResource.AccountId == accountId);
                    break;
            }

            var scheduleDtoResult = await query.Select(s => new ScheduleDTO()
            {
                ScheduleId = s.ScheduleId,
                HumanResourceId = s.HumanResource != null ? s.HumanResource.AccountId : null,
                HumanResourceName = s.HumanResource != null ? s.HumanResource.FullName : null,
                Status = s.Status,
                CreatedTime = s.CreatedTime,
                UpdatedTime = s.UpdatedTime,
                CreatedBy = s.CreatedBy,
                UpdatedBy = s.UpdatedBy,
                Interviewers = s.Interviewers.Select(i => new InterviewDTO
                {
                    ScheduleId = s.ScheduleId,
                    InterviewerId = i.InterviewerId,
                    InterviewerName = i.InterviewerNavigation != null ? i.InterviewerNavigation.FullName : null,
                    Status = i.Status
                }).ToList(),
                ScheduleDetails = s.ScheduleDetails.Where(sd => sd.StartDate > startDate && sd.EndDate < endDate)
                    .Select(sd => new ScheduleDetailDTO()
                    {
                        ScheduleDetailId = sd.ScheduleDetailId,
                        ScheduleId = s.ScheduleId,
                        CandidateId = sd.CandidateId,
                        CandidateName = sd.Candidate != null
                            ? sd.Candidate.Profile != null
                                ? sd.Candidate.Profile.Account != null
                                    ? sd.Candidate.Profile.Account.FullName
                                    : null
                                : null
                            : null,
                        ScheduleType = sd.ScheduleType,
                        StartDate = sd.StartDate,
                        EndDate = sd.EndDate,
                        Note = sd.Note,
                        CreatedBy = sd.CreatedBy,
                        CreatedTime = sd.CreatedTime,
                        UpdatedBy = sd.UpdatedBy,
                        UpdatedTime = sd.UpdatedTime,
                        Strength = sd.Strength,
                        Imperfection = sd.Imperfection,
                        Evaluate = sd.Evaluate,
                        Status = sd.Status
                    }).ToList()
            }).ToListAsync();
            // return scheduleDtoResult;
            // var scheduleAdditionDataResult = new List<ScheduleAdditionDataYear>();
            //
            // foreach (var scheduleDto in scheduleDtoResult)
            // {
            //     var scheduleAdditionDataDto = new ScheduleAdditionDataDTO()
            //     {
            //         HumanResourceName = scheduleDto.HumanResourceName,
            //         InterviewerName = scheduleDto.Interviewers.Select(i => i.InterviewerName).ToList()
            //     };
            //     foreach (var scheduleDetail in scheduleDto.ScheduleDetails)
            //     {
            //         scheduleAdditionDataDto.CandidateName = scheduleDetail.CandidateName;
            //         scheduleAdditionDataDto.type = scheduleDetail.ScheduleType ??
            //                                        (scheduleAdditionDataDto.InterviewerName.Count > 0 ? 1 : 2);
            //         if (scheduleDetail.StartDate != null)
            //         {
            //             var scheduleDate = scheduleDetail.StartDate.Value;
            //             if (!scheduleAdditionDataResult.Exists(result => result.Year == scheduleDate.Year))
            //             {
            //                 scheduleAdditionDataResult.Add(new ScheduleAdditionDataYear()
            //                 {
            //                     Year = scheduleDate.Year,
            //                     ScheduleAdditionDataMonths = new List<ScheduleAdditionDataMonth>()
            //                 });
            //             }
            //
            //             var scheduleAdditionDataYear =
            //                 scheduleAdditionDataResult.Find(result => result.Year == scheduleDate.Year);
            //
            //             if (scheduleAdditionDataYear != null &&
            //                 !scheduleAdditionDataYear.ScheduleAdditionDataMonths.Exists(result =>
            //                     result.Month == scheduleDate.Month))
            //             {
            //                 scheduleAdditionDataYear.ScheduleAdditionDataMonths.Add(new ScheduleAdditionDataMonth()
            //                 {
            //                     Month = scheduleDate.Month,
            //                     ScheduleAdditionDataDays = new List<ScheduleAdditionDataDay>()
            //                 });
            //             }
            //
            //             var scheduleAdditionDataMonth =
            //                 scheduleAdditionDataYear?.ScheduleAdditionDataMonths.Find(result =>
            //                     result.Month == scheduleDate.Month);
            //
            //             if (scheduleAdditionDataMonth != null &&
            //                 !scheduleAdditionDataMonth.ScheduleAdditionDataDays.Exists(result =>
            //                     result.Day == scheduleDate.Day))
            //             {
            //                 scheduleAdditionDataMonth.ScheduleAdditionDataDays.Add(new ScheduleAdditionDataDay()
            //                 {
            //                     Day = scheduleDate.Day,
            //                     ScheduleAdditionDataDTOs = new List<ScheduleAdditionDataDTO>()
            //                 });
            //             }
            //
            //             var scheduleAdditionDataDay =
            //                 scheduleAdditionDataMonth?.ScheduleAdditionDataDays.Find(result =>
            //                     result.Day == scheduleDate.Day);
            //             scheduleAdditionDataDay?.ScheduleAdditionDataDTOs.Add(scheduleAdditionDataDto);
            //         }
            //     }
            // }

            var scheduleAdditionDataResult =
                new Dictionary<int, Dictionary<int, Dictionary<int, List<ScheduleAdditionDataDTO>>>>();
            foreach (var scheduleDto in scheduleDtoResult)
            {
                foreach (var scheduleDtoScheduleDetail in scheduleDto.ScheduleDetails)
                {
                    if (scheduleDtoScheduleDetail.StartDate == null) continue;
                    var startDateValue = scheduleDtoScheduleDetail.StartDate.Value;
                    if (!scheduleAdditionDataResult.ContainsKey(startDateValue.Year))
                    {
                        scheduleAdditionDataResult.Add(startDateValue.Year, new Dictionary<int, Dictionary<int, List<ScheduleAdditionDataDTO>>>());
                    }
                    if (!scheduleAdditionDataResult[startDateValue.Year].ContainsKey(startDateValue.Month))
                    {
                        scheduleAdditionDataResult[startDateValue.Year].Add(startDateValue.Month, new Dictionary<int, List<ScheduleAdditionDataDTO>>());
                    }
                    if (!scheduleAdditionDataResult[startDateValue.Year][startDateValue.Month].ContainsKey(startDateValue.Day))
                    {
                        scheduleAdditionDataResult[startDateValue.Year][startDateValue.Month].Add(startDateValue.Day, new List<ScheduleAdditionDataDTO>());
                    }
                    scheduleAdditionDataResult[startDateValue.Year][startDateValue.Month][startDateValue.Day].Add(new ScheduleAdditionDataDTO()
                    {
                        CandidateName = scheduleDtoScheduleDetail.CandidateName,
                        HumanResourceName = scheduleDto.HumanResourceName,
                        InterviewerName = scheduleDto.Interviewers.Select(i => i.InterviewerName).ToList(),
                        type = scheduleDtoScheduleDetail.ScheduleType,
                        StartTime = scheduleDtoScheduleDetail.StartDate,
                        EndTime = scheduleDtoScheduleDetail.EndDate,
                        note = scheduleDtoScheduleDetail.Note
                    });
                }
            }

            return scheduleAdditionDataResult;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ScheduleDTO?> GetSchedule(int id)
    {
        var query = _context.Schedules
            .Include(s => s.HumanResource)
            .Include(s => s.ScheduleDetails)
            .ThenInclude(sd => sd.Candidate)
            .ThenInclude(ja => ja.Profile)
            .ThenInclude(p => p.Account)
            .Include(s => s.Interviewers)
            .ThenInclude(i => i.InterviewerNavigation)
            .Where(s => s.ScheduleId == id)
            .AsQueryable();

        var scheduleDtoResult = await query.Select(s => new ScheduleDTO()
        {
            ScheduleId = s.ScheduleId,
            HumanResourceId = s.HumanResource != null ? s.HumanResource.AccountId : null,
            HumanResourceName = s.HumanResource != null ? s.HumanResource.FullName : null,
            Interviewers = s.Interviewers.Select(i => new InterviewDTO
            {
                ScheduleId = s.ScheduleId,
                InterviewerId = i.InterviewerId,
                InterviewerName = i.InterviewerNavigation != null ? i.InterviewerNavigation.FullName : null,
                Status = i.Status
            }).ToList(),
            ScheduleDetails = s.ScheduleDetails.Select(sd => new ScheduleDetailDTO()
            {
                ScheduleDetailId = sd.ScheduleDetailId,
                ScheduleId = s.ScheduleId,
                CandidateId = sd.CandidateId,
                ScheduleType = sd.ScheduleType,
                StartDate = sd.StartDate,
                EndDate = sd.EndDate,
                Note = sd.Note,
                CreatedBy = sd.CreatedBy,
                CreatedTime = sd.CreatedTime,
                UpdatedBy = sd.UpdatedBy,
                UpdatedTime = sd.UpdatedTime,
                Status = sd.Status
            }).ToList(),
            CreatedTime = s.CreatedTime,
            CreatedBy = s.CreatedBy,
            UpdatedTime = s.UpdatedTime,
            UpdatedBy = s.UpdatedBy,
            Status = s.Status
        }).FirstOrDefaultAsync();
        return scheduleDtoResult;
    }

    public async Task<ScheduleDTO> AddSchedule(ScheduleDTO scheduleDto)
    {
        var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var hrAccount = await _context.Accounts.Where(p => p.AccountId == scheduleDto.HumanResourceId)
                .FirstOrDefaultAsync();
            if (hrAccount == null)
            {
                throw new Exception("HR không tồn tại!");
            }

            var schedule = new Schedule()
            {
                HumanResourceId = hrAccount.AccountId,
                Status = scheduleDto.Status,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                CreatedBy = scheduleDto.CreatedBy,
                UpdatedBy = scheduleDto.UpdatedBy
            };
            _context.Entry(schedule).State = EntityState.Added;
            await _context.SaveChangesAsync();
            var interviewerNames = "";
            foreach (var interviewer in scheduleDto.Interviewers)
            {
                var interviewerAccount = await _context.Accounts.Where(p => p.AccountId == interviewer.InterviewerId)
                    .FirstOrDefaultAsync();
                if (interviewerAccount == null)
                {
                    throw new Exception("Người phỏng vấn không tồn tại!");
                }

                interviewerNames += interviewerAccount.FullName + ", ";
                var interview = new Interview()
                {
                    ScheduleId = schedule.ScheduleId,
                    InterviewerId = interviewerAccount.AccountId,
                    Status = interviewer.Status
                };
                if (!_context.Interviews.Any(i =>
                        i.ScheduleId == interviewer.ScheduleId && i.InterviewerId == interviewerAccount.AccountId))
                {
                    _context.Entry(interview).State = EntityState.Added;
                }
            }

            foreach (var scheduleDetail in scheduleDto.ScheduleDetails)
            {
                var candidateApplication = await _context.JobApplications
                    .Where(ja => ja.ApplicationId == scheduleDetail.ApplicationId)
                    .FirstOrDefaultAsync();
                if (candidateApplication == null)
                {
                    throw new Exception("Không tìm thấy hồ sơ ứng viên!");
                }

                if (scheduleDetail.StartDate >= scheduleDetail.EndDate)
                {
                    throw new Exception("Thời gian kết thúc phải lớn hơn thời gian bắt đầu!");
                }

                if (scheduleDetail.StartDate < DateTime.Now)
                {
                    throw new Exception("Thời gian bắt đầu phải lớn hơn hoặc bằng thời gian hiện tại!");
                }

                var scheduleDetailEntity = await _context.ScheduleDetails.FirstOrDefaultAsync(sd =>
                    sd.CandidateId == candidateApplication.ApplicationId &&
                    sd.ScheduleType == scheduleDetail.ScheduleType);

                if (scheduleDetailEntity != null)
                {
                    throw new Exception(
                        "Ứng viên đã được tạo " + (scheduleDetail.ScheduleType == Constant.SCHEDULE_TYPE.INTERVIEW
                            ? "lịch phỏng vấn!"
                            : "lịch thi!"));
                }

                scheduleDetailEntity = new ScheduleDetail
                {
                    ScheduleId = schedule.ScheduleId,
                    CandidateId = candidateApplication.ApplicationId,
                    Status = scheduleDetail.Status,
                    ScheduleType = scheduleDetail.ScheduleType,
                    StartDate = scheduleDetail.StartDate,
                    EndDate = scheduleDetail.EndDate,
                    Note = scheduleDetail.Note,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now,
                    CreatedBy = scheduleDto.CreatedBy,
                    UpdatedBy = scheduleDto.UpdatedBy
                };
                _context.Entry(scheduleDetailEntity).State = EntityState.Added;
                if (scheduleDetail.ScheduleType == Constant.SCHEDULE_TYPE.INTERVIEW)
                {
                    if (candidateApplication.Status != 5)
                    {
                        throw new Exception("Hồ sơ ứng viên chưa đạt yêu cầu để phỏng vấn!");
                    }
                    var interviewerId = scheduleDto.Interviewers.FirstOrDefault()?.InterviewerId ?? null;
                    await _jobApplicationRepository.UpdateJobApplicationStatus(candidateApplication.ApplicationId,
                        interviewerId, 6);

                    await _emailTemplateRepository.SendEmailInterviewSchedule(candidateApplication.ApplicationId,
                        scheduleDetail.StartDate.ToString() ??
                        throw new InvalidOperationException("Thời gian phỏng vấn không được để trống!"),
                        scheduleDetail.Location ??
                        throw new InvalidOperationException("Địa điểm phỏng vấn không được để trống!"),
                        interviewerNames.Substring(0, interviewerNames.Length - 2));
                    await _emailTemplateRepository.SendEmailScheduleForInterviewer(candidateApplication.ApplicationId,
                        scheduleDetail.StartDate.ToString() ??
                        throw new InvalidOperationException("Thời gian phỏng vấn không được để trống!"),
                        scheduleDetail.Location);
                }
                else if (scheduleDetail.ScheduleType == Constant.SCHEDULE_TYPE.EXAM)
                {
                    if (candidateApplication.Status != 2)
                    {
                        throw new Exception("Hồ sơ ứng viên chưa đạt yêu cầu để tiến hành kiểm tra chuyên môn!");
                    }
                    await _jobApplicationRepository.UpdateJobApplicationStatus(candidateApplication.ApplicationId,
                        null, 3);

                    await _emailTemplateRepository.SendEmailExamSchedule(candidateApplication.ApplicationId,
                        scheduleDetail.StartDate.ToString() ??
                        throw new InvalidOperationException("Thời gian phỏng vấn không được để trống!"),
                        scheduleDetail.Location ??
                        throw new InvalidOperationException("Thời gian phỏng vấn không được để trống!"));
                }
                else
                {
                    throw new Exception("Không thể xác định được loại lịch!");
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return scheduleDto;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ScheduleDTO> UpdateSchedules(int id, ScheduleDTO scheduleDto)
    {
        var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var schedule = await _context.Schedules.FindAsync(id);

            if (schedule == null)
            {
                throw new Exception("Không thể tìm thấy thời gian biểu!");
            }

            var hrAccount = await _context.Accounts.Where(p => p.AccountId == scheduleDto.HumanResourceId)
                .FirstOrDefaultAsync();
            if (hrAccount == null)
            {
                throw new Exception("HR không tồn tại!");
            }

            schedule.HumanResourceId = hrAccount.AccountId;
            schedule.Status = scheduleDto.Status;
            schedule.UpdatedTime = DateTime.Now;
            schedule.UpdatedBy = scheduleDto.UpdatedBy;

            _context.Entry(schedule).State = EntityState.Modified;

            foreach (var interviewer in scheduleDto.Interviewers)
            {
                var interviewerAccount = await _context.Accounts.Where(p => p.AccountId == interviewer.InterviewerId)
                    .FirstOrDefaultAsync();
                if (interviewerAccount == null)
                {
                    throw new Exception("Người phỏng vấn không tồn tại!");
                }

                interviewer.ScheduleId = schedule.ScheduleId;
                interviewer.InterviewerId = interviewerAccount.AccountId;
                _context.Entry(interviewer).State = _context.Interviews.Any(i =>
                    i.ScheduleId == interviewer.ScheduleId && i.InterviewerId == interviewerAccount.AccountId)
                    ? EntityState.Modified
                    : EntityState.Added;
            }

            foreach (var scheduleDetail in scheduleDto.ScheduleDetails)
            {
                var candidateApplication = await _context.JobApplications
                    .Include(ja => ja.Profile)
                    .Where(ja => ja.Profile != null && ja.Profile.AccountId == scheduleDetail.CandidateId)
                    .FirstOrDefaultAsync();
                if (candidateApplication == null)
                {
                    throw new Exception("Ứng viên không tồn tại!");
                }

                var scheduleDetailEntity = await _context.ScheduleDetails.Where(sd =>
                        sd.ScheduleId == schedule.ScheduleId && sd.CandidateId == candidateApplication.ApplicationId)
                    .FirstOrDefaultAsync();

                if (scheduleDetailEntity == null)
                {
                    scheduleDetailEntity = new ScheduleDetail
                    {
                        ScheduleId = schedule.ScheduleId,
                        CandidateId = candidateApplication.ApplicationId,
                        Status = scheduleDetail.Status,
                        ScheduleType = scheduleDetail.ScheduleType,
                        StartDate = scheduleDetail.StartDate,
                        EndDate = scheduleDetail.EndDate,
                        Note = scheduleDetail.Note,
                        CreatedTime = DateTime.Now,
                        UpdatedTime = DateTime.Now,
                        CreatedBy = scheduleDto.CreatedBy,
                        UpdatedBy = scheduleDto.UpdatedBy
                    };
                    _context.Entry(scheduleDetailEntity).State = EntityState.Added;
                    if (scheduleDetail.ScheduleType == Constant.SCHEDULE_TYPE.INTERVIEW)
                    {
                        var interviewer = await _context.Interviews
                            .Include(i => i.InterviewerNavigation)
                            .Where(i => i.ScheduleId == scheduleDetailEntity.ScheduleId)
                            .FirstOrDefaultAsync();
                        await _jobApplicationRepository.UpdateJobApplicationStatus(candidateApplication.ApplicationId,
                            interviewer?.InterviewerNavigation?.AccountId, 6);

                        await _emailTemplateRepository.SendEmailInterviewSchedule(candidateApplication.ApplicationId,
                            scheduleDetail.StartDate.ToString() ??
                            throw new InvalidOperationException("Thời gian phỏng vấn không được để trống!"),
                            scheduleDetail.Location ??
                            throw new InvalidOperationException("Địa điểm phỏng vấn không được để trống!"),
                            interviewer?.InterviewerNavigation?.FullName?[..^2]);
                        await _emailTemplateRepository.SendEmailScheduleForInterviewer(
                            candidateApplication.ApplicationId,
                            scheduleDetail.StartDate.ToString() ??
                            throw new InvalidOperationException("Thời gian phỏng vấn không được để trống!"),
                            scheduleDetail.Location);
                    }
                    else if (scheduleDetail.ScheduleType == Constant.SCHEDULE_TYPE.EXAM)
                    {
                        await _jobApplicationRepository.UpdateJobApplicationStatus(candidateApplication.ApplicationId,
                            null, 3);

                        await _emailTemplateRepository.SendEmailExamSchedule(candidateApplication.ApplicationId,
                            scheduleDetail.StartDate.ToString() ??
                            throw new InvalidOperationException("Thời gian phỏng vấn không được để trống!"),
                            scheduleDetail.Location ??
                            throw new InvalidOperationException("Thời gian phỏng vấn không được để trống!"));
                    }
                }
                else
                {
                    scheduleDetailEntity.UpdatedTime = DateTime.Now;
                    scheduleDetailEntity.Status = scheduleDetail.Status;
                    scheduleDetailEntity.ScheduleType = scheduleDetail.ScheduleType;
                    scheduleDetailEntity.StartDate = scheduleDetail.StartDate;
                    scheduleDetailEntity.EndDate = scheduleDetail.EndDate;
                    scheduleDetailEntity.Note = scheduleDetail.Note;
                    scheduleDetailEntity.UpdatedBy = scheduleDetail.UpdatedBy;
                    scheduleDetailEntity.UpdatedTime = DateTime.Now;
                    _context.Entry(scheduleDetail).State = EntityState.Modified;
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return scheduleDto;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteSchedule(int scheduleId)
    {
        var schedule = await _context.Schedules.FindAsync(scheduleId);
        if (schedule == null)
        {
            return false;
        }

        var scheduleDetails = await _context.ScheduleDetails.Where(sd => sd.ScheduleId == scheduleId).ToListAsync();
        var interviewers = await _context.Interviews.Where(i => i.ScheduleId == scheduleId).ToListAsync();
        foreach (var scheduleDetail in scheduleDetails)
        {
            _context.Entry(scheduleDetail).State = EntityState.Deleted;
        }

        foreach (var interviewer in interviewers)
        {
            _context.Entry(interviewer).State = EntityState.Deleted;
        }

        _context.Entry(schedule).State = EntityState.Deleted;
        await _context.SaveChangesAsync();
        return true;
    }
}