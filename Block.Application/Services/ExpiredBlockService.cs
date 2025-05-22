using Block.Application.Interfaces.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Block.API.Services;

public class ExpiredBlockService : BackgroundService
{
    private readonly ICountryBlockService _countryBlockService;
    private readonly ILogger<ExpiredBlockService> _logger;
    private readonly TimeSpan _delay = TimeSpan.FromMinutes(5);

    public ExpiredBlockService(
        ICountryBlockService countryBlockService,
        ILogger<ExpiredBlockService> logger)
    {
        _countryBlockService = countryBlockService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ExpiredBlockService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _countryBlockService.RemoveExpiredTemporalBlocksAsync();
                _logger.LogInformation("Expired temporal blocks cleaned up at: {time}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cleaning expired temporal blocks.");
            }

            await Task.Delay(_delay, stoppingToken);
        }

        _logger.LogInformation("ExpiredBlockService stopping.");
    }
}
