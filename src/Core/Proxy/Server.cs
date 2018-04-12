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
        private readonly ProxyServer _proxy = new ProxyServer { ProxyRealm = "makai" };
        private readonly IObservable<Unit> _producer;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public Server(Options opt, IInterceptor interceptor)
        {
            _proxy.CertificateManager.SaveFakeCertificates = true;
            if (opt.Fiddler)
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
                //_proxy.ExceptionFunc = ex => Console.WriteLine(ex);
                _proxy.BeforeRequest += (s, e) => interceptor.ProcessRequest(e).ToTask();
                _proxy.BeforeResponse += (s, e) => interceptor.ProcessResponse(e).ToTask();
                _proxy.ServerCertificateValidationCallback += OnCertificateValidsation;
                _proxy.ClientCertificateSelectionCallback += OnClientCertificateSelection;
                if(opt.Authentication)
                    _proxy.AuthenticateUserFunc += Authenticate;

                var endPoint = new ExplicitProxyEndPoint(IPAddress.Any, opt.Port, true);
                endPoint.BeforeTunnelConnectRequest += BeforeTunnelConnectRequest;
                _proxy.AddEndPoint(endPoint);
                
                _proxy.Start();
                _proxy.ProxyEndPoints.ForEach(ep => _logger.Info($"makai hack server listening on {ep.IpAddress}:{ep.Port}"));
                return new CompositeDisposable(
                    Disposable.Create(() => _proxy.Stop())
                );
            });

        }

        private Task BeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e)
        {
            if (e.WebSession.Request.RequestUri.Host != "app.makaiwars-sp.jp" &&
                e.WebSession.Request.RequestUri.Host != "210.140.214.150")
                e.Excluded = true;
            return Task.FromResult(0);
        }

        private Task<bool> Authenticate(string user, string pass) => Observable.Return(Auth(user, pass)).ToTask();

        private bool Auth(string user, string pass)
        {
            if (user == "hulucc" && pass == "makai")
                return true;
            if (user == "makai" && pass == "makai")
                return true;
            return false;
        }

        private Task OnClientCertificateSelection(object arg1, CertificateSelectionEventArgs arg2)
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
