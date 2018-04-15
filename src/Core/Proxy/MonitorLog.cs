using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Unosquare.Labs.EmbedIO.Modules;
using Unosquare.Net;

namespace HuiZ.Makai.Proxy
{
    public class MonitorLog : WebSocketsServer
    {
        private readonly ISubject<WebSocketContext> _connected = new Subject<WebSocketContext>();
        private readonly ISubject<WebSocketContext> _disconnected = new Subject<WebSocketContext>();
        private static readonly ISubject<string> Logs = new ReplaySubject<string>();

        public static void OnLogReceived(string record) => Logs.OnNext(record);

        public MonitorLog() : base(true)
        {
            _connected
                .GroupByUntil(ctx => ctx, ctx => _disconnected
                    .Where(x => x == ctx.Key))
                .SelectMany(g => Observable
                    .Using(() => ListenLogs(g.Key), _ => g))
                .Subscribe();
        }
        private IDisposable ListenLogs(WebSocketContext ctx) 
            => Logs.Subscribe(log => Send(ctx, log));

        protected override void OnMessageReceived(WebSocketContext context, byte[] rxBuffer, WebSocketReceiveResult rxResult)
            => Expression.Empty();

        protected override void OnFrameReceived(WebSocketContext context, byte[] rxBuffer, WebSocketReceiveResult rxResult)
            => Expression.Empty();

        protected override void OnClientConnected(WebSocketContext context)
            => _connected.OnNext(context);

        protected override void OnClientDisconnected(WebSocketContext context)
            => _disconnected.OnNext(context);

        public override string ServerName => "Monitor Log Server";
    }

}
