using System.ComponentModel.DataAnnotations;

namespace CineLog.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [StringLength(40)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;
}
