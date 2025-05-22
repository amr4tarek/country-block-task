using Block.Application.Dtos;

namespace Block.Application.Interfaces.Services
{
    public interface IPGeolocationService

    {
        Task<IpGeoLocationResultDto?> LookupIpAsync(string ipAddress);
        Task<CountryInfoDto?> GetCountryByCodeAsync(string countryCode);

    }
}
