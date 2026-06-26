namespace CineLog.Models;

public class MoviesIndexViewModel
{
    public List<Movie> Movies { get; set; } = new();

    // Current filter / sort state (used to keep the form populated).
    public string? Search { get; set; }
    public string? Status { get; set; }
    public string Sort { get; set; } = "added";
    public string Dir { get; set; } = "desc";

    // Summary counts for the whole collection (not just the filtered view).
    public int TotalCount { get; set; }
    public int WatchedCount { get; set; }
    public int WantCount { get; set; }
}
