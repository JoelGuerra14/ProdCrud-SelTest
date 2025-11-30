using SGP.Domain.Base;
using SGP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGP.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<OperationResult> GetAllAsync();
        Task<OperationResult> GetByIdAsync(int id);
        Task<OperationResult> CreateAsync(Product entity);
        Task<OperationResult> UpdateAsync(Product entity);
        Task<OperationResult> DeleteAsync(int id);
    }
}
