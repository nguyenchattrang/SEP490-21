using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using RecruitXpress_BE.Helper;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Authorization;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/CV/")]
    [ApiController]

    public class CVController : ControllerBase
    {
        private readonly RecruitXpressContext _context;
        private readonly IConfiguration _configuration;
        public CVController(RecruitXpressContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpPost("accountId")]
        public async Task<IActionResult> addCV(IFormFile fileData, int accountId)
        {
            try
            {
                if (fileData == null)
                {
                    return BadRequest("Vui lòng tải lên file");
                }

                if (Path.GetExtension(fileData.FileName).ToLower() != ".pdf")
                {
                    return BadRequest("Chỉ có thể tải lên file pdf");
                }

                if (accountId != null)
                {
                    var check = await _context.CandidateCvs.SingleOrDefaultAsync(x => x.AccountId == accountId);
                    if (check != null)
                    {
                        await DeleteCVEsxit(accountId);
                    }
                }
                else
                {
                    return BadRequest("Không có account");
                }


                string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Upload\\CandidateCvs"));

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                int Timestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                var fileName = "";
                if (fileData.FileName.Contains(" "))
                {
                    fileName = Timestamp + "_" + TokenHelper.GenerateRandomToken(10) + fileData.FileName.Replace(" ", "_");
                }
                else
                {
                    fileName = Timestamp + "_" + TokenHelper.GenerateRandomToken(10) + fileData.FileName.Replace(" ", "_");
                }
                using (var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    await fileData.CopyToAsync(fileStream);
                }

                var fileMeterial = new CandidateCv()
                {
                    AccountId = accountId,
                    Status = 1,
                    Url = "\\" + fileName,
                    CreatedAt = DateTime.Now,
                    Token = TokenHelper.GenerateRandomToken(64),

                };

                var result = _context.CandidateCvs.Add(fileMeterial);
                await _context.SaveChangesAsync();
                return Ok("Thêm CV thành công ");


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
       
       /* [HttpGet("ViewCV/{jobAppId}")]
        public async Task<IActionResult> ViewCV(int jobAppId)
        {
            var result = await _context.JobApplications.FirstOrDefaultAsync(x => x.ApplicationId == jobAppId);

            if (result == null || result.Token == null)
            {
                return NotFound("Không tìm thấy file");
            }

            string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Upload\\CandidateCvs"));
            var filePath = path + result.Url;

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Không tìm thấy file" + filePath);
            }

            var fileContent = await System.IO.File.ReadAllBytesAsync(filePath);
            var contentType = "application/pdf"; // Set the content type to PDF

            string baseUrl = $"{Request.Scheme}://{Request.Host.Value}";

            *//* string baseUrl = _configuration["Website:HostUrl"];
           *//*
            // Construct the link to the documents endpoint
            string documentsLink = Url.Action("GetAddress", "CV", new { result.Token });

            // Return a JSON object containing the link
            return Ok(baseUrl + documentsLink);
        }*/

        [HttpGet("ViewCV/{accId}")]
        public async Task<IActionResult> ViewCV(int accId)
        {
            var result = await _context.CandidateCvs.FirstOrDefaultAsync(x => x.AccountId == accId);

            if (result == null || result.Token==null)
            {
                return NotFound("Không tìm thấy file");
            }

                string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Upload\\CandidateCvs"));
                var filePath = path + result.Url;

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Không tìm thấy file" + filePath);
            }

            var fileContent = await System.IO.File.ReadAllBytesAsync(filePath);
            var contentType = "application/pdf"; // Set the content type to PDF

            string baseUrl = $"{Request.Scheme}://{Request.Host.Value}";

           /* string baseUrl = _configuration["Website:HostUrl"];
          */ 
            // Construct the link to the documents endpoint
            string documentsLink = Url.Action("GetAddress", "CV", new { result.Token });

            // Return a JSON object containing the link
            return Ok(baseUrl+documentsLink);
        }

        [HttpGet("documents/{token}")]
        public async Task<IActionResult> GetAddress(string token)
        {
            var result = await _context.CandidateCvs.FirstOrDefaultAsync(x => x.Token == token);

            if (result == null)
            {
                return NotFound("Không tìm thấy file");
            }

            string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Upload\\CandidateCvs"));
            var filePath = path + result.Url;

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Không tìm thấy file" + filePath);
            }

            var fileContent = await System.IO.File.ReadAllBytesAsync(filePath);
            var contentType = "application/pdf"; // Set the content type to PDF
            Response.Headers.Add("Content-Disposition", "inline");
            return File(fileContent, contentType);
        }


        [HttpGet("cvtemplateId")]
        public async Task<IActionResult> DownloadCV(int cvId)
        {
            var result = await _context.CandidateCvs.FirstOrDefaultAsync(x => x.TemplateId == cvId);
            if (result == null)
            {
                return NotFound("File not found!");
            }
            string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Upload\\CandidateCvs"));

            var filePath = path + result.Url;

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found!" + filePath);
            }
            var fileContent = System.IO.File.ReadAllBytes(filePath);
            string fileName = Path.GetFileName(filePath);
            var contentType = "application/octet-stream";
            return File(fileContent, contentType, fileName);
        }
        [HttpGet("downloadCVJobapply")]
        public async Task<IActionResult> DownloadCVJobApply(int jobapplyId)
        {
            var result = await _context.JobApplications.FirstOrDefaultAsync(x => x.ApplicationId == jobapplyId);
            if (result == null)
            {
                return NotFound("Ứng viên chưa ứng tuyển ở công việc này");
            }
            string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Upload\\JobApplicationsCV"));

            var filePath = path + result.UrlCandidateCV;

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found!" + filePath);
            }
            var fileContent = System.IO.File.ReadAllBytes(filePath);
            string fileName = Path.GetFileName(filePath);
            var contentType = "application/octet-stream";
            return File(fileContent, contentType, fileName);
        }
        [HttpDelete("cvtemplateId")]
        public async Task<IActionResult> DeleteCV(int cvId)
        {
            try
            {
                var result = await _context.CandidateCvs.FirstOrDefaultAsync(x => x.TemplateId == cvId);
                if (result == null)
                {
                    return NotFound("File not found!");
                }
                string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Upload\\CandidateCvs"));
                var filePath = path + result.Url;
                System.IO.File.Delete(filePath);
                _context.CandidateCvs.Remove(result);
                await _context.SaveChangesAsync();
                return Ok("Xóa thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        private async Task<IActionResult> DeleteCVEsxit(int accountId)
        {
            try
            {
                var result = await _context.CandidateCvs.FirstOrDefaultAsync(x => x.AccountId == accountId);
                if (result == null)
                {
                    return BadRequest("Xoa CV fail");
                }
                string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Upload\\CandidateCvs"));
                var filePath = path + result.Url;
                System.IO.File.Delete(filePath);
                _context.CandidateCvs.Remove(result);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("myCV")]
        private async Task<IActionResult> getMyCV(int accountId)
        {
            if (accountId == null) return BadRequest("AccountId dau ?");

            var cv = await _context.CandidateCvs.FirstOrDefaultAsync(x => x.AccountId == accountId);
            if (cv == null)
            {
                return BadRequest("Khong tim thay CV");
            }
            return Ok(cv);
        }
    }
}
