using Block.Application.Interfaces.Repositories;
using Block.Domain;
using System.Collections.Concurrent;

namespace Block.Infrastructure.Repositories;

public class AttemptLogRepository : IAttemptLogRepository
{
    private readonly ConcurrentQueue<BlockedLog> _logs = new();

    public Task AddLog(BlockedLog log)
    {
        _logs.Enqueue(log);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<BlockedLog>> GetLogs()
    {
        return Task.FromResult<IEnumerable<BlockedLog>>(_logs.ToList());
    }
}
