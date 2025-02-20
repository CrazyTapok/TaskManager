using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TaskManager.API.Contracts.Request;
using TaskManager.API.Contracts.Response;
using TaskManager.API.Controllers;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.API.Tests.Controllers
{
    public class TaskControllerTests
    {
        private readonly Mock<ITaskService> _mockTaskService;
        private readonly TaskController _controller;
        private readonly Fixture _fixture;
        private readonly CancellationToken _cancellationToken;

        public TaskControllerTests()
        {
            _mockTaskService = new Mock<ITaskService>();
            _controller = new TaskController(_mockTaskService.Object);
            _fixture = new Fixture();
            _cancellationToken = new CancellationToken();

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsOkResult_WhenTaskExists()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = _fixture.Build<Core.Models.Task>().With(task => task.Id, taskId).Create();
            _mockTaskService.Setup(service => service.GetByIdAsync(taskId, _cancellationToken))
                            .ReturnsAsync(task);

            // Act
            var result = await _controller.GetByIdAsync(taskId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var taskResponse = Assert.IsType<TaskResponse>(okResult.Value);
            Assert.Equal(taskId, taskResponse.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNotFoundResult_WhenTaskDoesNotExist()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _mockTaskService.Setup(service => service.GetByIdAsync(taskId, _cancellationToken))
                            .ReturnsAsync((Core.Models.Task)null);

            // Act
            var result = await _controller.GetByIdAsync(taskId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task AddAsync_ReturnsCreatedAtActionResult_WhenTaskIsCreated()
        {
            // Arrange
            var taskRequest = _fixture.Create<TaskRequest>();
            var createdTask = _fixture.Build<Core.Models.Task>()
                                      .With(task => task.Title, taskRequest.Title)
                                      .Create();
            _mockTaskService.Setup(service => service.AddAsync(It.IsAny<Core.Models.Task>(), _cancellationToken))
                            .ReturnsAsync(createdTask);

            // Act
            var result = await _controller.AddAsync(taskRequest);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var taskResponse = Assert.IsType<TaskResponse>(createdAtActionResult.Value);
            Assert.Equal(createdTask.Id, taskResponse.Id);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNoContentResult_WhenTaskIsUpdated()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var taskRequest = _fixture.Create<TaskRequest>();
            _mockTaskService.Setup(service => service.UpdateAsync(It.IsAny<Core.Models.Task>(), _cancellationToken))
                            .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateAsync(taskId, taskRequest);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFoundResult_WhenTaskDoesNotExist()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var taskRequest = _fixture.Create<TaskRequest>();
            _mockTaskService.Setup(service => service.UpdateAsync(It.IsAny<Core.Models.Task>(), _cancellationToken))
                            .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateAsync(taskId, taskRequest);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNoContentResult_WhenTaskIsDeleted()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _mockTaskService.Setup(service => service.DeleteAsync(taskId, _cancellationToken)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteAsync(taskId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetTasksByProjectIdAsync_ReturnsOkResult_WithTasks()
        {
            // Arrange
            var expectedCount = 2;
            var projectId = Guid.NewGuid();
            var tasks = _fixture.CreateMany<Core.Models.Task>(expectedCount).ToList();
            _mockTaskService.Setup(service => service.GetTasksByProjectIdAsync(projectId, _cancellationToken))
                            .ReturnsAsync(tasks);

            // Act
            var result = await _controller.GetTasksByProjectIdAsync(projectId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var taskResponses = Assert.IsType<List<TaskResponse>>(okResult.Value);
            Assert.Equal(expectedCount, taskResponses.Count);
        }

        [Fact]
        public async Task GetTasksByEmployeeIdAsync_ReturnsOkResult_WithTasks()
        {
            // Arrange
            var expectedCount = 2;
            var employeeId = Guid.NewGuid();
            var tasks = _fixture.CreateMany<Core.Models.Task>(expectedCount).ToList();
            _mockTaskService.Setup(service => service.GetTasksByEmployeeIdAsync(employeeId, _cancellationToken))
                            .ReturnsAsync(tasks);

            // Act
            var result = await _controller.GetTasksByEmployeeIdAsync(employeeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var taskResponses = Assert.IsType<List<TaskResponse>>(okResult.Value);
            Assert.Equal(expectedCount, taskResponses.Count);
        }

        [Fact]
        public async Task ListAllTasksAsync_ReturnsOkResult_WithTasks()
        {
            // Arrange
            var expectedCount = 2;
            var tasks = _fixture.CreateMany<Core.Models.Task>(expectedCount).ToList();
            _mockTaskService.Setup(service => service.ListAllAsync(_cancellationToken))
                            .ReturnsAsync(tasks);

            // Act
            var result = await _controller.ListAllTasksAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var taskResponses = Assert.IsType<List<TaskResponse>>(okResult.Value);
            Assert.Equal(expectedCount, taskResponses.Count);
        }
    }
}