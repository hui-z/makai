using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace HuiZ.Makai.Proxy
{
    public interface IServer : IObservable<Unit>
    {

    }
    public class Server : IServer
    {
        private readonly ProxyServer _proxy = new ProxyServer();
        private readonly IObservable<Unit> _producer;

        public Server(int port)
        {
            var ep = new ExplicitProxyEndPoint(IPAddress.Any, port, true);
            _proxy.AddEndPoint(ep);

            _producer = Observable.Create<Unit>(o =>
            {
                var requests = Observable
                        .FromEventPattern<SessionEventArgs>(_proxy, nameof(_proxy.BeforeRequest))
                        .Do(Console.WriteLine)
                        .Select(_ => Unit.Default);

                _proxy.Start();
                return new CompositeDisposable(
                    requests.Subscribe(o),
                    Disposable.Create(() => _proxy.Stop())
                );
            });

        }

        public IDisposable Subscribe(IObserver<Unit> observer) 
            => _producer.Subscribe(observer);
    }
}
