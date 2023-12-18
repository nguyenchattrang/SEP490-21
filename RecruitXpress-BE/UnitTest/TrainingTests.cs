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
    public class TrainingControllerTests
    {
        private IMapper _mapper;
        private TrainingController _controller;


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
            _controller = new TrainingController(dbContext, _mapper);

        }

        [TestMethod]
        public async Task GetTraining_ReturnsNotFound_WhenTrainingNotFound()
        {
            // Arrange
            int account = 99;
            // Act
            var result = await _controller.GetTraining( account);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));

        }

        [TestMethod]
        public async Task AddTraining_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var accountId = 1; // Change this value based on your test scenario
            var listTest = new List<training>();
            listTest.Add(new training { ProfileId = 1 });
            // Act
            var result = await _controller.AddTraining(listTest, accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
      
        [TestMethod]
        public async Task AddTrainingByAdmin_ReturnsBadRequest_WhenFail()
        {
            // Arrange
            var accountId = 99; // Change this value based on your test scenario
            var listTest = new List<training>();
            listTest.Add(new training { ProfileId = 1 });
            var result = await _controller.AddTraining(listTest, accountId);
            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
        [TestMethod]
        public async Task AddTraining_ReturnsNotFound_WhenTrainingIsNotFound()
        {
            // Arrange
            var accountId = 22; // Change this value based on your test scenario
        
            // Act
            var result = await _controller.GetTraining(accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }
        [TestMethod]
        public async Task AddTraining_ReturnsNotFound_WhenAccountIsNotFound()
        {
            // Arrange
            var accountId = 99; // Change this value based on your test scenario
            var Training = 1;
            // Act
            var result = await _controller.GetTraining( accountId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }
        [TestMethod]
        public async Task DeleteTraining_ShouldReturnNotFound()
        {
            // Arrange
            
            var Training = 99;
            // Act
            var result = await _controller.DeleteTraining(Training);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }
        [TestMethod]
        public async Task DeleteTraining_ShouldReturnOk()
        {
            // Arrange
            var Training = 1;
            // Act
            var result = await _controller.DeleteTraining(Training);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
       
        [TestMethod]
        public async Task UpdateTraining_ShouldReturnOk()
        {
            // Arrange
            
            var Training = new training { TrainingId =1, SkillsCovered = "C#"};
            var listTest = new List<training>();
            listTest.Add(Training);
            // Act
            var result = await _controller.UpdateTraining(listTest);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
        [TestMethod]
        public async Task UpdateTraining_ShouldReturnBadRequest()
        {
            // Arrange
            var Training = new training { TrainingId = 99, SkillsCovered = "C#" };
            var listTest = new List<training>();
            listTest.Add(Training);
            // Act
            var result = await _controller.UpdateTraining(listTest);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }
       
        // Add similar test methods for other actions (AddTraining, UpdateTraining, DeleteTraining) based on your specific requirements.
    }
}