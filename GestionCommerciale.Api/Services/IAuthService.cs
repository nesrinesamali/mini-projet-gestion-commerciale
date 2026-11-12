using GestionCommerciale.Api.Dtos;

namespace GestionCommerciale.Api.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto dto);
}