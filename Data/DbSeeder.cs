using CineLog.Helpers;
using CineLog.Models;

namespace CineLog.Data;

public static class DbSeeder
{
    // Creates a demo account with a few sample movies the first time the app runs,
    // so the app is easy to try and test without registering first.
    // Demo login:  username "demo"  /  password "Demo12345"
    public static void Seed(AppDbContext db)
    {
        if (db.Users.Any()) return;

        var demo = new User
        {
            Username = "demo",
            PasswordHash = PasswordHasher.Hash("Demo12345")
        };
        db.Users.Add(demo);
        db.SaveChanges();

        db.Movies.AddRange(
            new Movie { UserId = demo.Id, Title = "Inception", Genre = "Sci-Fi", Year = 2010, Status = WatchStatus.Watched, Rating = 5 },
            new Movie { UserId = demo.Id, Title = "The Dark Knight", Genre = "Action", Year = 2008, Status = WatchStatus.Watched, Rating = 5 },
            new Movie { UserId = demo.Id, Title = "Interstellar", Genre = "Sci-Fi", Year = 2014, Status = WatchStatus.Watched, Rating = 4 },
            new Movie { UserId = demo.Id, Title = "Dune: Part Two", Genre = "Sci-Fi", Year = 2024, Status = WatchStatus.WantToWatch },
            new Movie { UserId = demo.Id, Title = "Oppenheimer", Genre = "Drama", Year = 2023, Status = WatchStatus.WantToWatch }
        );
        db.SaveChanges();
    }
}
