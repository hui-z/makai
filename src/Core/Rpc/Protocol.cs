using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuiZ.Makai.Rpc
{
    public static class Protocol
    {
        public const string Worker = "Worker";

        public const string Heartbeat = "HEARTBEAT";
        public const string Disconnect = "DISCONNECT";
        public const string Message = "MESSAGE";

        public static TimeSpan HeartbeatInterval = TimeSpan.FromSeconds(0.1);
        public static TimeSpan HeartbeatExpired = TimeSpan.FromSeconds(0.3);
    }
}
