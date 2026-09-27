using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailCompare.API.Data;
using RetailCompare.Shared.models;

namespace RetailCompare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly RetailCompareDbContext _context;

    public ProductsController(RetailCompareDbContext context)
    {
        _context = context;
    }

    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts([FromQuery] string? search)
    {
        var query = _context.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(term) || p.Category.ToLower().Contains(term));
        }

        var products = await query
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                CurrentLowestPrice = p.Price, // Maps entity Price -> ProductDto CurrentLowestPrice
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