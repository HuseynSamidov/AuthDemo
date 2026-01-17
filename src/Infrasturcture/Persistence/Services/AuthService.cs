using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Persistence.Context;

namespace Persistence.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> userManager;
    private readonly SignInManager<AppUser> signInManager;
    private readonly AppDbContext context;

    public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppDbContext context)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.context = context;
    }
    public Task<BaseResponse<TokenResponse>>Register(RegisterDTO dto)
    {

    }


}
