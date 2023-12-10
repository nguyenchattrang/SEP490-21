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

    public async Task<Dictionary<int, Dictionary<int, Dictionary<int, List<ScheduleAdditionDataDTO>>>>>
        GetListSchedules(int accountId, DateTime? startDate,
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
                .Include(s => s.SpecializedExam)
                .ThenInclude(se => se.Job)
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
                HumanResourceId = s.HumanResourceId,
                HumanResourceName = s.HumanResource != null ? s.HumanResource.FullName : null,
                SpecializedExamId = s.ExamId,
                SpecializedExam = new SpecializedExamDTO()
                {
                    ExamId = s.SpecializedExam != null ? s.SpecializedExam.ExamId : 0,
                    ExamName = s.SpecializedExam != null ? s.SpecializedExam.ExamName : "",
                    Description = s.SpecializedExam != null ? s.SpecializedExam.Description : null,
                    StartDate = s.SpecializedExam != null ? s.SpecializedExam.StartDate : null,
                    EndDate = s.SpecializedExam != null ? s.SpecializedExam.EndDate : null,
                    CreatedAt = s.SpecializedExam != null ? s.SpecializedExam.CreatedAt : null,
                    CreatedBy = s.SpecializedExam != null ? s.SpecializedExam.CreatedBy : null,
                    Code = s.SpecializedExam != null ? s.SpecializedExam.Code : null,
                    ExpertEmail = s.SpecializedExam != null ? s.SpecializedExam.ExpertEmail : null,
                    JobId = s.SpecializedExam != null ? s.SpecializedExam.JobId : null,
                    Status = s.SpecializedExam != null ? s.SpecializedExam.Status : null,
                    JobTitle = s.SpecializedExam != null
                        ? s.SpecializedExam.Job != null ? s.SpecializedExam.Job.Title : null
                        : null
                },
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
                        CandidateId = sd.Candidate != null
                            ? sd.Candidate.Profile != null ? sd.Candidate.Profile.AccountId : null
                            : null,
                        ApplicationId = sd.CandidateId,
                        CandidateName = sd.Candidate != null
                            ? sd.Candidate.Profile != null
                                ? sd.Candidate.Profile.Account != null
                                    ? sd.Candidate.Profile.Account.FullName
                                    : null
                                : null
                            : null,
                        CandidateEmail = sd.Candidate != null
                            ? sd.Candidate.Profile != null
                                ? sd.Candidate.Profile.Account != null ? sd.Candidate.Profile.Account.Account1 : null
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

            var scheduleAdditionDataResult =
                new Dictionary<int, Dictionary<int, Dictionary<int, List<ScheduleAdditionDataDTO>>>>();
            foreach (var scheduleDto in scheduleDtoResult)
            {
                foreach (var scheduleDtoScheduleDetail in scheduleDto.ScheduleDetails)
                {
                    if (scheduleDtoScheduleDetail.StartDate == null ||
                        scheduleDtoScheduleDetail.EndDate == null) continue;
                    var startDateValue = scheduleDtoScheduleDetail.StartDate.Value;
                    if (!scheduleAdditionDataResult.ContainsKey(startDateValue.Year))
                    {
                        scheduleAdditionDataResult.Add(startDateValue.Year,
                            new Dictionary<int, Dictionary<int, List<ScheduleAdditionDataDTO>>>());
                    }

                    if (!scheduleAdditionDataResult[startDateValue.Year].ContainsKey(startDateValue.Month))
                    {
                        scheduleAdditionDataResult[startDateValue.Year].Add(startDateValue.Month,
                            new Dictionary<int, List<ScheduleAdditionDataDTO>>());
                    }

                    if (!scheduleAdditionDataResult[startDateValue.Year][startDateValue.Month]
                            .ContainsKey(startDateValue.Day))
                    {
                        scheduleAdditionDataResult[startDateValue.Year][startDateValue.Month]
                            .Add(startDateValue.Day, new List<ScheduleAdditionDataDTO>());
                    }

                    var scheduleFilterByTime =
                        scheduleAdditionDataResult[startDateValue.Year][startDateValue.Month][startDateValue.Day]
                            .FirstOrDefault(i =>
                                i.StartTime == scheduleDtoScheduleDetail.StartDate &&
                                i.EndTime == scheduleDtoScheduleDetail.EndDate &&
                                i.type == scheduleDtoScheduleDetail.ScheduleType);
                    if (scheduleFilterByTime != null)
                    {
                        scheduleFilterByTime.Candidates.Add(new CandidateSchedule()
                        {
                            ScheduleDetailId = scheduleDtoScheduleDetail.ScheduleDetailId,
                            CandidateId = scheduleDtoScheduleDetail.CandidateId,
                            ApplicationId = scheduleDtoScheduleDetail.ApplicationId,
                            CandidateName = scheduleDtoScheduleDetail.CandidateName,
                            CandidateEmail = scheduleDtoScheduleDetail.CandidateEmail
                        });
                    }
                    else
                    {
                        scheduleAdditionDataResult[startDateValue.Year][startDateValue.Month][startDateValue.Day].Add(
                            new ScheduleAdditionDataDTO()
                            {
                                Candidates = new List<CandidateSchedule?>()
                                {
                                    new()
                                    {
                                        ScheduleDetailId = scheduleDtoScheduleDetail.ScheduleDetailId,
                                        CandidateId = scheduleDtoScheduleDetail.CandidateId,
                                        ApplicationId = scheduleDtoScheduleDetail.ApplicationId,
                                        CandidateName = scheduleDtoScheduleDetail.CandidateName,
                                        CandidateEmail = scheduleDtoScheduleDetail.CandidateEmail
                                    }
                                },
                                ScheduleId = scheduleDto.ScheduleId,
                                HumanResourceId = scheduleDto.HumanResourceId,
                                HumanResourceName = scheduleDto.HumanResourceName,
                                SpecializedExamId = scheduleDto.SpecializedExamId,
                                SpecializedExam = scheduleDto.SpecializedExam,
                                Interviewers = scheduleDto.Interviewers.Select(i => new InterviewerSchedule()
                                {
                                    InterviewerId = i.InterviewerId,
                                    InterviewerName = i.InterviewerName
                                }).ToList(),
                                type = scheduleDtoScheduleDetail.ScheduleType,
                                StartTime = scheduleDtoScheduleDetail.StartDate,
                                EndTime = scheduleDtoScheduleDetail.EndDate,
                                note = scheduleDtoScheduleDetail.Note,
                                location = scheduleDtoScheduleDetail.Location
                            });
                    }
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
            SpecializedExamId = s.ExamId,
            SpecializedExam = new SpecializedExamDTO()
            {
                ExamId = s.SpecializedExam != null ? s.SpecializedExam.ExamId : 0,
                ExamName = s.SpecializedExam != null ? s.SpecializedExam.ExamName : "",
                Description = s.SpecializedExam != null ? s.SpecializedExam.Description : null,
                StartDate = s.SpecializedExam != null ? s.SpecializedExam.StartDate : null,
                EndDate = s.SpecializedExam != null ? s.SpecializedExam.EndDate : null,
                CreatedAt = s.SpecializedExam != null ? s.SpecializedExam.CreatedAt : null,
                CreatedBy = s.SpecializedExam != null ? s.SpecializedExam.CreatedBy : null,
                Code = s.SpecializedExam != null ? s.SpecializedExam.Code : null,
                ExpertEmail = s.SpecializedExam != null ? s.SpecializedExam.ExpertEmail : null,
                JobId = s.SpecializedExam != null ? s.SpecializedExam.JobId : null,
                Status = s.SpecializedExam != null ? s.SpecializedExam.Status : null,
                JobTitle = s.SpecializedExam != null
                    ? s.SpecializedExam.Job != null ? s.SpecializedExam.Job.Title : null
                    : null
            },
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
                ExamId = scheduleDto.SpecializedExamId,
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

            if (await ValidateSchedule(scheduleDto))
            {
                foreach (var scheduleDetail in scheduleDto.ScheduleDetails)
                {
                    if (scheduleDetail.ScheduleType == Constant.SCHEDULE_TYPE.INTERVIEW)
                    {
                        var interviewerId = scheduleDto.Interviewers.FirstOrDefault()?.InterviewerId ?? null;
                        await _jobApplicationRepository.UpdateJobApplicationStatus((int)scheduleDetail.ApplicationId,
                            interviewerId, 6);

                        await _emailTemplateRepository.SendEmailInterviewSchedule((int)scheduleDetail.ApplicationId,
                            scheduleDetail.StartDate.ToString() ??
                            throw new InvalidOperationException("Thời gian phỏng vấn không được để trống!"),
                            scheduleDetail.Location ??
                            throw new InvalidOperationException("Địa điểm phỏng vấn không được để trống!"),
                            interviewerNames[..^2]);
                        await _emailTemplateRepository.SendEmailScheduleForInterviewer(
                            (int)scheduleDetail.ApplicationId,
                            scheduleDetail.StartDate.ToString() ??
                            throw new InvalidOperationException("Thời gian phỏng vấn không được để trống!"),
                            scheduleDetail.Location);
                    }
                    else if (scheduleDetail.ScheduleType == Constant.SCHEDULE_TYPE.EXAM)
                    {
                        await _jobApplicationRepository.UpdateJobApplicationStatus((int)scheduleDetail.ApplicationId,
                            null, 3);

                        await _emailTemplateRepository.SendEmailExamSchedule((int)scheduleDetail.ApplicationId,
                            scheduleDetail.StartDate.ToString() ??
                            throw new InvalidOperationException("Thời gian làm bài kiểm tra không được để trống!"),
                            scheduleDetail.Location ??
                            throw new InvalidOperationException("Địa điểm kiểm tra không được để trống!"));
                    }
                    else
                    {
                        throw new Exception("Không thể xác định được loại lịch!");
                    }

                    var scheduleDetailEntity = new ScheduleDetail
                    {
                        ScheduleId = schedule.ScheduleId,
                        CandidateId = scheduleDetail.ApplicationId,
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
            schedule.ExamId = scheduleDto.SpecializedExamId;
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

            var interviewerEntity = await _context.Interviews
                .Include(i => i.InterviewerNavigation)
                .Where(i => i.ScheduleId == schedule.ScheduleId)
                .FirstOrDefaultAsync();

            if (await ValidateSchedule(scheduleDto))
            {
                var scheduleCandidates = await _context.ScheduleDetails.Where(sd =>
                        sd.ScheduleId == schedule.ScheduleId).Select(sd => sd.CandidateId).ToListAsync();
                foreach (var scheduleDetail in scheduleDto.ScheduleDetails)
                {
                    var scheduleDetailEntity = await _context.ScheduleDetails.Where(sd =>
                            sd.ScheduleId == schedule.ScheduleId && sd.CandidateId == scheduleDetail.ApplicationId)
                        .FirstOrDefaultAsync();

                    if (scheduleDetailEntity == null)
                    {
                        scheduleDetailEntity = new ScheduleDetail
                        {
                            ScheduleId = schedule.ScheduleId,
                            CandidateId = scheduleDetail.ApplicationId,
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
                            var updateJobAppTask = _jobApplicationRepository.UpdateJobApplicationStatus(
                                (int)scheduleDetail.ApplicationId,
                                interviewerEntity?.InterviewerId, 6);

                            var sendEmailInterviewScheduleTask = _emailTemplateRepository.SendEmailInterviewSchedule(
                                (int)scheduleDetail.ApplicationId,
                                scheduleDetail.StartDate.ToString() ??
                                throw new InvalidOperationException("Thời gian phỏng vấn không được để trống!"),
                                scheduleDetail.Location ??
                                throw new InvalidOperationException("Địa điểm phỏng vấn không được để trống!"),
                                interviewerEntity?.InterviewerNavigation?.FullName);
                            var sendEmailScheduleForInterviewerTask =
                                _emailTemplateRepository.SendEmailScheduleForInterviewer(
                                    (int)scheduleDetail.ApplicationId,
                                    scheduleDetail.StartDate.ToString() ??
                                    throw new InvalidOperationException("Thời gian phỏng vấn không được để trống!"),
                                    scheduleDetail.Location);
                            await Task.WhenAll(updateJobAppTask, sendEmailInterviewScheduleTask,
                                sendEmailScheduleForInterviewerTask);
                        }
                        else if (scheduleDetail.ScheduleType == Constant.SCHEDULE_TYPE.EXAM)
                        {
                            var updateJobAppTask = _jobApplicationRepository.UpdateJobApplicationStatus(
                                (int)scheduleDetail.ApplicationId,
                                null, 3);

                            var sendEmailExamScheduleTask = _emailTemplateRepository.SendEmailExamSchedule(
                                (int)scheduleDetail.ApplicationId,
                                scheduleDetail.StartDate.ToString() ??
                                throw new InvalidOperationException("Thời gian làm bài kiểm tra không được để trống!"),
                                scheduleDetail.Location ??
                                throw new InvalidOperationException("Địa điểm kiểm tra không được để trống!"));
                            await Task.WhenAll(updateJobAppTask, sendEmailExamScheduleTask);
                        }
                        else
                        {
                            throw new Exception("Không thể xác định được loại lịch!");
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
                        _context.Entry(scheduleDetailEntity).State = EntityState.Modified;

                        if (scheduleDetail.ScheduleType == Constant.SCHEDULE_TYPE.INTERVIEW)
                        {
                            var updateJobAppTask = _jobApplicationRepository.UpdateJobApplicationStatus(
                                (int)scheduleDetail.ApplicationId,
                                interviewerEntity?.InterviewerId, 6);

                            var sendEmailUpdateInterviewScheduleToCandidateTask =
                                _emailTemplateRepository.SendEmailUpdateInterviewScheduleToCandidate(
                                    (int)scheduleDetail.ApplicationId,
                                    scheduleDetail.StartDate.ToString() ??
                                    throw new InvalidOperationException("Thời gian phỏng vấn không được để trống!"),
                                    scheduleDetail.Location ??
                                    throw new InvalidOperationException("Địa điểm phỏng vấn không được để trống!")
                                );
                            var sendEmailUpdateScheduleForInterviewerTask =
                                _emailTemplateRepository.SendEmailUpdateScheduleForInterviewer(
                                    (int)scheduleDetail.ApplicationId,
                                    scheduleDetail.StartDate.ToString() ??
                                    throw new InvalidOperationException("Thời gian phỏng vấn không được để trống!"),
                                    scheduleDetail.Location);
                            await Task.WhenAll(updateJobAppTask, sendEmailUpdateInterviewScheduleToCandidateTask,
                                sendEmailUpdateScheduleForInterviewerTask);
                        }
                        else if (scheduleDetail.ScheduleType == Constant.SCHEDULE_TYPE.EXAM)
                        {
                            var updateJobAppTask = _jobApplicationRepository.UpdateJobApplicationStatus(
                                (int)scheduleDetail.ApplicationId,
                                null, 3);

                            var sendEmailUpdateExamScheduleToCandidateTask = _emailTemplateRepository.SendEmailUpdateExamScheduleToCandidate(
                                (int)scheduleDetail.ApplicationId,
                                scheduleDetail.StartDate.ToString() ??
                                throw new InvalidOperationException("Thời gian làm bài kiểm tra không được để trống!"),
                                scheduleDetail.Location ??
                                throw new InvalidOperationException("Địa điểm kiểm tra không được để trống!"));
                            await Task.WhenAll(updateJobAppTask, sendEmailUpdateExamScheduleToCandidateTask);
                        }
                        else
                        {
                            throw new Exception("Không thể xác định được loại lịch!");
                        }

                        scheduleCandidates.Remove(scheduleDetail.ApplicationId);
                    }
                }

                if (scheduleCandidates.Count > 0)
                {
                    foreach (var scheduleCandidate in scheduleCandidates)
                    {
                        if (scheduleCandidate == null) continue;
                        var updateJobAppTask = _jobApplicationRepository.UpdateJobApplicationStatus(
                            (int)scheduleCandidate,
                            null, 5);

                        var sendEmailDeleteInterviewScheduleToCandidateTask =
                            _emailTemplateRepository.SendEmailDeleteInterviewScheduleToCandidate(
                                (int)scheduleCandidate, "chưa nghĩ ra lý do");
                        await Task.WhenAll(updateJobAppTask, sendEmailDeleteInterviewScheduleToCandidateTask);
                    }
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
            if (scheduleDetail.CandidateId == null) continue;

            var sendEmailDeleteInterviewScheduleToCandidateTask =
                _emailTemplateRepository.SendEmailDeleteInterviewScheduleToCandidate(
                    (int)scheduleDetail.CandidateId, "chưa nghĩ ra lý do");
            var sendEmailDeleteScheduleForInterviewerTask =
                _emailTemplateRepository.SendEmailDeleteScheduleForInterviewer(
                    (int)scheduleDetail.CandidateId, "chưa nghĩ ra lý do");
            await Task.WhenAll(sendEmailDeleteInterviewScheduleToCandidateTask, sendEmailDeleteScheduleForInterviewerTask);
            await _jobApplicationRepository.UpdateJobApplicationStatus(
                (int)scheduleDetail.CandidateId,
                null, 5);
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

    private async Task<bool> ValidateSchedule(ScheduleDTO scheduleDto)
    {
        string? candidateScheduleExist = null;
        string? candidateApplicationStatusError = null;

        foreach (var scheduleDetail in scheduleDto.ScheduleDetails)
        {
            var candidateApplication = await _context.JobApplications
                .Where(ja => ja.ApplicationId == scheduleDetail.ApplicationId)
                .FirstOrDefaultAsync();
            if (candidateApplication == null)
            {
                throw new Exception("Không tìm thấy hồ sơ của ứng viên: " + scheduleDetail.CandidateName);
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
                candidateScheduleExist += scheduleDetail.CandidateName + ", ";
            }

            if (scheduleDetail.ScheduleType == Constant.SCHEDULE_TYPE.INTERVIEW)
            {
                if (candidateApplication.Status != 5 || candidateApplication.Status != 6)
                {
                    candidateApplicationStatusError += scheduleDetail.CandidateName + ", ";
                }
            }
            else if (scheduleDetail.ScheduleType == Constant.SCHEDULE_TYPE.EXAM)
            {
                if (candidateApplication.Status != 2 || candidateApplication.Status != 3)
                {
                    candidateApplicationStatusError += scheduleDetail.CandidateName + ", ";
                }
            }
            else
            {
                throw new Exception("Không thể xác định được loại lịch!");
            }
        }

        if (scheduleDto.ScheduleDetails.Count <= 0) return true;
        if (!string.IsNullOrWhiteSpace(candidateScheduleExist))
        {
            throw new Exception(
                "Trạng thái hồ sơ ứng viên chưa đúng để tiến hành tạo lịch " +
                (scheduleDto.ScheduleDetails.First().ScheduleType == Constant.SCHEDULE_TYPE.INTERVIEW
                    ? "phỏng vấn: "
                    : "kiểm tra chuyên môn: ") + candidateScheduleExist[..^2]);
        }

        if (!string.IsNullOrWhiteSpace(candidateApplicationStatusError))
        {
            throw new Exception(
                "Ứng viên đã được tạo lịch" + (scheduleDto.ScheduleDetails.First().ScheduleType ==
                                               Constant.SCHEDULE_TYPE.INTERVIEW
                    ? " phỏng vấn: "
                    : "kiểm tra chuyên môn: ") + candidateApplicationStatusError[..^2]);
        }

        return true;
    }
}