using Block.Application.Dtos;
using Block.Domain;

namespace Block.Application.Interfaces.Services;

public interface ICountryBlockService
{
    Task<bool> BlockCountryAsync(string countryCode);
    Task<bool> UnblockCountryAsync(string countryCode);
    Task<PagedResultDto<Country>> GetBlockedCountriesAsync(int page, int pageSize, string? filter);
    Task<bool> TemporalBlockCountryAsync(string countryCode, int durationMinutes);
    Task RemoveExpiredTemporalBlocksAsync();
    Task<bool> IsCountryBlockedAsync(string countryCode);
}
