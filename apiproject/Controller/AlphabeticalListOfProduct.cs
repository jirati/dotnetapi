using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Northwind.Data;
using Northwind.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.MySQL.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlphabeticalListOfProductController : ControllerBase
    {
        private readonly NorthwindContext _context;
        private readonly ILogger<AlphabeticalListOfProductController> _logger;

        public AlphabeticalListOfProductController(NorthwindContext context, ILogger<AlphabeticalListOfProductController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlphabeticalListOfProduct>>> GetProducts()
        {
            try
            {
                var products = await _context.AlphabeticalListOfProducts.ToListAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AlphabeticalListOfProduct>> GetProduct(int id)
        {
            try
            {
                var product = await _context.AlphabeticalListOfProducts.FindAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving product with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<AlphabeticalListOfProduct>> PostProduct([FromBody] AlphabeticalListOfProduct product)
        {
            try
            {
                // Validation
                var validationContext = new ValidationContext(product, serviceProvider: null, items: null);
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(product, validationContext, validationResults, validateAllProperties: true))
                {
                    return BadRequest(validationResults.Select(r => r.ErrorMessage));
                }

                _context.AlphabeticalListOfProducts.Add(product);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error creating the product");
                return BadRequest("Error creating the product");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, [FromBody] AlphabeticalListOfProduct product)
        {
            try
            {
                if (id != product.ProductId)
                {
                    return BadRequest();
                }

                _context.Entry(product).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError($"Concurrency error updating product with ID: {id}");
                    return StatusCode(500, "Internal server error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal server error updating product with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.AlphabeticalListOfProducts.FindAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                _context.AlphabeticalListOfProducts.Remove(product);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Error deleting product with ID: {id}");
                return BadRequest("Error deleting the product");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal server error deleting product with ID: {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        private bool ProductExists(int id)
        {
            return _context.AlphabeticalListOfProducts.Any(e => e.ProductId == id);
        }
    }
}
