using Microsoft.AspNetCore.Mvc;
using WorkCleverSolution.Dto.Auth;
using WorkCleverSolution.Services;

namespace WorkCleverSolution.Controllers;

[Route("Api/Auth/[action]")]
public class AuthController : BaseApiController
{
    public AuthController(IServices services) : base(services)
    {
    }

    [HttpPost]
    public async Task<ServiceResult> Login([FromBody] LoginInput input)
    {
        return Wrap(await Services.AuthService().LoginAsync(input));
    }

    [HttpPost]
    public async Task<ServiceResult> Register([FromBody] RegisterInput input)
    {
        return Wrap(await Services.AuthService().RegisterAsync(input));
    }
}