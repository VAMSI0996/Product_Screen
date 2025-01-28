using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductServices.Entity;
using ProductServices.Models;

namespace ProductServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductApprovalContext _context;

        public ProductController(ProductApprovalContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveProducts()
        {
            var products = await _context.Products
                                          .Where(p => p.Status == "Active")
                                          .OrderByDescending(p => p.PostedDate).ToListAsync();
            return Ok(products);
        }

        private Product MapProductObject(ProductDto productInfo)
        {
            var result =new Product();
            result.ProductId = productInfo.ProductId;
            result.Name = productInfo.Name;
            result.Price = productInfo.Price;
            result.PostedDate = productInfo.PostedDate;
            result.Status = productInfo.Status;
            result.IsInApprovalQueue = productInfo.IsInApprovalQueue;
            result.ApprovalQueues = new List<ApprovalQueue>();
            foreach (var item in productInfo.ApprovalQueues)
            {
                var approval = new ApprovalQueue();
                approval.RequestDate = item.RequestDate;
                approval.Action = item.Action;
                result.ApprovalQueues.Add(approval);
            }

            return result;

        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductDto productInfo)
        {
            var newproduct = MapProductObject(productInfo);
            _context.Products.Add(newproduct);
            await _context.SaveChangesAsync();
            return Created("", newproduct);

        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string productName, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] DateTime? minPostedDate, [FromQuery] DateTime? maxPostedDate)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(productName))
                query = query.Where(p => p.Name.Contains(productName));

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            if (minPostedDate.HasValue)
                query = query.Where(p => p.PostedDate >= minPostedDate);

            if (maxPostedDate.HasValue)
                query = query.Where(p => p.PostedDate <= maxPostedDate);

            var products = await query.ToListAsync();
            return Ok(products);
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] ProductDto updatedProduct)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return NotFound();

            var priceDifference = Math.Abs(updatedProduct.Price - product.Price);
            if (priceDifference > (product.Price * 0.5m)) // More than 50% change in price
            {
                var approvalQueue = new ApprovalQueue
                {
                    ProductId = productId,
                    RequestDate = DateTime.UtcNow,
                    Action = "Rejected"
                };
                _context.ApprovalQueues.Add(approvalQueue);
                await _context.SaveChangesAsync();
            }

            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            product.Status = updatedProduct.Status;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return NotFound();

            var approvalQueue = new ApprovalQueue
            {
                ProductId = productId,
                RequestDate = DateTime.UtcNow,
            };
            _context.ApprovalQueues.Add(approvalQueue);
            await _context.SaveChangesAsync();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("approval-queue")]
        public async Task<IActionResult> GetApprovalQueue()
        {
            var approvalQueueItems = await _context.ApprovalQueues
                                                     .Where(a => a.Action == "Approved")
                                                     .OrderBy(a => a.RequestDate)
                                                     .ToListAsync();
            return Ok(approvalQueueItems);
        }

        [HttpPut("approval-queue/{approvalId}/approve")]
        public async Task<IActionResult> ApproveProduct(int approvalId)
        {
            var approvalQueue = await _context.ApprovalQueues.FindAsync(approvalId);
            if (approvalQueue == null || approvalQueue.Action != "Rejected")
                return NotFound();

            approvalQueue.Action = "Approved";
            var product = await _context.Products.FindAsync(approvalQueue.ProductId);
            product.Status = "Active";

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("approval-queue/{approvalId}/reject")]
        public async Task<IActionResult> RejectProduct(int approvalId)
        {
            var approvalQueue = await _context.ApprovalQueues.FindAsync(approvalId);
            if (approvalQueue == null || approvalQueue.Action != "Approved")
                return NotFound();

            approvalQueue.Action = "Rejected";

            await _context.SaveChangesAsync();

            return NoContent();
        }





    }
}
