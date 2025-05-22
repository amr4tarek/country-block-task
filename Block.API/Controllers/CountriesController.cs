using Block.API.Common;
using Block.Application.Dtos;
using Block.Application.Interfaces.Services;
using Block.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Block.API.Controllers;

[ApiController]
[Route("api/countries")]
public class CountriesController : ControllerBase
{
    private readonly ICountryBlockService _countryBlockService;

    public CountriesController(ICountryBlockService countryBlockService)
    {
        _countryBlockService = countryBlockService;
    }

    [HttpPost("block")]
    public async Task<IActionResult> BlockCountry([FromBody] CountryBlockRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.CountryCode))
            return BadRequest(ApiResponse<string>.ErrorResponse("CountryCode is required."));

        var success = await _countryBlockService.BlockCountryAsync(request.CountryCode.ToUpper());

        if (!success)
            return Conflict(ApiResponse<string>.ErrorResponse($"Country {request.CountryCode} is already blocked."));

        return Ok(ApiResponse<string>.SuccessResponse($"Country {request.CountryCode} blocked."));
    }

    [HttpDelete("block/{countryCode}")]
    public async Task<IActionResult> UnblockCountry(string countryCode)
    {
        var success = await _countryBlockService.UnblockCountryAsync(countryCode.ToUpper());

        if (!success)
            return NotFound(ApiResponse<string>.ErrorResponse($"Country {countryCode} is not blocked."));

        return Ok(ApiResponse<string>.SuccessResponse($"Country {countryCode} unblocked."));
    }

    [HttpGet("blocked")]
    public async Task<IActionResult> GetBlockedCountries([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? filter = null)
    {
        var result = await _countryBlockService.GetBlockedCountriesAsync(page, pageSize, filter);
        return Ok(ApiResponse<PagedResultDto<Country>>.SuccessResponse(result));
    }

    [HttpPost("temporal-block")]
    public async Task<IActionResult> TemporalBlock([FromBody] TemporalBlockRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.CountryCode))
            return BadRequest(ApiResponse<string>.ErrorResponse("CountryCode is required."));

        if (request.DurationMinutes < 1 || request.DurationMinutes > 1440)
            return BadRequest(ApiResponse<string>.ErrorResponse("DurationMinutes must be between 1 and 1440."));

        var success = await _countryBlockService.TemporalBlockCountryAsync(request.CountryCode.ToUpper(), request.DurationMinutes);

        if (!success)
            return Conflict(ApiResponse<string>.ErrorResponse($"Country {request.CountryCode} is already temporarily blocked."));

        return Ok(ApiResponse<string>.SuccessResponse($"Country {request.CountryCode} temporarily blocked for {request.DurationMinutes} minutes."));
    }

}
