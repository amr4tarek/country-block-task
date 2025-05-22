using Block.Domain;

namespace Block.Application.Interfaces.Repositories;

public interface ICountryBlockRepository
{
    Task<bool> AddBlockAsync(Country status);
    Task<bool> RemoveBlockAsync(string countryCode);
    Task<Country?> GetCountryAsync(string countryCode);
    Task<IEnumerable<Country>> GetAllAsync();
    Task<bool> UpsertBlockAsync(Country country);

}
