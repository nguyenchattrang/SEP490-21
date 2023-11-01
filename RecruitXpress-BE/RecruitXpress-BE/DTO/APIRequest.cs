using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{
    public class ApiRequest<T>
    {
        public int Limit { get; set; } = 20; // Number of items to retrieve per page
        public int Offset { get; set; } = 0; // Number of items to skip
        public string SortBy { get; set; } // Field to sort by
        private bool? _orderByAscending;

        public bool OrderByAscending
        {
            get => _orderByAscending ?? false;
            set => _orderByAscending = value;
        }
        public string SearchAll { get; set; } // Search het

 
    }

    public class QuestionRequest : ApiRequest<QuestionDTO>
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public int? CreatedBy { get; set; }
        public int? Status { get; set; }

    }
    public class JobApplicationRequest : ApiRequest<JobApplicationDTO>
    {
        public string? SearchString { get; set; }
        public string? Location { get; set; }
        public string? EmploymentType { get; set; }
        public string? Industry { get; set; }
        public string? SalaryRange { get; set; }
        public string? NameCandidate { get; set; }
        public string? PhoneCandidate { get; set; }
        public string? EmailCandidate { get; set; }
        public DateTime? ApplicationDeadline { get; set; }

    }

    public class GeneralTestRequest : ApiRequest<GeneralTest>
    {
        public string? TestName { get; set; }
        public string? Description { get; set; }
        public int? CreatedBy { get; set; }
        public int? ProfileId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
