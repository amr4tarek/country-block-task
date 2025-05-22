using Block.Application.Dtos;
using Block.Domain;

namespace Block.Application.Interfaces.Services;

public interface IBlockedLogService
{
    Task AddLogAsync(BlockedLog log);
    Task<PagedResultDto<BlockedLog>> GetLogsAsync(int page, int pageSize);
}
