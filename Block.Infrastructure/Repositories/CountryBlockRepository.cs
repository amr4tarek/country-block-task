using Block.Application.Interfaces.Repositories;
using Block.Domain;
using System.Collections.Concurrent;

namespace Block.Infrastructure.Repositories;

public class CountryBlockRepository : ICountryBlockRepository
{
    private readonly ConcurrentDictionary<string, Country> _countryDictionary = new(StringComparer.OrdinalIgnoreCase);

    public Task<bool> AddBlockAsync(Country country)
    {
        var result = _countryDictionary.TryAdd(country.CountryCode, country);
        return Task.FromResult(result);
    }

    public Task<bool> RemoveBlockAsync(string countryCode)
    {
        var result = _countryDictionary.TryRemove(countryCode, out _);
        return Task.FromResult(result);
    }

    public Task<Country?> GetCountryAsync(string countryCode)
    {
        _countryDictionary.TryGetValue(countryCode, out var result);
        return Task.FromResult(result);
    }

    public Task<IEnumerable<Country>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Country>>(_countryDictionary.Values.ToList());
    }

    public Task<bool> UpsertBlockAsync(Country country)
    {
        _countryDictionary.AddOrUpdate(country.CountryCode, country, (key, old) => country);
        return Task.FromResult(true);
    }

}
