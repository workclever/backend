using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WorkCleverSolution.Data;
using WorkCleverSolution.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using WorkCleverSolution.Dto.Auth;

namespace WorkCleverSolution.Services;

public interface IAuthService
{
    Task<string> LoginAsync(LoginInput input);
    Task<string> RegisterAsync(RegisterInput input);
}

public class AuthService : IAuthService
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IRepository<UserPreference> _userPreferencesRepository;
    private readonly ISiteSettingsService _siteSettingsService;

    public AuthService(SignInManager<User> signInManager,
        UserManager<User> userManager,
        ApplicationDbContext dbContext,
        ISiteSettingsService siteSettingsService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _siteSettingsService = siteSettingsService;
        _userPreferencesRepository = new Repository<UserPreference>(dbContext);
    }

    public async Task<string> LoginAsync(LoginInput input)
    {
        var result = await _signInManager.PasswordSignInAsync(input.Email, input.Password, true, false);
        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(input.Email);
            var roles = await _userManager.GetRolesAsync(user);
            return GenerateJwtToken(user, roles);
        }

        throw new ApplicationException("WRONG_CREDENTIALS");
    }

    public async Task<string> RegisterAsync(RegisterInput input)
    {
        var user = new User
        {
            FullName = input.FullName,
            UserName = input.Email,
            Email = input.Email
        };

        var result = await _userManager.CreateAsync(user, input.Password);

        if (result.Succeeded)
        {
            var siteSettings = await _siteSettingsService.GetSiteSettings();
            var userPreference = new UserPreference
            {
                UserId = user.Id,
                Timezone = siteSettings.DefaultTimezone
            };
            await _userPreferencesRepository.Create(userPreference);
            return GenerateJwtToken(user, new List<string>());
        }

        var errors = result.Errors.Select(r => r.Description).ToList();
        throw new ApplicationException(String.Join(", ", errors.ToArray()));
    }

    private string GenerateJwtToken(User user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.JwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(Convert.ToDouble(Constants.JwtExpireDays));

        var token = new JwtSecurityToken(
            Constants.JwtIssuer,
            Constants.JwtIssuer,
            claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}