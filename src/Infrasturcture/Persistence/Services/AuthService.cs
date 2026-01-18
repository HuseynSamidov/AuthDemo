using Application.Abstractions.Interfaces;
using Application.DTOs.UserDTOs;
using Application.Shared;
using Application.Shared.Settings;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Persistence.Context;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Persistence.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> userManager;
    private readonly SignInManager<AppUser> signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext context;
    private readonly JWTSettings _jwtSettings;

    public AuthService(UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        AppDbContext context,
        IOptions<JWTSettings> jwtOptions,
        RoleManager<IdentityRole> roleManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.context = context;
        _jwtSettings = jwtOptions.Value;
        _roleManager = roleManager;
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

        var tokenResponse = await GenerateJwtToken(user);

        return new BaseResponse<TokenResponse>("User login succesfully!",
            tokenResponse,
            HttpStatusCode.OK);
    }

    #region Privates
    private async Task<TokenResponse> GenerateJwtToken(AppUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id),
        new(ClaimTypes.Email, user.Email)
    };

        var roles = await userManager.GetRolesAsync(user);

        foreach (var roleName in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleName));

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role is not null)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(role);
                foreach (var permission in roleClaims.Where(c => c.Type == "Permission"))
                {
                    claims.Add(new Claim("Permission", permission.Value));
                }
            }
        }

        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_jwtSettings.ExpiryMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Expires = expires,
            NotBefore = now,
            IssuedAt = now,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(2);

        await userManager.UpdateAsync(user);

        return new TokenResponse
        {
            AccessToken = jwt,
            ExpireDate = expires,
            RefreshToken = refreshToken
        };
    }
    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }


    #endregion


}
