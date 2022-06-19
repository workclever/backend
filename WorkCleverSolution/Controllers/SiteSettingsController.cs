using Microsoft.AspNetCore.Mvc;
using WorkCleverSolution.Attributes;
using WorkCleverSolution.Dto.Global;
using WorkCleverSolution.Services;

namespace WorkCleverSolution.Controllers;

[Route("Api/SiteSettings/[action]")]
public class SiteSettingsController : BaseApiController
{
    public SiteSettingsController(IServices services) : base(services)
    {
    }

    [HttpGet]
    public async Task<ServiceResult> GetSiteSettings()
    {
        return Wrap(await Services.SiteSettingsService().GetSiteSettings());
    }

    [HttpPost]
    [JwtAuthorize(Policy = "RequireAdminRole")]
    public async Task<ServiceResult> UpdateSiteSetting([FromBody] UpdateSiteSettingInput input)
    {
        await Services.SiteSettingsService().UpdateSiteSetting(input);
        return Wrap();
    }
}