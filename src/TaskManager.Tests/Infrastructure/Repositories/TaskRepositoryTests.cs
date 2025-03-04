﻿using AutoFixture;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskManager.Infrastructure.EF;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Tests.Infrastructure.EF;

namespace TaskManager.Tests.Infrastructure.Repositories;

public class TaskRepositoryTests
{
    private readonly DbContextOptions<DBContext> _options;
    private readonly TestDBContext _context;
    private readonly TaskRepository _taskRepository;
    private readonly Fixture _fixture;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public TaskRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<DBContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new TestDBContext(_options);
        _fixture = new Fixture();

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(behavior => _fixture.Behaviors.Remove(behavior));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _taskRepository = new TaskRepository(_context);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingTasks()
    {
        // Arrange
        var taskTitle = "BestTask";
        var tasks = _fixture.CreateMany<TaskManager.Core.Models.Task>().ToList();
        await _context.Set<TaskManager.Core.Models.Task>().AddRangeAsync(tasks);
        await _context.SaveChangesAsync();

        Expression<Func<TaskManager.Core.Models.Task, bool>> predicate = task => task.Title.Contains(taskTitle);
        var matchingTasks = tasks.Where(predicate.Compile()).ToList();

        // Act
        var result = await _taskRepository.FindAsync(predicate, _cancellationToken);

        // Assert
        Assert.Equal(matchingTasks.Count, result.Count);
        Assert.All(result, task => Assert.Contains(taskTitle, task.Title));
    }
}
