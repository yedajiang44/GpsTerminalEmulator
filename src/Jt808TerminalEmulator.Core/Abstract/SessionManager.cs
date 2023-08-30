using System.Collections.Concurrent;
using Jt808TerminalEmulator.Core.Netty;

namespace Jt808TerminalEmulator.Core.Abstract;

public class SessionManager : ISessionManager
{
    // key为链接标识
    readonly ConcurrentDictionary<string, ITcpClientSession> tcpSessions = new();

    // key为链接标识
    public void Add(ISession session)
    {
        switch (session)
        {
            case ITcpClientSession tcpSession:
                tcpSessions.AddOrUpdate(tcpSession.Id, tcpSession, (_, value) =>
                {
                    value?.Close().GetAwaiter().GetResult();
                    return tcpSession;
                });
                break;
        }
    }

    public bool Contains(string phoneNumber)
    {
        return tcpSessions.Values.Any(x => x.PhoneNumber == phoneNumber);
    }

    public ITcpClientSession GetTcpClientSession(string phoneNumber)
    {
        return tcpSessions.Values.FirstOrDefault(x => x.PhoneNumber == phoneNumber);
    }

    public bool TryGetTcpClientSession(string phoneNumber, out ITcpClientSession session)
    {
        session = GetTcpClientSession(phoneNumber);
        return session != null;
    }

    public ITcpClientSession GetTcpClientSessionById(string sessionId)
    {
        return tcpSessions.TryGetValue(sessionId, out var session) ? session : null;
    }

    public IEnumerable<ITcpClientSession> GetTcpClientSessions() => tcpSessions.Values.ToList();

    public void RemoveById(string sessionId)
    {
        if (tcpSessions.TryRemove(sessionId, out var session))
        {
            session.Close();
        }
    }
}
public interface ISessionManager
{
    void Add(ISession session);
    void RemoveById(string sessionId);
    bool Contains(string simNumber);
    ITcpClientSession GetTcpClientSession(string phoneNumber);
    bool TryGetTcpClientSession(string phoneNumber, out ITcpClientSession session);
    ITcpClientSession GetTcpClientSessionById(string sessionId);
    IEnumerable<ITcpClientSession> GetTcpClientSessions();
}