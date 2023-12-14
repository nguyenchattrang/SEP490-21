using RecruitXpress_BE.DTO;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.IRepositories;

public interface ICityRepository
{
    Task<CityResponse> GetCities(CityRequest request);
    Task<City> GetCity(int id);
    Task<City> AddCity(City city);
    Task<City> UpdateCity(int id, City city);
    Task<bool> DeleteCity(int id);
}