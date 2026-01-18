using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Domain.Entities;

public class AppUser : IdentityUser
{
    public string FullName {  get; set; } = string.Empty;
    public int Age { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
