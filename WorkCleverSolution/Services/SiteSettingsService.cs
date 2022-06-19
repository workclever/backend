using WorkCleverSolution.Data;
using WorkCleverSolution.Dto.Global;
using WorkCleverSolution.Utils;

namespace WorkCleverSolution.Services;

public interface ISiteSettingsService
{
    Task UpdateSiteSetting(UpdateSiteSettingInput input);
    Task<SiteSettings> GetSiteSettings();
}

public class SiteSettingsService : ISiteSettingsService
{
    private readonly IRepository<SiteSettings> _siteSettingsRepository;

    public SiteSettingsService(ApplicationDbContext dbContext)
    {
        _siteSettingsRepository = new Repository<SiteSettings>(dbContext);
    }

    public async Task UpdateSiteSetting(UpdateSiteSettingInput input)
    {
        var setting = (await _siteSettingsRepository
            .GetAll())[0];
        var oldValue = ReflectionUtils.GetObjectPropertyValue(setting, input.Property);
        var newValue = input.Value;

        // If old and new value are same, we don't need to update
        if (oldValue == newValue)
        {
            return;
        }

        ReflectionUtils.SetObjectProperty(setting, input.Property, input.Value);
        await _siteSettingsRepository.Update(setting);
    }

    public async Task<SiteSettings> GetSiteSettings()
    {
        return (await _siteSettingsRepository.GetAll())[0];
    }
}