using Block.Application.Dtos;
using Block.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Block.Infrastructure.Services;

public class GeolocationService : IPGeolocationService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public GeolocationService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["IpGeolocation:ApiKey"] ?? throw new ArgumentNullException("ApiKey");
        _baseUrl = configuration["IpGeolocation:BaseUrl"] ?? "https://api.ipgeolocation.io/ipgeo";
    }

    public async Task<IpGeoLocationResultDto?> LookupIpAsync(string ipAddress)
    {
        var url = $"{_baseUrl}?apiKey={_apiKey}&ip={ipAddress}";
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        return new IpGeoLocationResultDto
        {
            Ip = root.GetProperty("ip").GetString() ?? "",
            CountryCode = root.GetProperty("country_code2").GetString() ?? "",
            CountryName = root.GetProperty("country_name").GetString() ?? "",
            Isp = root.TryGetProperty("isp", out var isp) ? isp.GetString() ?? "" : ""
        };
    }

    public async Task<CountryInfoDto?> GetCountryByCodeAsync(string countryCode)
    {
        var url = $"https://restcountries.com/v3.1/alpha/{countryCode}";
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var array = doc.RootElement;
        if (array.GetArrayLength() == 0) return null;

        var country = array[0];
        var name = country
            .GetProperty("name")
            .GetProperty("common")
            .GetString() ?? "";

        return new CountryInfoDto
        {
            CountryCode = countryCode.ToUpperInvariant(),
            CountryName = name
        };
    }
}
