using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApp.Domain.Entities;
using TaskManagementApp.Domain.Interface;

namespace TaskManagementApp.Domain.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
    }
}
