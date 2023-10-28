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

}
