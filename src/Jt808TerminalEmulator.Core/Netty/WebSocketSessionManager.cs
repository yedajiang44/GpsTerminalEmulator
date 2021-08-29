using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Jt808TerminalEmulator.Core.Netty
{
    public class WebSocketSessionManager
    {
        private readonly ILogger logger;
        private ConcurrentDictionary<string, WebSocketSession> Sessions { get; } = new ConcurrentDictionary<string, WebSocketSession>();
        public int SessionCount => Sessions.Count;
        public WebSocketSessionManager(ILogger<WebSocketSessionManager> logger)
        {
            this.logger = logger;
        }

        public IEnumerable<WebSocketSession> GetAllSessions() => Sessions.Values;

        public bool TryAdd(WebSocketSession session) => Sessions.TryAdd(session.Id, session);
        public bool TryRemove(string sessionId)
        {
            if (Sessions.TryRemove(sessionId, out var session))
            {
                session.Dispose();
                return true;
            };
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
            return Task.Run(() =>
            {
                Sessions.Values.AsParallel().ForEach(x =>
                {
                    Send(x.Id, data);
                });
            });
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
}