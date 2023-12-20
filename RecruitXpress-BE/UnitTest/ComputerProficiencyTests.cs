using AutoMapper;
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
    public class ComputerProficiencyControllerTests
    {
        private IMapper _mapper;
        private ComputerProficiencyController _controller;


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
            _controller = new ComputerProficiencyController(dbContext, _mapper);

        }

        // Add more test cases as needed to cover different scenarios

        [TestMethod]
        public async Task GetComputerProficiency_ValidAccountId_ReturnsOkWithResults()
        {
            // Arrange
            int validAccountId = 1; // Provide a valid account ID for testing

            // Act
            var result = await _controller.GetComputerProficiency(validAccountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // You can add more assertions based on the expected behavior for valid input
        }

        [TestMethod]
        public async Task AddComputerProficiency_ValidData_ReturnsOk()
        {
            // Arrange
            List<ComputerProficiency> validData = new List<ComputerProficiency>
    {
        new ComputerProficiency { /* Provide valid properties for testing */ }
    };
            int accountId = 1; // Provide a valid account ID for testing

            // Act
            var result = await _controller.AddComputerProficiency(validData, accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // You can add more assertions based on the expected behavior for valid input
        }

        [TestMethod]
        public async Task UpdateComputerProficiency_ValidData_ReturnsOk()
        {
            // Arrange
            List<ComputerProficiency> validData = new List<ComputerProficiency>
    {
        new ComputerProficiency { /* Provide valid properties for testing */ }
    };

            // Act
            var result = await _controller.UpdateComputerProficiency(validData);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // You can add more assertions based on the expected behavior for valid input
        }

        [TestMethod]
        public async Task DeleteComputerProficiency_ValidId_ReturnsOk()
        {
            // Arrange
            int validId = 1; // Provide a valid ComputerProficiencyId for testing

            // Act
            var result = await _controller.DeleteComputerProficiency(validId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            // You can add more assertions based on the expected behavior for valid input
        }


        // Add similar test methods for other actions (AddComputerProficiency, UpdateComputerProficiency, DeleteComputerProficiency) based on your specific requirements.
    }
}