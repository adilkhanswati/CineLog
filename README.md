# CineLog

A simple personal **movie watchlist** built with ASP.NET Core MVC for a 3rd-year
Web Development project. Users register, log in, and keep a private list of
movies — what they want to watch and what they have already seen, with an
optional rating.

## Tech stack
- ASP.NET Core **MVC (.NET 8)**
- **Entity Framework Core** (code-first) with **SQLite**
- Cookie-based authentication, PBKDF2 password hashing

## Features
- Register / log in / log out
- Add, edit, and delete movies (title, genre, year, status, rating)
- Each user only sees their own movies
- Server-side validation and anti-forgery protection

## Running locally
```bash
dotnet restore
dotnet run
```
Open the URL shown in the console (e.g. `http://localhost:5080`). The SQLite
database (`cinelog.db`) is created automatically on first run.

## Project structure
| Path | Purpose |
|------|---------|
| `Controllers/` | Home, Account (auth), Movies (CRUD) |
| `Models/` | `User`, `Movie`, and the auth view models |
| `Data/AppDbContext.cs` | EF Core database context |
| `Helpers/PasswordHasher.cs` | PBKDF2 password hashing |
| `Views/` | Razor views (minimal black-and-white UI) |
