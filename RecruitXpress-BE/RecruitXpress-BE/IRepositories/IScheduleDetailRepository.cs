using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories;

public interface IScheduleDetailRepository
{
    Task<ScheduleDetail> GetScheduleDetails(int scheduleId);
    Task<ScheduleDetail> GetScheduleDetail(ScheduleDetail scheduleDetail);
    Task<ScheduleDetail> CreateScheduleDetail(ScheduleDetail scheduleDetail);
    Task<ScheduleDetail> UpdateScheduleDetail(int id, ScheduleDetail scheduleDetail);
    Task<bool> DeleteScheduleDetail(int scheduleDetailId);
}