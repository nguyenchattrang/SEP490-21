using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using RecruitXpress_BE.Contracts;
using RecruitXpress_BE.Controllers;
using RecruitXpress_BE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Profile = RecruitXpress_BE.Models.Profile;

namespace UnitTest
{
    [TestClass]
    public class ShortListingControllerTests
    {
        private IMapper _mapper;
        private ShortListingController _controller;


        [TestInitialize]
        public void Setup()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var options = new DbContextOptionsBuilder<RecruitXpressContext>()
     .UseSqlServer(config.GetConnectionString("RecruitXpress")) // Use the appropriate provider (e.g., UseSqlServer for SQL Server)
     .Options;

            // Create an instance of the context using the actual database
            var dbContext = new RecruitXpressContext(options);

            // Configure AutoMapper with your mapping profile
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            // Create an instance of the mapper
            _mapper = mappingConfig.CreateMapper();

            // Create an instance of your controller with the mock context and mapper
            _controller = new ShortListingController(dbContext, _mapper);

        }
        [TestMethod]
        public async Task AddToShortListing_WithValidJobApplyId_ShouldToggleShortedFlagAndReturnOk()
        {
            // Arrange
            var jobApplyId = 1;

            // Act
            var result = await _controller.AddToShortListing(jobApplyId);

            // Assert
          
            Assert.AreEqual(result, typeof (OkObjectResult));

            // Additional assertions based on your specific logic
        }
        [TestMethod]
        public async Task AddToShortListing_WithInValidJobApplyId_ReturnBadRequest()
        {
            // Arrange
            var jobApplyId = -1;

            // Act
            var result = await _controller.AddToShortListing(jobApplyId);

            // Assert

            Assert.AreEqual(result, typeof(BadRequestObjectResult));

        }

        // Add similar test methods for other actions (AddMaritalStatus, UpdateMaritalStatus, DeleteMaritalStatus) based on your specific requirements.
    }
}