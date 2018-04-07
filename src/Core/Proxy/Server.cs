using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using NLog;
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
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public Server(int port, bool fiddler, IInterceptor interceptor)
        {
            _proxy.CertificateManager.SaveFakeCertificates = true;
            if (fiddler)
            {
                _proxy.UpStreamHttpsProxy = new ExternalProxy
                {
                    HostName = "127.0.0.1",
                    Port = 8888,
                    BypassLocalhost = true,
                };
                _logger.Trace("use fiddler as upstream proxy");
            }

            _producer = Observable.Create<Unit>(o =>
            {
                //_proxy.ExceptionFunc = ex => Console.WriteLine(ex.Message);
                _proxy.BeforeRequest += (s, e) => interceptor.ProcessRequest(e).ToTask();
                _proxy.BeforeResponse += (s, e) => interceptor.ProcessResponse(e).ToTask();
                _proxy.ServerCertificateValidationCallback += OnCertificateValidsation;
                _proxy.ClientCertificateSelectionCallback += OnClientCertificateSelectionCallback;

                var endPoint = new ExplicitProxyEndPoint(IPAddress.Any, port, true);
                _proxy.AddEndPoint(endPoint);
                
                _proxy.Start();
                _proxy.ProxyEndPoints.ForEach(ep => _logger.Info($"makai hack server listening on {ep.IpAddress}:{ep.Port}"));
                return new CompositeDisposable(
                    Disposable.Create(() => _proxy.Stop())
                );
            });

        }

        private Task OnClientCertificateSelectionCallback(object arg1, CertificateSelectionEventArgs arg2)
        {
            return Task.FromResult(0);
        }

        private Task OnCertificateValidsation(object sender, CertificateValidationEventArgs e)
        {
            if (e.SslPolicyErrors == SslPolicyErrors.None)
                e.IsValid = true;
            return Task.FromResult(0);
        }

        public IDisposable Subscribe(IObserver<Unit> observer) 
            => _producer.Subscribe(observer);
    }
}
