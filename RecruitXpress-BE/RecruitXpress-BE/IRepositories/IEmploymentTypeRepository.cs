using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories;

public interface IEmploymentTypeRepository
{
    Task<EmploymentType> AddEmploymentType(EmploymentType employmentType);
    Task<EmploymentType> UpdateEmploymentType(int id, EmploymentType employmentType);
    Task<bool> DeleteEmploymentType(int id);
}