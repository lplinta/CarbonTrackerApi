using CarbonTrackerApi.DTOs.Inputs;
using CarbonTrackerApi.DTOs.Outputs;
using CarbonTrackerApi.Models;

namespace CarbonTrackerApi.Interfaces.Services;

public interface IAuthService
{
    Task<LoginOutput?> Authenticate(LoginInput loginInput);
    Task<Usuario?> Register(RegisterInput registerInput);
}