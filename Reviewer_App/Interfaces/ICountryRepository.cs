using Reviewer_App.Models;

namespace Reviewer_App.Interfaces
{
    public interface ICountryRepository
    {

        ICollection<Country> GetCountries();
        Country GetCountry(int id);
        Country GetCountryByOwner(int owner_id);
        ICollection<Owner> GetOwnersFromACountry(int id);
        bool CountryExist(int id);

        bool CreateCountry(Country country);
        bool UpdateCountry(Country country);
        bool DeleteCountry(Country country);
        bool Save();
    }
}
