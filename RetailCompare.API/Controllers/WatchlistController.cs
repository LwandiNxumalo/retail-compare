using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailCompare.API.Data;
using RetailCompare.Shared.models;

namespace RetailCompare.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WatchlistController : ControllerBase
{
    private readonly RetailCompareDbContext _context;

    public WatchlistController(RetailCompareDbContext context)
    {
        _context = context;
    }

    // GET: api/watchlist/{userId}
    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<WatchlistRequest>>> GetUserWatchlist(string userId)
    {
        var items = await _context.Watchlists
            .Where(w => w.UserId == userId)
            .ToListAsync();

        return Ok(items);
    }

    // POST: api/watchlist
    [HttpPost]
    public async Task<ActionResult<WatchlistRequest>> AddToWatchlist([FromBody] WatchlistRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.UserId))
        {
            return BadRequest("Invalid watchlist payload.");
        }

        var existing = await _context.Watchlists
            .FirstOrDefaultAsync(w => w.UserId == request.UserId && w.ProductId == request.ProductId);

        if (existing != null)
        {
            // Update target price if item already on user's watchlist
            existing.TargetPrice = request.TargetPrice;
            _context.Watchlists.Update(existing);
        }
        else
        {
            _context.Watchlists.Add(request);
        }

        await _context.SaveChangesAsync();
        return Ok(request);
    }

    // DELETE: api/watchlist/{userId}/{productId}
    [HttpDelete("{userId}/{productId:int}")]
    public async Task<IActionResult> RemoveFromWatchlist(string userId, int productId)
    {
        var item = await _context.Watchlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

        if (item == null)
        {
            return NotFound();
        }

        _context.Watchlists.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}