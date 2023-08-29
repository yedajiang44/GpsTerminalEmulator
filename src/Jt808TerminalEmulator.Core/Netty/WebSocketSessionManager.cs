using System.Collections.Concurrent;
using System.Text;

namespace Jt808TerminalEmulator.Core.Netty;

public class WebSocketSessionManager
{
    private ConcurrentDictionary<string, IWebSocketSession> Sessions { get; } = new ConcurrentDictionary<string, IWebSocketSession>();
    public int SessionCount => Sessions.Count;

    public IEnumerable<IWebSocketSession> GetAllSessions() => Sessions.Values;

    public bool TryAdd(IWebSocketSession session) => Sessions.TryAdd(session.Id, session);
    public bool TryRemove(string sessionId)
    {
        if (Sessions.TryRemove(sessionId, out var session))
        {
            session.Close().GetAwaiter().GetResult();
            return true;
        }
        return default;
    }

    public Task SendAll(string data) => TrySendAll(data);

    public Task Send(string sessionId, string data)
    {
        if (Sessions.TryGetValue(sessionId, out var session))
        {
            session.LastActiveTime = DateTime.Now;
            return session.Send(Encoding.UTF8.GetBytes(data));
        }
        return Task.FromResult(false);
    }

    public Task TrySendAll(string data)
    {
        return Task.Run(() => Sessions.Values.AsParallel().ForEach(x => Send(x.Id, data)));
    }

    public Task UpdateLastActiveTime(string sessionId)
    {
        return Task.Run(() =>
        {
            if (Sessions.TryGetValue(sessionId, out var session))
            {
                session.LastActiveTime = DateTime.Now;
            }
        });
    }
}