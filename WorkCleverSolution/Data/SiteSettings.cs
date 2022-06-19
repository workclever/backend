namespace WorkCleverSolution.Data;

public class SiteSettings : TimeAwareEntity
{
    public string DefaultTimezone { get; set; }
    public string DefaultDateTimeFormat { get; set; }
    public string DefaultDateFormat { get; set; }
}