using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using System.Collections;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace RecruitXpress_BE.Repositories
{
    public class GeneralTestRepository : IGeneralTestRepository
    {
        private readonly RecruitXpressContext _context;
        private readonly IMapper _mapper;
        public GeneralTestRepository(RecruitXpressContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<IEnumerable<GeneralTestDTO>> GetAllGeneralTests(GeneralTestRequest request)
        {
            var query = _context.GeneralTests
                .Include(gt => gt.GeneralTestDetails)
                .Include(gt => gt.CreatedByNavigation)
                .Include(gt => gt.Profile)
                .Where(gt => gt.GeneralTestId == gt.GeneralTestId);

            if (!string.IsNullOrEmpty(request.TestName))
            {
                query = query.Where(gt => gt.TestName.Contains(request.TestName));
            }

            if (!string.IsNullOrEmpty(request.Description))
            {
                query = query.Where(gt => gt.Description.Contains(request.Description));
            }

            if (request.CreatedBy.HasValue)
            {
                query = query.Where(gt => gt.CreatedBy == request.CreatedBy);
            }

            if (request.ProfileId.HasValue)
            {
                query = query.Where(gt => gt.ProfileId == request.ProfileId);
            }

            if (request.CreatedAt.HasValue)
            {
                DateTime requestDate = request.CreatedAt.Value.Date; // Get the date portion of the request's DateTime

                query = query.Where(gt => gt.CreatedAt.Value.Date == requestDate);
            }

            if (!string.IsNullOrEmpty(request.SortBy))
            {
                switch (request.SortBy)
                {
                    case "testName":
                        query = request.OrderByAscending
                            ? query.OrderBy(gt => gt.TestName)
                            : query.OrderByDescending(gt => gt.TestName);
                        break;

                    case "generalTestId":
                        query = request.OrderByAscending
                            ? query.OrderBy(gt => gt.GeneralTestId)
                            : query.OrderByDescending(gt => gt.GeneralTestId);
                        break;

                    case "createAt":
                        query = request.OrderByAscending
                            ? query.OrderBy(gt => gt.CreatedAt)
                            : query.OrderByDescending(gt => gt.CreatedAt);
                        break;

                    default:
                        query = request.OrderByAscending
                            ? query.OrderBy(gt => gt.GeneralTestId)
                            : query.OrderByDescending(gt => gt.GeneralTestId);
                        break;
                }
            }

            var generalTests = await query
                .Skip(request.Offset)
                .Take(request.Limit)
                .ToListAsync();

            var generalTestDTOs = _mapper.Map<List<GeneralTestDTO>>(generalTests);

            return generalTestDTOs;
        }



        public async Task<GeneralTestDTO> GetGeneralTestById(int generalTestId)
        {
            var generalTest = _context.GeneralTests
        .Include(gt => gt.CreatedByNavigation)
        .Include(gt => gt.Profile)
        .Where(gt => gt.GeneralTestId == generalTestId).SingleOrDefault();

            var generalTestDTO = _mapper.Map<GeneralTestDTO>(generalTest);

            return generalTestDTO;

        }

        public async Task CreateGeneralTest(GeneralTest generalTest)
        {
            generalTest.CreatedAt = DateTime.Now; // Set the CreatedAt property to the current date and time
            _context.GeneralTests.Add(generalTest);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGeneralTest(int generalTestId, GeneralTest generalTest)
        {
            if (generalTestId != generalTest.GeneralTestId)
                throw new ArgumentException("GeneralTestId in the URL does not match the provided entity.");

            _context.Entry(generalTest).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteGeneralTest(int generalTestId)
        {
            var generalTestDetails = _context.GeneralTestDetails.Where(d => d.GeneralTestId == generalTestId);

            // Delete GeneralTestDetails first
            _context.GeneralTestDetails.RemoveRange(generalTestDetails);

            var generalTest = await _context.GeneralTests.FindAsync(generalTestId);
            if (generalTest == null)
            {
                return false;
            }

            _context.GeneralTests.Remove(generalTest);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

