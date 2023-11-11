using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Data;
using Northwind.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind.MySQL.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderDetailController : ControllerBase
    {
        private readonly NorthwindContext _context;

        public OrderDetailController(NorthwindContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Orderdetail>>> GetOrderDetails()
        {
            var orderDetails = await _context.Orderdetails.ToListAsync();
            return orderDetails;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Orderdetail>> GetOrderDetail(int id)
        {
            var orderDetail = await _context.Orderdetails.FindAsync(id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            return orderDetail;
        }

        [HttpPost]
        public async Task<ActionResult<Orderdetail>> PostOrderDetail(Orderdetail orderDetail)
        {
            try
            {
                _context.Orderdetails.Add(orderDetail);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetOrderDetail), new { id = orderDetail.OrderDetailsId }, orderDetail);
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException;
                // Log or handle the inner exception details
                return StatusCode(500, "An error occurred while saving the entity changes.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderDetail(int id, Orderdetail orderDetail)
        {
            if (id != orderDetail.OrderDetailsId)
            {
                return BadRequest();
            }

            _context.Entry(orderDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderDetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {
            var orderDetail = await _context.Orderdetails.FindAsync(id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            _context.Orderdetails.Remove(orderDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderDetailExists(int id)
        {
            return _context.Orderdetails.Any(e => e.OrderDetailsId == id);
        }
    }
}
