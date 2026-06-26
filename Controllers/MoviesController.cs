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

    public async Task<IActionResult> Index()
    {
        var movies = await _db.Movies
            .Where(m => m.UserId == CurrentUserId)
            .OrderByDescending(m => m.Id)
            .ToListAsync();
        return View(movies);
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

    private Task<Movie?> FindOwnedAsync(int id) =>
        _db.Movies.FirstOrDefaultAsync(m => m.Id == id && m.UserId == CurrentUserId);
}
