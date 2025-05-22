using Block.Application.Dtos;
using Block.Application.Interfaces.Repositories;
using Block.Application.Interfaces.Services;
using Block.Domain;

namespace Block.Application.Services;

public class BlockedtLogService : IBlockedLogService
{
    private readonly IAttemptLogRepository _repo;

    public BlockedtLogService(IAttemptLogRepository repo)
    {
        _repo = repo;
    }

    public async Task AddLogAsync(BlockedLog log)
    {
        await _repo.AddLog(log);
    }

    public async Task<PagedResultDto<BlockedLog>> GetLogsAsync(int page, int pageSize)
    {
        var all = (await _repo.GetLogs()).OrderByDescending(x => x.Timestamp);

        var total = all.Count();
        var items = all
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResultDto<BlockedLog>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            Items = items
        };
    }
}
