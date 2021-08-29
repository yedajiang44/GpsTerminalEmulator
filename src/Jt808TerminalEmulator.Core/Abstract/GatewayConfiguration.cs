using System;

namespace Jt808TerminalEmulator.Core.Abstract
{
    public class GatewayConfiguration
    {
        public int WebSocketPort { get; set; } = 808;

        public int HttpPort { get; set; } = 801;

        public bool UseLibuv { get; set; }

        public int QuietPeriodSeconds { get; set; } = 100;

        public TimeSpan QuietPeriodTimeSpan => TimeSpan.FromMilliseconds(QuietPeriodSeconds);

        public int ShutdownTimeoutSeconds { get; set; } = 1;

        public TimeSpan ShutdownTimeoutTimeSpan => TimeSpan.FromSeconds(ShutdownTimeoutSeconds);

        public int SoBacklog { get; set; } = 8192;

        public int EventLoopCount { get; set; } = Environment.ProcessorCount;

        public int ReaderIdleTimeSeconds { get; set; } = 3600;

        public int WriterIdleTimeSeconds { get; set; } = 3600;

        public int AllIdleTimeSeconds { get; set; } = 3600;
    }
}