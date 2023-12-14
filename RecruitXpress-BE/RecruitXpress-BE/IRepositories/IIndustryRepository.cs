using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories;

public interface IIndustryRepository
{
    Task<IndustryResponse> GetIndustries(IndustryRequest request);
    Task<Industry> GetIndustry(int id);
    Task<Industry> AddIndustry(Industry industry);
    Task<Industry> UpdateIndustry(int id, Industry industry);
    Task<bool> DeleteIndustry(int id);
}