using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Unosquare.Labs.EmbedIO.Modules;
using Unosquare.Net;

namespace HuiZ.Makai.Proxy
{
    public class MonitorLog : WebSocketsServer
    {
        public MonitorLog() : base(true, 0)
        {
        }

        protected override void OnMessageReceived(WebSocketContext context, byte[] rxBuffer, WebSocketReceiveResult rxResult)
            => Expression.Empty();

        protected override void OnFrameReceived(WebSocketContext context, byte[] rxBuffer, WebSocketReceiveResult rxResult) 
            => Expression.Empty();

        protected override void OnClientConnected(WebSocketContext context)
        {
            throw new NotImplementedException();
        }

        protected override void OnClientDisconnected(WebSocketContext context)
        {
            throw new NotImplementedException();
        }

        public override string ServerName => "Monitor Log Server";
    }
}
