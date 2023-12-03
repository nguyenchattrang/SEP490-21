using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories
{
    public interface ICalendarRepository
    {
        Task<List<Calendar>> GetAll();

        Task<List<CalendarDTO>> GetListByAccountIdDate(int accountId, int? year, int? month, int? day);

        Task<CalendarDTO> GetById(int id);

        Task<CalendarDTO> Create(Calendar calendar);

        Task<CalendarDTO> Update(Calendar calendar);

        Task<int> Delete(int id);
    }
}
