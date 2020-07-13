using Jt808TerminalEmulator.Core.Netty;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jt808TerminalEmulator.Core.Abstract
{
    public class SessionManager : ISessionManager
    {
        readonly ConcurrentDictionary<string, ISession> sessions = new ConcurrentDictionary<string, ISession>();
        public void Add(ISession session)
        {
            sessions.AddOrUpdate(session.PhoneNumber, session, (key, value) => session);
        }

        public ISession GetSession(string phoneNumber)
        {
            sessions.TryGetValue(phoneNumber, out ISession session);
            return session;
        }

        public IEnumerable<ISession> GetSessions() => sessions.Values.ToList();
    }
    public interface ISessionManager
    {
        void Add(ISession session);
        ISession GetSession(string phoneNumber);
        IEnumerable<ISession> GetSessions();
    }
}
