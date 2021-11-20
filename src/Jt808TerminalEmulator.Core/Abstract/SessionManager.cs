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
                    tcpSessions.AddOrUpdate(tcpSession.PhoneNumber, tcpSession, (key, value) => tcpSession);
                    break;
            }
        }

        public ITcpClientSession GetTcpClientSession(string phoneNumber)
        {
            tcpSessions.TryGetValue(phoneNumber, out ITcpClientSession session);
            return session;
        }
        public ITcpClientSession GetTcpClientSessionById(string sessionId)
        {
            return tcpSessions.Values.FirstOrDefault(x => x.Id == sessionId);
        }

        public IEnumerable<ITcpClientSession> GetTcpClientSessions() => tcpSessions.Values.ToList();

        public void RemoveById(string sessionId)
        {
            var session = tcpSessions.Values.FirstOrDefault(x => x.Id == sessionId);
            tcpSessions.TryRemove(session.PhoneNumber, out session);
        }
    }
    public interface ISessionManager
    {
        void Add(ISession session);
        void RemoveById(string sessionId);
        ITcpClientSession GetTcpClientSession(string phoneNumber);
        ITcpClientSession GetTcpClientSessionById(string sessionId);
        IEnumerable<ITcpClientSession> GetTcpClientSessions();
    }
}
