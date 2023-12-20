using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{
    public class ApiResponse<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }

        public ApiResponse(IEnumerable<T> items, int totalCount)
        {
            Items = items;
            TotalCount = totalCount;
        }
        public ApiResponse()
        {
         
        }
    }

    public class CityResponse : ApiResponse<City>{}
    public class IndustryResponse : ApiResponse<Industry>{}
    public class EmploymentTypeResponse : ApiResponse<EmploymentType>{}
}
