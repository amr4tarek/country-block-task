using Block.Domain;

namespace Block.Application.Interfaces.Repositories;

public interface IAttemptLogRepository
{
    Task AddLog(BlockedLog log);
    Task<IEnumerable<BlockedLog>> GetLogs();
}
