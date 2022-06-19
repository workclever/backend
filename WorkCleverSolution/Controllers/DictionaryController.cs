using Microsoft.AspNetCore.Mvc;
using WorkCleverSolution.Services;

namespace WorkCleverSolution.Controllers;

[Route("Api/Dictionary/[action]")]
public class DictionaryController : BaseApiController
{
    public DictionaryController(IServices services) : base(services)
    {
    }
    
    [HttpGet]
    public Task<ServiceResult> GetTimeZones()
    {
        var tzList = TimeZoneInfo.GetSystemTimeZones();
        return Task.FromResult(Wrap(tzList.Select(r => r.Id).ToList()));
    }
}