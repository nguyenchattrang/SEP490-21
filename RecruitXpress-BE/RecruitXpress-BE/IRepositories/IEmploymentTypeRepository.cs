using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories;

public interface IEmploymentTypeRepository
{
    Task<EmploymentTypeResponse> GetEmploymentTypes(EmploymentTypeRequest request);
    Task<EmploymentType> GetEmploymentType(int id);
    Task<EmploymentType> AddEmploymentType(EmploymentType employmentType);
    Task<EmploymentType> UpdateEmploymentType(int id, EmploymentType employmentType);
    Task<bool> DeleteEmploymentType(int id);
}