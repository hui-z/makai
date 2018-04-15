using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;

namespace HuiZ.Makai.Proxy
{
    public interface IMonitor : IObservable<Unit>
    {

    }
    public class Monitor : IMonitor
    {
        private readonly IObservable<Unit> _producer;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        public Monitor(Options opt, MonitorLog mlog)
        {
            _producer = Observable.Using(() => new WebServer(opt.MonitorPort), server => Observable.Create<Unit>(o =>
            {
                _logger.Info($"starting monitor at 0.0.0.0:{opt.MonitorPort}");
                server.RegisterModule(new WebSocketsModule());
                server.Module<WebSocketsModule>().RegisterWebSocketsServer("/log", mlog);
                server.RunAsync().ToObservable().Subscribe(o);
                return Disposable.Empty;
            }));
        }

        public IDisposable Subscribe(IObserver<Unit> observer) 
            => _producer.Subscribe(observer);
    }
}
