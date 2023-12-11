using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories;

public interface IIndustryRepository
{
    Task<Industry> AddIndustry(Industry industry);
    Task<Industry> UpdateIndustry(int id, Industry industry);
    Task<bool> DeleteIndustry(int id);
}