using Microsoft.Extensions.Logging;
using SGP.Application.DTOs;
using SGP.Application.Interfaces;
using SGP.Domain.Base;
using SGP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGP.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductService> _logger; 

        public ProductService(IProductRepository repository, ILogger<ProductService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<OperationResult> GetAllAsync()
        {
            _logger.LogInformation("Iniciando consulta de todos los productos...");
            var result = await _repository.GetAllAsync();
            _logger.LogInformation($"Consulta finalizada. Éxito: {result.IsSuccess}");
            return result;
        }

        public async Task<OperationResult> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Buscando producto con ID: {id}");
            return await _repository.GetByIdAsync(id);
        }

        public async Task<OperationResult> CreateAsync(ProductDTO dto)
        {
            _logger.LogInformation($"Intentando crear producto: {dto.Nombre}");

            if (dto.Precio < 0)
            {
                _logger.LogWarning($"Intento de creación fallido: Precio negativo ({dto.Precio}) para {dto.Nombre}");
                return OperationResult.Failure("El precio no puede ser negativo");
            }

            var product = new Product
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                Stock = dto.Stock
            };

            var result = await _repository.CreateAsync(product);

            if (result.IsSuccess)
                _logger.LogInformation($"Producto creado exitosamente con ID: {((Product)result.Data).Id}");

            return result;
        }

        public async Task<OperationResult> UpdateAsync(int id, ProductDTO dto)
        {
            _logger.LogInformation($"Iniciando actualización de producto ID: {id}");


            var result = await _repository.GetByIdAsync(id);
            if (!result.IsSuccess)
            {
                _logger.LogWarning($"No se pudo actualizar. Producto ID {id} no encontrado.");
                return result;
            }

            var product = (Product)result.Data;

            product.Nombre = dto.Nombre;
            product.Descripcion = dto.Descripcion;
            product.Precio = dto.Precio;
            product.Stock = dto.Stock;

            var updateResult = await _repository.UpdateAsync(product);
            _logger.LogInformation($"Producto ID {id} actualizado correctamente.");

            return updateResult;
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            _logger.LogInformation($"Solicitud de eliminación para producto ID: {id}");

            var result = await _repository.DeleteAsync(id);

            if (result.IsSuccess)
                _logger.LogInformation($"Producto ID {id} eliminado (Soft Delete).");
            else
                _logger.LogWarning($"Fallo al eliminar producto ID {id}: {result.Message}");

            return result;
        }
    }
}
