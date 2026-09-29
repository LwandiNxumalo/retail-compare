using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailCompare.API.Data;
using RetailCompare.Shared.models;

namespace RetailCompare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(RetailCompareDbContext context) : ControllerBase
{
    private readonly RetailCompareDbContext _context = context;

    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts([FromQuery] string? search)
    {
        var query = _context.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            // Refactored to avoid client-side allocation warning
            query = query.Where(p => EF.Functions.Like(p.Name, $"%{term}%") ||
                                     EF.Functions.Like(p.Category, $"%{term}%"));
        }

        var products = await query
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                CurrentLowestPrice = p.Price,
                ImageUrl = p.ImageUrl ?? string.Empty,
                Description = p.Description ?? string.Empty
            })
            .ToListAsync();

        return Ok(products);
    }

    // GET: api/products/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                CurrentLowestPrice = p.Price,
                ImageUrl = p.ImageUrl ?? string.Empty,
                Description = p.Description ?? string.Empty
            })
            .FirstOrDefaultAsync();

        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }
}