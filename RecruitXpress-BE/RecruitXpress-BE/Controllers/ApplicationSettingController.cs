using Microsoft.AspNetCore.Mvc;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Controllers;

[Route("api/ApplicationSetting")]
[ApiController]
public class ApplicationSettingController : ControllerBase
{
    private readonly ICityRepository _cityRepository;
    private readonly IIndustryRepository _industryRepository;
    private readonly IEmploymentTypeRepository _employmentTypeRepository;

    public ApplicationSettingController(ICityRepository cityRepository, IIndustryRepository industryRepository,
        IEmploymentTypeRepository employmentTypeRepository)
    {
        _cityRepository = cityRepository;
        _industryRepository = industryRepository;
        _employmentTypeRepository = employmentTypeRepository;
    }

    //POST: api/ApplicationSetting
    [HttpGet("City")]
    public async Task<ActionResult<CityResponse>> GetCities([FromQuery] CityRequest request)
    {
        try
        {
            return await _cityRepository.GetCities(request);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //POST: api/ApplicationSetting
    [HttpGet("City/{id:int}")]
    public async Task<ActionResult<City>> GetCity(int id)
    {
        try
        {
            return await _cityRepository.GetCity(id);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //POST: api/ApplicationSetting
    [HttpPost("City")]
    public async Task<ActionResult<City>> AddCity(City city)
    {
        try
        {
            city = await _cityRepository.AddCity(city);
            return Ok(city);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //POST: api/ApplicationSetting
    [HttpPut("City/{id:int}")]
    public async Task<ActionResult<City>> UpdateCity(int id, City city)
    {
        try
        {
            city = await _cityRepository.UpdateCity(id, city);
            return Ok(city);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //POST: api/ApplicationSetting
    [HttpDelete("City/{id:int}")]
    public async Task<IActionResult> DeleteCity(int id)
    {
        try
        {
            await _cityRepository.DeleteCity(id);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //POST: api/ApplicationSetting
    [HttpGet("Industry")]
    public async Task<ActionResult<IndustryResponse>> GetIndustries([FromQuery] IndustryRequest request)
    {
        try
        {
            return await _industryRepository.GetIndustries(request);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //POST: api/ApplicationSetting
    [HttpGet("Industry/{id:int}")]
    public async Task<ActionResult<Industry>> GetIndustry(int id)
    {
        try
        {
            return await _industryRepository.GetIndustry(id);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //POST: api/ApplicationSetting
    [HttpPost("Industry")]
    public async Task<ActionResult<Industry>> AddIndustry(Industry industry)
    {
        try
        {
            industry = await _industryRepository.AddIndustry(industry);
            return Ok(industry);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //POST: api/ApplicationSetting
    [HttpPut("Industry/{id:int}")]
    public async Task<ActionResult<Industry>> UpdateIndustry(int id, Industry industry)
    {
        try
        {
            industry = await _industryRepository.UpdateIndustry(id, industry);
            return Ok(industry);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //POST: api/ApplicationSetting
    [HttpDelete("Industry/{id:int}")]
    public async Task<IActionResult> DeleteIndustry(int id)
    {
        try
        {
            await _industryRepository.DeleteIndustry(id);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //POST: api/ApplicationSetting
    [HttpGet("EmploymentType")]
    public async Task<ActionResult<EmploymentTypeResponse>> GetEmploymentTypes([FromQuery] EmploymentTypeRequest request)
    {
        try
        {
            return await _employmentTypeRepository.GetEmploymentTypes(request);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //POST: api/ApplicationSetting
    [HttpGet("EmploymentType/{id:int}")]
    public async Task<ActionResult<EmploymentType>> GetEmploymentType(int id)
    {
        try
        {
            return await _employmentTypeRepository.GetEmploymentType(id);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //POST: api/ApplicationSetting
    [HttpPost("EmploymentType")]
    public async Task<ActionResult<EmploymentType>> AddCity(EmploymentType employmentType)
    {
        try
        {
            employmentType = await _employmentTypeRepository.AddEmploymentType(employmentType);
            return Ok(employmentType);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //POST: api/ApplicationSetting
    [HttpPut("EmploymentType/{id:int}")]
    public async Task<ActionResult<EmploymentType>> UpdateCity(int id, EmploymentType employmentType)
    {
        try
        {
            employmentType = await _employmentTypeRepository.UpdateEmploymentType(id, employmentType);
            return Ok(employmentType);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //POST: api/ApplicationSetting
    [HttpDelete("EmploymentType/{id:int}")]
    public async Task<ActionResult<City>> AddCity(int id)
    {
        try
        {
            await _employmentTypeRepository.DeleteEmploymentType(id);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}