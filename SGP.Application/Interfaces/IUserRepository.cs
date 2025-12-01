using SGP.Domain.Base;
using SGP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGP.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<OperationResult> ValidateUserAsync(string username, string password);
        Task<OperationResult> RegisterAsync(User user);
    }
}
