using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailCompare.API.Data;
using RetailCompare.Shared.models;

namespace RetailCompare.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WatchlistController(RetailCompareDbContext context) : ControllerBase
{
    private readonly RetailCompareDbContext _context = context;

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var id) ? id : 0;
    }

    [HttpGet]
    public async Task<ActionResult<List<WatchlistItemDto>>> GetWatchlist()
    {
        var userId = GetCurrentUserId();
        if (userId == 0) return Unauthorized();

        var items = await _context.WatchlistItems
            .AsNoTracking()
            .Include(w => w.Product)
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.AddedAt)
            .Select(w => new WatchlistItemDto
            {
                Id = w.Id,
                ProductId = w.ProductId,
                ProductName = w.Product != null ? w.Product.Name : "Unknown Product",
                StoreName = w.Product != null ? w.Product.StoreName : "N/A",
                CurrentPrice = w.Product != null ? w.Product.Price : 0,
                TargetPrice = w.TargetPrice,
                ImageUrl = w.Product != null ? w.Product.ImageUrl : null,
                AddedAt = w.AddedAt
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<WatchlistItemDto>> AddToWatchlist(WatchlistRequestDto request)
    {
        var userId = GetCurrentUserId();
        if (userId == 0) return Unauthorized();

        var product = await _context.Products.FindAsync(request.ProductId);
        if (product == null) return NotFound("Product not found.");

        var existingItem = await _context.WatchlistItems
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == request.ProductId);

        if (existingItem != null)
        {
            existingItem.TargetPrice = request.TargetPrice;
            await _context.SaveChangesAsync();

            return Ok(new WatchlistItemDto
            {
                Id = existingItem.Id,
                ProductId = existingItem.ProductId,
                ProductName = product.Name,
                StoreName = product.StoreName,
                CurrentPrice = product.Price,
                TargetPrice = existingItem.TargetPrice,
                ImageUrl = product.ImageUrl,
                AddedAt = existingItem.AddedAt
            });
        }

        var watchlistItem = new WatchlistItem
        {
            UserId = userId,
            ProductId = request.ProductId,
            TargetPrice = request.TargetPrice,
            AddedAt = DateTime.UtcNow
        };

        _context.WatchlistItems.Add(watchlistItem);
        await _context.SaveChangesAsync();

        var resultDto = new WatchlistItemDto
        {
            Id = watchlistItem.Id,
            ProductId = watchlistItem.ProductId,
            ProductName = product.Name,
            StoreName = product.StoreName,
            CurrentPrice = product.Price,
            TargetPrice = watchlistItem.TargetPrice,
            ImageUrl = product.ImageUrl,
            AddedAt = watchlistItem.AddedAt
        };

        return CreatedAtAction(nameof(GetWatchlist), new { id = watchlistItem.Id }, resultDto);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> RemoveFromWatchlist(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == 0) return Unauthorized();

        var item = await _context.WatchlistItems
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

        if (item == null) return NotFound("Watchlist item not found.");

        _context.WatchlistItems.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}