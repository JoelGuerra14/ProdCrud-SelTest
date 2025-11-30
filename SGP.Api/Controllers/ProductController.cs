using Microsoft.AspNetCore.Mvc;
using SGP.Application.DTOs;
using SGP.Application.Interfaces;

namespace SGP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result.IsSuccess) return Ok(result);
            return NotFound(result);
        }

        // POST: api/Product
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDTO dto)
        {
            var result = await _service.CreateAsync(dto);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDTO dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (result.IsSuccess) return Ok(result);
            return BadRequest(result);
        }
    }
}
