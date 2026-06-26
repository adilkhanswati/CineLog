using System.Security.Claims;
using CineLog.Data;
using CineLog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Controllers;

[Authorize]
public class MoviesController : Controller
{
    private readonly AppDbContext _db;

    public MoviesController(AppDbContext db) => _db = db;

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index(string? search, string? status, string sort = "added", string dir = "desc")
    {
        var query = _db.Movies.Where(m => m.UserId == CurrentUserId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(m => m.Title.Contains(s) || (m.Genre != null && m.Genre.Contains(s)));
        }

        if (status == "WantToWatch") query = query.Where(m => m.Status == WatchStatus.WantToWatch);
        else if (status == "Watched") query = query.Where(m => m.Status == WatchStatus.Watched);

        bool asc = dir == "asc";
        query = sort switch
        {
            "title" => asc ? query.OrderBy(m => m.Title) : query.OrderByDescending(m => m.Title),
            "year" => asc ? query.OrderBy(m => m.Year) : query.OrderByDescending(m => m.Year),
            "rating" => asc ? query.OrderBy(m => m.Rating) : query.OrderByDescending(m => m.Rating),
            _ => asc ? query.OrderBy(m => m.Id) : query.OrderByDescending(m => m.Id),
        };

        var owned = _db.Movies.Where(m => m.UserId == CurrentUserId);
        var vm = new MoviesIndexViewModel
        {
            Movies = await query.ToListAsync(),
            Search = search,
            Status = status,
            Sort = sort,
            Dir = dir,
            TotalCount = await owned.CountAsync(),
            WatchedCount = await owned.CountAsync(m => m.Status == WatchStatus.Watched),
            WantCount = await owned.CountAsync(m => m.Status == WatchStatus.WantToWatch),
        };
        return View(vm);
    }

    [HttpGet]
    public IActionResult Create() => View(new Movie { Year = DateTime.Now.Year });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Movie movie)
    {
        if (!ModelState.IsValid) return View(movie);

        movie.UserId = CurrentUserId;
        _db.Movies.Add(movie);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var movie = await FindOwnedAsync(id);
        if (movie == null) return NotFound();
        return View(movie);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Movie movie)
    {
        if (id != movie.Id) return NotFound();

        var existing = await FindOwnedAsync(id);
        if (existing == null) return NotFound();
        if (!ModelState.IsValid) return View(movie);

        existing.Title = movie.Title;
        existing.Genre = movie.Genre;
        existing.Year = movie.Year;
        existing.Status = movie.Status;
        existing.Rating = movie.Rating;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var movie = await FindOwnedAsync(id);
        if (movie == null) return NotFound();
        return View(movie);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var movie = await FindOwnedAsync(id);
        if (movie != null)
        {
            _db.Movies.Remove(movie);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleWatched(int id)
    {
        var movie = await FindOwnedAsync(id);
        if (movie == null) return NotFound();

        movie.Status = movie.Status == WatchStatus.Watched ? WatchStatus.WantToWatch : WatchStatus.Watched;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private Task<Movie?> FindOwnedAsync(int id) =>
        _db.Movies.FirstOrDefaultAsync(m => m.Id == id && m.UserId == CurrentUserId);
}
