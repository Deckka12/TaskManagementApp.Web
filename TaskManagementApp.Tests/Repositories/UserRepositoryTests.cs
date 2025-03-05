using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Infrastructure.DBContext ;
using TaskManagementApp.Infrastructure.Repositories;
using Xunit;

namespace TaskManagementApp.Tests.Repositories
{
    public class UserRepositoryTests
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
        public async Task AddUser_Should_AddUserToDatabase()
        {
            // Arrange
            var context = await GetDbContext();
            var repository = new UserRepository(context);
            var user = new User { Id = Guid.NewGuid(), Name = "John Doe", Email = "john@example.com", PasswordHash = "hashed123" };

            // Act
            await repository.AddAsync(user);
            var result = await repository.GetByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Name, result.Name);
        }

        [Fact]
        public async Task GetUserByEmail_Should_ReturnUser()
        {
            // Arrange
            var context = await GetDbContext();
            var repository = new UserRepository(context);
            var user = new User { Id = Guid.NewGuid(), Name = "Alice", Email = "alice@example.com", PasswordHash = "hashed456" };
            await repository.AddAsync(user);

            // Act
            var result = await repository.GetByEmailAsync(user.Email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task UpdateUser_Should_ModifyExistingUser()
        {
            // Arrange
            var context = await GetDbContext();
            var repository = new UserRepository(context);
            var user = new User { Id = Guid.NewGuid(), Name = "Old Name", Email = "old@example.com", PasswordHash = "hashed789" };
            await repository.AddAsync(user);

            // Act
            user.Name = "New Name";
            await repository.UpdateAsync(user);
            var updatedUser = await repository.GetByIdAsync(user.Id);

            // Assert
            Assert.NotNull(updatedUser);
            Assert.Equal("New Name", updatedUser.Name);
        }

        [Fact]
        public async Task DeleteUser_Should_RemoveUserFromDatabase()
        {
            // Arrange
            var context = await GetDbContext();
            var repository = new UserRepository(context);
            var user = new User { Id = Guid.NewGuid(), Name = "Bob", Email = "bob@example.com", PasswordHash = "hashed999" };
            await repository.AddAsync(user);

            // Act
            await repository.DeleteAsync(user);
            var deletedUser = await repository.GetByIdAsync(user.Id);

            // Assert
            Assert.Null(deletedUser);
        }
    }
}
