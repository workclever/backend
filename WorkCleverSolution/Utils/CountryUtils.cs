using CountryData.Standard;

namespace WorkCleverSolution.Utils;

public static class CountryUtils
{
    public static IEnumerable<Country> GetCountries()
    {
        var countries = new List<Country>(new CountryHelper().GetCountryData());
        return countries.Prepend(new Country
        {
            CountryName = "Worldwide",
            CountryShortCode = "WW"
        });
    }
}