using Block.API.Common;
using Block.Application.Dtos;
using Block.Application.Interfaces;
using Block.Application.Interfaces.Services;
using Block.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Block.API.Controllers;

[ApiController]
[Route("api/logs")]
public class LogsController : ControllerBase
{
    private readonly IBlockedLogService _logService;

    public LogsController(IBlockedLogService logService)
    {
        _logService = logService;
    }

    [HttpGet("blocked-attempts")]
    public async Task<IActionResult> GetBlockedAttempts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1 || pageSize < 1)
            return BadRequest(ApiResponse<string>.ErrorResponse("Page and pageSize must be greater than zero."));

        var result = await _logService.GetLogsAsync(page, pageSize);
        return Ok(ApiResponse<PagedResultDto<BlockedLog>>.SuccessResponse(result));
    }

}
