using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Infrastructure.DBContext;
using TaskManagementApp.Infrastructure.Repositories;
using Xunit;

namespace TaskManagementApp.Tests.Repositories
{
    public class TaskRepositoryTests
    {
        private async Task<AppDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);
            await context.Database.EnsureCreatedAsync();
            return context;
        }

        [Fact]
        public async Task AddTask_Should_AddTaskToDatabase()
        {
            // Arrange
            var context = await GetDbContext();
            var repository = new TaskRepository(context);
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "Test Task", UserId = Guid.NewGuid(), ProjectId = Guid.NewGuid() };

            // Act
            await repository.AddAsync(task);
            var result = await repository.GetByIdAsync(task.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(task.Title, result.Title);
        }

        [Fact]
        public async Task GetByUserId_Should_ReturnTasksForUser()
        {
            // Arrange
            var context = await GetDbContext();
            var repository = new TaskRepository(context);
            var userId = Guid.NewGuid();
            var task1 = new TaskItem { Id = Guid.NewGuid(), Title = "Task 1", UserId = userId, ProjectId = Guid.NewGuid() };
            var task2 = new TaskItem { Id = Guid.NewGuid(), Title = "Task 2", UserId = userId, ProjectId = Guid.NewGuid() };
            await repository.AddAsync(task1);
            await repository.AddAsync(task2);

            // Act
            var userTasks = await repository.GetByUserIdAsync(userId);

            // Assert
            Assert.Equal(2, userTasks.Count());
        }

        [Fact]
        public async Task DeleteTask_Should_RemoveTaskFromDatabase()
        {
            // Arrange
            var context = await GetDbContext();
            var repository = new TaskRepository(context);
            var task = new TaskItem { Id = Guid.NewGuid(), Title = "Task to delete", UserId = Guid.NewGuid(), ProjectId = Guid.NewGuid() };
            await repository.AddAsync(task);

            // Act
            await repository.DeleteAsync(task);
            var deletedTask = await repository.GetByIdAsync(task.Id);

            // Assert
            Assert.Null(deletedTask);
        }
    }
}
