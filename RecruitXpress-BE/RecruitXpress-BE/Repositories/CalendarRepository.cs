﻿using AutoMapper;
using AutoMapper.Internal;
using Google.Apis.Util;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Helper;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using System.Linq;

namespace RecruitXpress_BE.Repositories
{
    public class CalendarRepository:ICalendarRepository
    { 
        private readonly RecruitXpressContext _context;
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;

        private readonly IMapper _mapper;
        public CalendarRepository(RecruitXpressContext context, IMapper mapper, IEmailTemplateRepository emailTemplateRepository, IJobApplicationRepository jobApplicationRepository)
        {
            _context = context;
            _mapper = mapper;
            _emailTemplateRepository = emailTemplateRepository;
            _jobApplicationRepository = jobApplicationRepository;
        }


        public async Task<List<Calendar>> GetAll()
        {
            return await _context.Calendars.ToListAsync();
        }

        public async Task<List<CalendarDTO>> GetListByAccountIdDate(int accountId, int? year, int? month, int? day)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == accountId);
            if(account==null)
            {
                throw new ArgumentException("Không tìm thấy tài khoản");
            }

            var query = _context.Calendars
                .Include(q=> q.Job)
                .Include(q=> q.CandidateNavigation)
                .Include(q=> q.CreatedByNavigation)
                .Include(q=> q.InterviewerNavigation)
                .AsQueryable();
            if (account.RoleId == Constant.ROLE.INTERVIEWER)
                query = query.Where(a => a.Interviewer == account.AccountId);
            if (account.RoleId == Constant.ROLE.CANDIDATE)
                query = query.Where(a => a.Candidate == account.AccountId);
            if(year!=null)
                query = query.Where(a => a.StartDate.Value.Year == year);
            if (month != null)
                query = query.Where(a => a.StartDate.Value.Month == month);
            if (day != null)
                query = query.Where(a => a.StartDate.Value.Day == day);
            var calendars = await query.ToListAsync();
            var calendarDTOs = _mapper.Map<List<CalendarDTO>>(calendars);
            return calendarDTOs;
        }

        public async Task<CalendarDTO> GetById(int id)
        {
            var calendar = await _context.Calendars.FindAsync(id);
            var calendarDTO = _mapper.Map<CalendarDTO>(calendar);
            return calendarDTO;
        }

        public async Task<CalendarDTO> Create(Calendar calendar)
        {
            if (calendar.Candidate == null || calendar.Candidate == 0)
            {
                throw new ArgumentException("Candidate là bắt buộc");
            }
            if (calendar.JobId == null || calendar.JobId == 0)
            {
                throw new ArgumentException("Job là bắt buộc");
            }

            var profile = await _context.Profiles
                .Where(p => p.AccountId == calendar.Candidate)
                .FirstOrDefaultAsync();

            if (profile == null)
            {
                throw new ArgumentException("Không tìm thấy hồ sơ ứng viên");
            }

            var jobApp = await _context.JobApplications
                .Where(j => j.JobId == calendar.JobId && j.ProfileId == profile.ProfileId)
                .FirstOrDefaultAsync();

            if (jobApp == null)
            {
                throw new ArgumentException("Ứng viên chưa đăng kí công việc này");
            }

            if (calendar.StartDate == null)
            {
                throw new ArgumentException("Không được để trống thời gian");
            }

            string time = "";
            if (calendar.EndDate != null)
            {
                time = "Từ " + (calendar.StartDate?.ToString("dd/MM/yyyy lúc HH:mm:ss") ?? "N/A") +
                       " tới " + (calendar.EndDate?.ToString("dd/MM/yyyy lúc HH:mm:ss") ?? "N/A");
            }

            if (calendar.EndDate == null)
            {
                time = "Vào " + (calendar.StartDate?.ToString("dd/MM/yyyy lúc HH:mm:ss") ?? "N/A");
            }

            switch (calendar.Type)
            {
                case 1:
                    if (jobApp.Status != 5)
                    {
                        throw new ArgumentException("Trạng thái của hồ sơ chưa đúng để có thể tạo mới lịch phỏng vấn");
                    }
                    else
                    {
                        await _emailTemplateRepository.SendEmailInterviewSchedule(jobApp.ApplicationId, time, calendar.Location, null);
                        await _emailTemplateRepository.SendEmailScheduleForInterviewer(jobApp.ApplicationId, time, calendar.Location);
                        await _jobApplicationRepository.UpdateJobApplicationStatus(jobApp.ApplicationId, null, 6);
                    }
                    break;
                case 2:
                    if (jobApp.Status != 2)
                    {
                        throw new ArgumentException("Trạng thái của hồ sơ chưa đúng để có thể tạo mới lịch thi");
                    }
                    else
                    {
                        await _emailTemplateRepository.SendEmailExamSchedule(jobApp.ApplicationId, time, calendar.Location);
                        await _jobApplicationRepository.UpdateJobApplicationStatus(jobApp.ApplicationId, null, 3);
                    }
                    break;
                default:
                    throw new ArgumentException("Không thể xác minh được đây là loại lịch nào");
                    break;
            }

            calendar.JobApplicationId = jobApp.ApplicationId;
            calendar.CreatedAt = DateTime.Now;
            _context.Calendars.Add(calendar);
            await _context.SaveChangesAsync();

            var calendarDTO = _mapper.Map<CalendarDTO>(calendar);
            return calendarDTO;
        }

        public async Task<List<CalendarDTO>> CreateMultipleCandidates(List<int> candidateIds, CalendarTemp calendar)
        {
            using var transaction = _context.Database.BeginTransaction();
            var errorMessages = new List<string>();
            var calendarDTOs = new List<CalendarDTO>();

            if (candidateIds == null || candidateIds.Count == 0)
            {
                throw new ArgumentException("Danh sách ứng viên là bắt buộc");
            }

            if (calendar.JobId == null || calendar.JobId == 0)
            {
                throw new ArgumentException("Job là bắt buộc");
            }

            if (calendar.StartDate == null)
            {
                throw new ArgumentException("Không được để trống thời gian");
            }

            string time = "";
            if (calendar.EndDate != null)
            {
                time = "Từ " + (calendar.StartDate?.ToString("dd/MM/yyyy lúc HH:mm:ss") ?? "N/A") +
                       " tới " + (calendar.EndDate?.ToString("dd/MM/yyyy lúc HH:mm:ss") ?? "N/A");
            }

            if (calendar.EndDate == null)
            {
                time = "Vào " + (calendar.StartDate?.ToString("dd/MM/yyyy lúc HH:mm:ss") ?? "N/A");
            }

            foreach (var candidateId in candidateIds)
            {
                var account = await _context.Accounts.Where(a => a.AccountId == candidateId).FirstOrDefaultAsync();
                if (account == null)
                    throw new ArgumentException("Không tìm thấy tài khoản ứng viên");
                var profile = await _context.Profiles
                    .Where(p => p.AccountId == candidateId)
                    .FirstOrDefaultAsync();

                if (profile == null)
                {
                    errorMessages.Add($"Không tìm thấy hồ sơ ứng viên có ID {candidateId}");
                    continue; 
                }

                var jobApp = await _context.JobApplications
                    .Where(j => j.JobId == calendar.JobId && j.ProfileId == profile.ProfileId)
                    .FirstOrDefaultAsync();

                if (jobApp == null)
                {
                    errorMessages.Add($"Ứng viên có ID {candidateId} chưa đăng kí công việc này");
                }


                switch (calendar.Type)
                {
                    case 1:
                        if (jobApp.Status != 5)
                        {
                            errorMessages.Add($"Trạng thái hồ sơ của ứng viên có tên {account.FullName} chưa đúng để có thể tạo mới lịch phỏng vấn");
                        }
                        break;
                    case 2:
                        if (jobApp.Status != 2)
                        {
                            errorMessages.Add($"Trạng thái hồ sơ của ứng viên có tên {account.FullName} chưa đúng để có thể tạo mới lịch thi");
                        }  
                        break;
                    default:
                        throw new ArgumentException("Không thể xác minh được đây là loại lịch nào");
                        break;
                }

                var newCalendar = _mapper.Map<Calendar>(calendar);
                newCalendar.JobApplicationId = jobApp.ApplicationId;
                newCalendar.CreatedAt = DateTime.Now;
                newCalendar.Candidate = candidateId;
                _context.Calendars.Add(newCalendar);
                await _context.SaveChangesAsync();
                var calendarDTO = _mapper.Map<CalendarDTO>(newCalendar);
                calendarDTOs.Add(calendarDTO);
            }

            if (errorMessages.Count > 0)
            {
                // Combine error messages and throw an exception
                var errorMessage = string.Join(Environment.NewLine, errorMessages);
                throw new ArgumentException(errorMessage);
            }
            foreach (var candidateId in candidateIds)
            {
                var profile = await _context.Profiles
                    .Where(p => p.AccountId == candidateId)
                    .FirstOrDefaultAsync();

                if (profile == null)
                {
                    throw new ArgumentException($"Không tìm thấy hồ sơ ứng viên có ID {candidateId}");
                }

                var jobApp = await _context.JobApplications
                    .Where(j => j.JobId == calendar.JobId && j.ProfileId == profile.ProfileId)
                    .FirstOrDefaultAsync();

                switch (calendar.Type)
                {
                    case 1:
                        if (jobApp.Status != 5)
                        {
                            throw new ArgumentException("Trạng thái của hồ sơ chưa đúng để có thể tạo mới lịch phỏng vấn");
                        }
                        else
                        {
                            await _emailTemplateRepository.SendEmailInterviewSchedule(jobApp.ApplicationId, time, calendar.Location, null);
                            await _emailTemplateRepository.SendEmailScheduleForInterviewer(jobApp.ApplicationId, time, calendar.Location);
                            await _jobApplicationRepository.UpdateJobApplicationStatus(jobApp.ApplicationId, null, 6);
                        }
                        break;
                    case 2:
                        if (jobApp.Status != 2)
                        {
                            throw new ArgumentException("Trạng thái của hồ sơ chưa đúng để có thể tạo mới lịch thi");
                        }
                        else
                        {
                            await _emailTemplateRepository.SendEmailExamSchedule(jobApp.ApplicationId, time, calendar.Location);
                            await _jobApplicationRepository.UpdateJobApplicationStatus(jobApp.ApplicationId, null, 3);
                        }
                        break;
                    default:
                        throw new ArgumentException("Không thể xác minh được đây là loại lịch nào");
                        break;
                }
            }



            transaction.Commit(); 

            return calendarDTOs;
        }



        public async Task<CalendarDTO> Update(Calendar calendar)
        {
            _context.Entry(calendar).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            var calendarDTO = _mapper.Map<CalendarDTO>(calendar);
            return calendarDTO;
        }

        public async Task<int> Delete(int id)
        {
            var calendar = await _context.Calendars.FindAsync(id);
            if (calendar == null)
            {
                return 0;
            }

            _context.Calendars.Remove(calendar);
            return await _context.SaveChangesAsync();
        }
    }
}
