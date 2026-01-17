using Application.DTOs.UserDTOs;
using Application.Shared;

namespace Application.Abstractions.Interfaces;

public interface IAuthService
{
    Task<BaseResponse<TokenResponse>> Register(RegisterDTO dto);
    Task<BaseResponse<TokenResponse>> Login(LoginDTO dto);
}
