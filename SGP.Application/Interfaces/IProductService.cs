using SGP.Application.DTOs;
using SGP.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGP.Application.Interfaces
{
    public interface IProductService
    {
        Task<OperationResult> GetAllAsync();
        Task<OperationResult> GetByIdAsync(int id);
        Task<OperationResult> CreateAsync(ProductDTO dto);
        Task<OperationResult> UpdateAsync(int id, ProductDTO dto);
        Task<OperationResult> DeleteAsync(int id);
    }
}
