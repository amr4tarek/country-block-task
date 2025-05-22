using Block.Application.Dtos;
using Block.Application.Interfaces;
using Block.Application.Interfaces.Services;
using Block.Domain;
using Block.API.Common;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.RegularExpressions;

namespace Block.API.Controllers
{
    [ApiController]
    [Route("api/ip")]
    public class IpController : ControllerBase
    {
        private readonly IPGeolocationService _geoService;
        private readonly ICountryBlockService _countryBlockService;
        private readonly IBlockedLogService _logService;
        private readonly IHttpClientFactory _httpClientFactory;

        public IpController(
            IPGeolocationService geoService,
            ICountryBlockService countryBlockService,
            IBlockedLogService logService,
            IHttpClientFactory httpClientFactory)
        {
            _geoService = geoService;
            _countryBlockService = countryBlockService;
            _logService = logService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup([FromQuery] string? ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                if (ipAddress == null)
                    return BadRequest(ApiResponse<string>.ErrorResponse("Unable to determine caller IP address."));
            }

            if (!IsValidIp(ipAddress))
                return BadRequest(ApiResponse<string>.ErrorResponse("Invalid IP address format."));

            var geo = await _geoService.LookupIpAsync(ipAddress);

            if (geo == null)
                return NotFound(ApiResponse<string>.ErrorResponse("Geolocation info not found for IP."));

            return Ok(ApiResponse<IpGeoLocationResultDto>.SuccessResponse(geo));
        }

        [HttpGet("check-block")]
        public async Task<IActionResult> CheckBlock()
        {
            string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrWhiteSpace(ip) ||
                IPAddress.TryParse(ip, out var addr) &&
                (IPAddress.IsLoopback(addr) || addr.IsIPv6LinkLocal || addr.IsIPv6SiteLocal))
            {
                try
                {
                    var client = _httpClientFactory.CreateClient();
                    ip = await client.GetStringAsync("https://api.ipify.org");
                }
                catch
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse("Unable to determine external IP address."));
                }
            }

            if (!IPAddress.TryParse(ip, out _))
                return BadRequest(ApiResponse<string>.ErrorResponse("Invalid IP address."));

            var geo = await _geoService.LookupIpAsync(ip);
            if (geo == null)
                return NotFound(ApiResponse<string>.ErrorResponse("Geolocation info not found for IP."));

            var isBlocked = await _countryBlockService.IsCountryBlockedAsync(geo.CountryCode);

            await _logService.AddLogAsync(new BlockedLog
            {
                IpAddress = ip,
                Timestamp = DateTime.UtcNow,
                CountryCode = geo.CountryCode,
                IsBlocked = isBlocked,
                UserAgent = Request.Headers["User-Agent"].ToString()
            });

            var response = new
            {
                Ip = ip,
                geo.CountryCode,
                geo.CountryName,
                Blocked = isBlocked
            };

            return Ok(ApiResponse<object>.SuccessResponse(response));
        }
        private bool IsValidIp(string ip)
        {
            return IPAddress.TryParse(ip, out _);
        }
    }
}
