using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Jt808TerminalEmulator.Core.Netty;

namespace Jt808TerminalEmulator.Core.Abstract
{
    public class SessionManager : ISessionManager
    {
        // key为终端卡号
        readonly ConcurrentDictionary<string, ITcpClientSession> tcpSessions = new ConcurrentDictionary<string, ITcpClientSession>();

        // key为链接标识
        public void Add(ISession session)
        {
            switch (session)
            {
                case ITcpClientSession tcpSession:
                    tcpSessions.AddOrUpdate(tcpSession.PhoneNumber, tcpSession, (key, value) =>
                    {
                        value?.Close().GetAwaiter().GetResult();
                        return tcpSession;
                    });
                    break;
            }
        }

        public bool Contains(string phoneNumber)
        {
            return tcpSessions.TryGetValue(phoneNumber, out _);
        }

        public ITcpClientSession GetTcpClientSession(string phoneNumber)
        {
            tcpSessions.TryGetValue(phoneNumber, out ITcpClientSession session);
            return session;
        }

        public bool TryGetTcpClientSession(string phoneNumber, out ITcpClientSession session)
        {
            session = GetTcpClientSession(phoneNumber);
            return session != null;
        }

        public ITcpClientSession GetTcpClientSessionById(string sessionId)
        {
            return tcpSessions.Values.FirstOrDefault(x => x.Id == sessionId);
        }

        public IEnumerable<ITcpClientSession> GetTcpClientSessions() => tcpSessions.Values.ToList();

        public void RemoveById(string sessionId)
        {
            var session = tcpSessions.Values.FirstOrDefault(x => x.Id == sessionId);
            if (session != null)
                tcpSessions.TryRemove(session.PhoneNumber, out _);
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
}
