using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGP.Application.Interfaces;
using SGP.Domain.Base;
using SGP.Domain.Entities;
using SGP.Persistence.Db;

namespace SGP.Persistence
{
    public class ProductRepository : IProductRepository
    {
        private readonly Context _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(Context context, ILogger<ProductRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<OperationResult> GetAllAsync()
        {
            try
            {
                var products = await _context.Products
                                             .Where(p => !p.IsDeleted)
                                             .AsNoTracking()
                                             .ToListAsync();
                return OperationResult.Success("Productos obtenidos", products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en GetAllAsync");
                return OperationResult.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<OperationResult> GetByIdAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null || product.IsDeleted)
                    return OperationResult.Failure("Producto no encontrado");

                return OperationResult.Success("Producto encontrado", product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error en GetByIdAsync {id}");
                return OperationResult.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<OperationResult> CreateAsync(Product entity)
        {
            try
            {
                entity.IsDeleted = false;
                await _context.Products.AddAsync(entity);
                await _context.SaveChangesAsync();
                return OperationResult.Success("Producto creado", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en CreateAsync");
                return OperationResult.Failure($"Error al crear: {ex.Message}");
            }
        }

        public async Task<OperationResult> UpdateAsync(Product entity)
        {
            try
            {
                _context.Products.Update(entity);
                await _context.SaveChangesAsync();
                return OperationResult.Success("Producto actualizado", entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en UpdateAsync");
                return OperationResult.Failure($"Error al actualizar: {ex.Message}");
            }
        }
        public async Task<OperationResult> DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.Products.FindAsync(id);

                if (entity == null || entity.IsDeleted)
                    return OperationResult.Failure("El producto no existe.");

                entity.IsDeleted = true;

                _context.Products.Update(entity);
                await _context.SaveChangesAsync();

                return OperationResult.Success("Producto eliminado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error en DeleteAsync {id}");
                return OperationResult.Failure($"Error al eliminar: {ex.Message}");
            }
        }
    }
}
