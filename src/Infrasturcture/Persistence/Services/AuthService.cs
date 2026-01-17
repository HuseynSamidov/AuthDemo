using Application.Abstractions.Interfaces;
using Application.DTOs.UserDTOs;
using Application.Shared;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Persistence.Context;
using System.Net;
using System.Text;

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

    public async Task<BaseResponse<TokenResponse>> Register(RegisterDTO dto)
    {
        var existUser = await userManager.FindByEmailAsync(dto.Email);
        if(existUser != null)
        {
            return new BaseResponse<TokenResponse>("This email already registered",
                System.Net.HttpStatusCode.BadRequest);
        }

        var newUser = new AppUser
        {
            UserName = dto.Name,
            Email = dto.Email,
            Age = dto.Age
        };

        IdentityResult identityResult = await userManager.CreateAsync(newUser,dto.Password);
        if (!identityResult.Succeeded)
        {
            var errors = identityResult.Errors;
            StringBuilder errorMessage = new StringBuilder();
            foreach(var error in errors)
            {
                errorMessage.Append(error.Description + ";");
            }
            return new(errorMessage.ToString(),
                System.Net.HttpStatusCode.BadRequest);
        }

        if (string.IsNullOrWhiteSpace(newUser.Email))
        {
            return new BaseResponse<TokenResponse>(
                "Email cannot be empty. Registration failed.",
                HttpStatusCode.BadRequest);
        }
        return new BaseResponse<TokenResponse>("You registered successfully.",
            HttpStatusCode.Created);
    }
    public async Task<BaseResponse<TokenResponse>> Login(LoginDTO dto)
    {
        var user= await userManager.FindByEmailAsync(dto.Email);

        if(user == null)
        {
            return new BaseResponse<TokenResponse>("Email or password is wrong",
                HttpStatusCode.Unauthorized);
        }
        var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, true);


        bool isUserLockOut = await userManager.IsLockedOutAsync(user);
        if (isUserLockOut)
        {
            return new BaseResponse<TokenResponse>("User is temporary blocked. Try again later.",
                HttpStatusCode.Forbidden);
        }

    }
   
}
