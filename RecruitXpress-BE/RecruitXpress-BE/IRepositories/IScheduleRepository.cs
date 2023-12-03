using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories;

public interface IScheduleRepository
{
    Task<List<Schedule>>? GetListSchedules();
    Task<ScheduleResponse> GetListSchedules(int accountId, DateTime? startDate, DateTime? endDate);
    Task<ScheduleDTO?> GetSchedule(int id);
    Task<ScheduleDTO> AddSchedule(ScheduleDTO schedule);
    Task<ScheduleDTO> UpdateSchedules(int id, ScheduleDTO scheduleDto);
    Task<bool> DeleteSchedule(int scheduleId);
}