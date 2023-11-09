using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories;

public interface IScheduleRepository
{
    Task<List<Schedule>> GetListSchedules();
    Task<List<Schedule>> GetListSchedules(string? searchString, string? orderBy, bool? isSortAscending, int? accountId, int? page, int? size);
    Task<List<Schedule>> GetListScheduleAdvancedSearch(Schedule schedule, int? accountId, int page, int size);
    Task<Schedule?> GetSchedule(int id);
    Task<Schedule> AddSchedule(Schedule Schedule);
    Task<Schedule> UpdateSchedules(int id, Schedule Schedule);
    Task<bool> DeleteSchedule(int jobId);
}