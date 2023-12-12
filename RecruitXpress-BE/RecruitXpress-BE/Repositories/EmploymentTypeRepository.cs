using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Repositories;

public class EmploymentTypeRepository : IEmploymentTypeRepository
{
    private readonly RecruitXpressContext _context;

    public EmploymentTypeRepository(RecruitXpressContext context)
    {
        _context = context;
    }
    
    public async Task<EmploymentType> AddEmploymentType(EmploymentType employmentType)
    {
        try
        {
            _context.Entry(employmentType).State = EntityState.Added;
            await _context.SaveChangesAsync();
            return employmentType;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<EmploymentType> UpdateEmploymentType(int id, EmploymentType employmentType)
    {
        try
        {
            employmentType.EmploymentTypeId = id;
            if (!_context.EmploymentTypes.Any(i => i.EmploymentTypeId == id))
            {
                throw new Exception("Không tìm thấy thông tin loại công việc");
            }
            _context.Entry(employmentType).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return employmentType;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteEmploymentType(int id)
    {
        try
        {
            var employmentType = _context.EmploymentTypes.FirstOrDefault(i => i.EmploymentTypeId == id);
            if (employmentType == null)
            {
                throw new Exception("Không tìm thấy thông tin loại công việc");
            }
            _context.Entry(employmentType).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}