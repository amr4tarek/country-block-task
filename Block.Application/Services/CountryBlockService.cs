using Block.Application.Dtos;
using Block.Application.Interfaces.Repositories;
using Block.Application.Interfaces.Services;
using Block.Domain;
using System.Collections.Concurrent;

public class CountryBlockService : ICountryBlockService
{
    private readonly ICountryBlockRepository _repo;
    private readonly IPGeolocationService _geoService;

    public CountryBlockService(ICountryBlockRepository repo, IPGeolocationService geoService)
    {
        _repo = repo;
        _geoService = geoService;
    }

    private string NormalizeCountryCode(string countryCode) => countryCode.ToUpperInvariant();

    public async Task<bool> BlockCountryAsync(string countryCode)
    {
        countryCode = NormalizeCountryCode(countryCode);
        if (await _repo.GetCountryAsync(countryCode) != null)
            return false;

        var info = await _geoService.GetCountryByCodeAsync(countryCode);
        if (info == null) return false;

        var status = new Country
        {
            CountryCode = info.CountryCode,
            CountryName = info.CountryName
        };

        return await _repo.AddBlockAsync(status);
    }

    public Task<bool> UnblockCountryAsync(string countryCode) =>
        _repo.RemoveBlockAsync(NormalizeCountryCode(countryCode));

    public async Task<PagedResultDto<Country>> GetBlockedCountriesAsync(int page, int pageSize, string? filter)
    {
        var all = await _repo.GetAllAsync();

        var filtered = string.IsNullOrWhiteSpace(filter)
            ? all
            : all.Where(x =>
                x.CountryCode.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                x.CountryName?.Contains(filter, StringComparison.OrdinalIgnoreCase) == true
            );

        var total = filtered.Count();
        var items = filtered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResultDto<Country>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            Items = items
        };
    }

    public async Task<bool> TemporalBlockCountryAsync(string countryCode, int durationMinutes)
    {
        countryCode = NormalizeCountryCode(countryCode);

        var existing = await _repo.GetCountryAsync(countryCode);
        if (existing != null && existing.IsTemporarilyBlocked)
            return false; 
        var info =  await _geoService.GetCountryByCodeAsync(countryCode);
        if (info == null)
            return false;

        var updated = new Country
        {
            CountryCode = info.CountryCode,
            CountryName = info.CountryName,
            TemporaryBlockExpiry = DateTime.UtcNow.AddMinutes(durationMinutes)
        };

        return await _repo.UpsertBlockAsync(updated);
    }


    public async Task RemoveExpiredTemporalBlocksAsync()
    {
        var all = await _repo.GetAllAsync();
        var expired = all.Where(b => b.TemporaryBlockExpiry.HasValue && !b.IsTemporarilyBlocked);

        foreach (var block in expired)
        {
            await _repo.RemoveBlockAsync(block.CountryCode);
        }
    }

    public async Task<bool> IsCountryBlockedAsync(string countryCode)
    {
        var block = await _repo.GetCountryAsync(NormalizeCountryCode(countryCode));
        return block != null;
    }
}
