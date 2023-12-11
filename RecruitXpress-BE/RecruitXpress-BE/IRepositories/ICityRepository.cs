using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories;

public interface ICityRepository
{
    Task<City> AddCity(City city);
    Task<City> UpdateCity(int id, City city);
    Task<bool> DeleteCity(int id);
}