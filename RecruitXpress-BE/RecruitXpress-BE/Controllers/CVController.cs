using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.IRepository;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repository;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;


namespace RecruitXpress_BE.Controllers
{
    [Route("api/CV/")]
    [ApiController]

    public class CVController : ControllerBase
    {
        private readonly RecruitXpressContext _context;

        public CVController(RecruitXpressContext context)
        {
            _context = context;
        }
        [HttpPost("accountId")]
        public async Task<IActionResult> addCV(IFormFile fileData, int accountId)
        {
            try
            {
                if(fileData == null)
                {
                    return BadRequest("Please upload file");
                }
                string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "Upload\\CVTemplates"));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                int Timestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                var fileName = "";
                if(fileData.FileName.Contains(" "))
                {
                     fileName = Timestamp + "_"  + accountId + fileData.FileName.Replace(" ", "_");
                }
                else
                {
                    fileName = Timestamp + "_"+ accountId + fileData.FileName ;
                }
                using (var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    await fileData.CopyToAsync(fileStream);
                }
               
                var fileMeterial = new Cvtemplate()
                {
                    AccountId = accountId,
                    Status = 1,
                    Url = path + "\\" + fileName,
                    CreatedAt = DateTime.Now,

                };
                using (var context = new RecruitXpressContext())
                {
                    var result = context.Cvtemplates.Add(fileMeterial);
                    await context.SaveChangesAsync();
                    return Ok("Thêm CV thành công ");
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet("cvtemplate")]
        public IActionResult DownloadMaterial(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "CVTemplates", fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found!" + fileName);
            }
            var fileContent = System.IO.File.ReadAllBytes(filePath);
            var contentType = "application/octet-stream";
            return File(fileContent, contentType, fileName);
        }
        [HttpDelete("cvtemplateId")]
        public async Task<IActionResult> DeleteCVAsync(int cvId)
        {
            try
            {
                 var result = await _context.Cvtemplates.FirstOrDefaultAsync(x => x.TemplateId == cvId);
                if(result == null)
                {
                    return NotFound("File not found!");
                }
                var filePath = result.Url;
                System.IO.File.Delete(filePath);
                _context.Cvtemplates.Remove(result);
                await _context.SaveChangesAsync();
                return Ok("Xóa thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
    }
}
