﻿using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.DTO
{
    public class ApiRequest<T>
    {
        public int Page { get; set; } = 0; 
        public int Size { get; set; } = 20; 
        public string SortBy { get; set; } // Field to sort by
        private bool? _orderByAscending;
        public bool OrderByAscending
        {
            get => _orderByAscending ?? false;
            set => _orderByAscending = value;
        }
        public string SearchAll { get; set; } // Search het
    }
    public class ShortListingRequest : ApiRequest<ShortListingDTO>
    {
        public string? SearchString { get; set; }
        public string? JobTitile { get; set; }
        public string? NameCandidate { get; set; }
        public string? PhoneCandidate { get; set; }
        public string? EmailCandidate { get; set; }
        public string? Company { get; set; }

        public int? Status { get; set; }

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
        public int? LocationId { get; set; }
        public int? EmploymentTypeId { get; set; }
        public int? IndustryId { get; set; }
        public long? MinSalary { get; set; }
        public long? MaxSalary { get; set; }
        public string? NameCandidate { get; set; }
        public string? PhoneCandidate { get; set; }
        public string? EmailCandidate { get; set; }
        public string? Company { get; set; }
        public string? Title { get; set; }
        public int? Status { get; set; }
        public int? Shorted { get; set; }
        public DateTime? ApplicationDeadline { get; set; }
        public DateTime? DatePosted { get; set; }

    }

    public class GeneralTestRequest : ApiRequest<GeneralTest>
    {
        public string? TestName { get; set; }
        public string? Description { get; set; }
        public int? CreatedBy { get; set; }
        public int? ProfileId { get; set; }
        public int? Score { get; set; }
        public int? TimeCount { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class SpecializedExamRequest : ApiRequest<SpecializedExam>
    {
        public string? ExamName { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public int? Status { get; set; }
        public string? Code { get; set; }
        public string? ExpertEmail { get; set; }
        public int? JobId { get; set; }

    }

    public class ExamRequest : ApiRequest<Exam>
    {
        public int? AccountId { get; set; }
        public string? FileUrl { get; set; }
        public DateTime? TestDate { get; set; }
        public DateTime? TestTime { get; set; }
        public string? Point { get; set; }
        public string? Comment { get; set; }
        public string? MarkedBy { get; set; }
        public DateTime? MarkedDate { get; set; }
        public int? Status { get; set; }

    }
    public class EmailTemplateRequest : ApiRequest<SpecializedExam>
    {
        public string? Title { get; set; }
        public string? Header { get; set; }
        public string? Body { get; set; }
        public string? SendTo { get; set; }
        public int? CreatedBy { get; set; }
        public int? MailType { get; set; }
        public int? Status { get; set; }

    }

    public class GetListActivityRequest : ApiRequest<Account>
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public int? Type { get; set; }

    }

    public class GetListEvaluateRequest : ApiRequest<Evaluate>
    {
        public int JobApplicationId { get; set; }
        public int? CalendarId { get; set; }
        public int? ProfileId { get; set; }
        public string? Comments { get; set; }
        public string? Strengths { get; set; }
        public string? Weaknesses { get; set; }
        public double? Score { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? Status { get; set; }

    }

    public class CalendarMultipleCandidatesRequest
    {
        public List<int> CandidateIds { get; set; }
        public CalendarTemp Calendar { get; set; }
    }

    public class CityRequest : ApiRequest<City>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
    
    public class IndustryRequest : ApiRequest<Industry>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
    
    public class EmploymentTypeRequest : ApiRequest<EmploymentType>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
