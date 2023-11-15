using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using Org.BouncyCastle.Asn1.Ocsp;
using AutoMapper;
using RecruitXpress_BE.DTO;
using System.Runtime.Intrinsics.X86;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/GenerateReport/")]
    [ApiController]

    public class GenerateReportController : ControllerBase
    {
        private readonly RecruitXpressContext _context;
        private readonly IMapper _mapper;

        public GenerateReportController(RecruitXpressContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet("ReportForJob")]
        public async Task<IActionResult> generateReportJob(int month, int year)
        {
            try
            {

                if (month != null && year != null)
                {
                    //so luong job trong he thong
                    var countJobNumber = _context.JobPostings.Count();

                    //so luong job da dong trong he thong
                    var countJobNumberClosed = _context.JobPostings.Where(x => x.Status == 0).Count();

                    //so luong job dang hoat dongtrong he thong
                    var countJobNumberOpen = _context.JobPostings.Where(x => x.Status == 1).Count();

                    //so luong job het han trong he thong
                    var countJobNumberEnded = _context.JobPostings.Where(x => x.Status == 2).Count();

                    //so luong nganh nghe cua job trong he thong
                    var industriesNumber = _context.JobPostings
                     .Select(job => job.Industry).Distinct()
                     .Count();
                    //so luong cong ty cua job trong he thong
                    var companyNumber = _context.JobPostings
                     .Select(job => job.Company).Distinct()
                     .Count();
                    //so luong dia chi cua job trong he thong
                    var locationNumber = _context.JobPostings
                    .Select(job => job.Location).Distinct()
                    .Count();
                    //so luong ung vien trong he thong
                    var candidateNumber = _context.Profiles
                    .Select(c => c.ProfileId).Distinct()
                    .Count();
                    //so luong ung vien da apply
                    var candidateNumberAppliedJob = _context.JobApplications
                    .Select(c => c.ProfileId).Distinct()
                    .Count();
                    //so luong ung vien duoc chap thuan 
                    var acceptedCandidateNumber = _context.JobApplications
                    .Where(x => x.Status == 6).Select(c => c.ProfileId).Distinct()
                    .Count();
                    //so luong ung vien bi tu choi
                    var rejectdCandidateNumber = _context.JobApplications
                    .Where(x => x.Status == 0).Select(c => c.ProfileId).Distinct()
                    .Count();
                    //so luong ung vien dang trong qua trinh apply
                    var onGoingCandidateNumber = _context.JobApplications
                    .Where(x => x.Status != 0 && x.Status != 6).Select(c => c.ProfileId).Distinct()
                    .Count();
                    var responseData = new
                    {
                        jobNumber = countJobNumber,
                        JobNumberClosed = countJobNumberClosed,
                        JobNumberOpen = countJobNumberOpen,
                        JobNumberEnded = countJobNumberEnded,
                        industriesNumber = industriesNumber,
                        locationNumber = locationNumber,
                        companyNumber = companyNumber,
                        candidateNumber = candidateNumber,
                        candidateNumberAppliedJob = candidateNumberAppliedJob,
                        acceptedCandidateNumber = acceptedCandidateNumber,
                        rejectdCandidateNumber = rejectdCandidateNumber,
                        onGoingCandidateNumber = onGoingCandidateNumber

                    };
                    return Ok(responseData);
                }
                else
                {
                    //so luong job trong he thong
                    var countJobNumber = _context.JobPostings.Where(x => x.DatePosted.HasValue && x.DatePosted.Value.Month == month &&
                    x.DatePosted.Value.Year == year).Count();

                    //so luong job da dong trong he thong
                    var countJobNumberClosed = _context.JobPostings.Where(x => x.Status == 0 && x.DatePosted.HasValue && x.DatePosted.Value.Month == month &&
                    x.DatePosted.Value.Year == year).Count();

                    //so luong job dang hoat dongtrong he thong
                    var countJobNumberOpen = _context.JobPostings.Where(x => x.Status == 1 && x.DatePosted.HasValue && x.DatePosted.Value.Month == month &&
                    x.DatePosted.Value.Year == year).Count();

                    //so luong job het han trong he thong
                    var countJobNumberEnded = _context.JobPostings.Where(x => x.Status == 2 && x.DatePosted.HasValue && x.DatePosted.Value.Month == month &&
                    x.DatePosted.Value.Year == year).Count();

                    //so luong nganh nghe cua job trong he thong
                    var industriesNumber = _context.JobPostings
                     .Where(x => x.DatePosted.HasValue && x.DatePosted.Value.Month == month &&
                    x.DatePosted.Value.Year == year).Select(job => job.Industry).Distinct()
                     .Count();
                    //so luong cong ty cua job trong he thong
                    var companyNumber = _context.JobPostings
                     .Where(x => x.DatePosted.HasValue && x.DatePosted.Value.Month == month &&
                    x.DatePosted.Value.Year == year).Select(job => job.Company).Distinct()
                     .Count();
                    //so luong dia chi cua job trong he thong
                    var locationNumber = _context.JobPostings
                    .Where(x => x.DatePosted.HasValue && x.DatePosted.Value.Month == month &&
                    x.DatePosted.Value.Year == year).Select(job => job.Location).Distinct()
                    .Count();
                    //so luong ung vien trong he thong
                    var candidateNumber = _context.Profiles
                    .Select(c => c.ProfileId).Distinct()
                    .Count();
                    //so luong ung vien da apply
                    var candidateNumberAppliedJob = _context.JobApplications
                    .Where(x => x.CreatedAt.HasValue && x.CreatedAt.Value.Month == month &&
                    x.CreatedAt.Value.Year == year).Select(c => c.ProfileId).Distinct()
                    .Count();
                    //so luong ung vien duoc chap thuan 
                    var acceptedCandidateNumber = _context.JobApplications
                     .Where(x => x.CreatedAt.HasValue && x.CreatedAt.Value.Month == month &&
                    x.CreatedAt.Value.Year == year && x.Status == 6).Select(c => c.ProfileId).Distinct()
                    .Count();
                    //so luong ung vien bi tu choi
                    var rejectdCandidateNumber = _context.JobApplications
                     .Where(x => x.CreatedAt.HasValue && x.CreatedAt.Value.Month == month &&
                    x.CreatedAt.Value.Year == year &&x.Status == 0).Select(c => c.ProfileId).Distinct()
                    .Count();
                    //so luong ung vien dang trong qua trinh apply
                    var onGoingCandidateNumber = _context.JobApplications
                    .Where(x => x.Status != 0 && x.Status != 6 && x.CreatedAt.HasValue && x.CreatedAt.Value.Month == month &&
                    x.CreatedAt.Value.Year == year).Select(c => c.ProfileId).Distinct()
                    .Count();
                    var responseData = new
                    {
                        jobNumber = countJobNumber,
                        JobNumberClosed = countJobNumberClosed,
                        JobNumberOpen = countJobNumberOpen,
                        JobNumberEnded = countJobNumberEnded,
                        industriesNumber = industriesNumber,
                        locationNumber = locationNumber,
                        companyNumber = companyNumber,
                        candidateNumber = candidateNumber,
                        candidateNumberAppliedJob = candidateNumberAppliedJob,
                        acceptedCandidateNumber = acceptedCandidateNumber,
                        rejectdCandidateNumber = rejectdCandidateNumber,
                        onGoingCandidateNumber = onGoingCandidateNumber

                    };
                    return Ok(responseData);
                }
                

               

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ReportForEvaluate")]
        public async Task<IActionResult> generateReportEvaluate(int profileId)
        {
            try
            {
                if(profileId == null)
                {
                    return BadRequest("Khong co du lieu");
                }

                var allEvaluate = await _context.Evaluates.Where(x=> x.ProfileId== profileId && x.Status == 1).ToListAsync();

                //evaluate co diem >=5
                var allEvaluateGood = _context.Evaluates.Where(x => x.ProfileId == profileId && x.Status == 1 && x.Mark >=5).Count();

                //evaluate co diem <5
                var totalEvaluateNotGood = _context.Evaluates.Where(x => x.ProfileId == profileId && x.Status == 1 && x.Mark < 5).Count();

                var allPoint = await _context.Evaluates.Where(x => x.ProfileId == profileId && x.Status == 1).Select(x=> x.Mark).ToArrayAsync();
                var avgPoint = allPoint.Average();
                var responseData = new
                {
                    allEvaluate = allEvaluate,
                    totalEvaluate = allEvaluate.Count,
                    avgPoint = avgPoint,
                    totalGoodEvaluate = allEvaluateGood,
                    totalEvaluateNotGood = totalEvaluateNotGood

                };

                return Ok(responseData);

            }
            catch (Exception ex)
            {
               return BadRequest(ex.Message);
            }
        }
    }
}
