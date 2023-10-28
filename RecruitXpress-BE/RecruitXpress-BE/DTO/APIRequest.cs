namespace RecruitXpress_BE.DTO
{
    public class ApiRequest<T>
    {
        public int Limit { get; set; } = 20; // Number of items to retrieve per page
        public int Offset { get; set; } = 0; // Number of items to skip
        public string SortOrder { get; set; } // Field to sort by
        public string SearchAll { get; set; } // Search het

        public Dictionary<string, Filter> Filters { get; set; } = new Dictionary<string, Filter>();

        public class Filter
        {
            public string Operator { get; set; } // Filter operator (eq, lte, gte, etc.)
            public string Value { get; set; } // Filter value
        }
    }

    public class QuestionRequest : ApiRequest<QuestionDTO>
    {
        public int QuestionId { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public int? CreatedBy { get; set; }
        public int? Status { get; set; }

    }

}
