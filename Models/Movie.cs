using System.ComponentModel.DataAnnotations;

namespace CineLog.Models;

public enum WatchStatus
{
    [Display(Name = "Want to Watch")]
    WantToWatch,

    Watched
}

public class Movie
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(60)]
    public string? Genre { get; set; }

    [Range(1888, 2100, ErrorMessage = "Enter a valid release year.")]
    public int Year { get; set; }

    public WatchStatus Status { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int? Rating { get; set; }

    // Stored file name of an uploaded poster image (in wwwroot/uploads), if any.
    public string? PosterFileName { get; set; }

    // Owner of this movie entry.
    public int UserId { get; set; }
}
