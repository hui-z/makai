using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Titanium.Web.Proxy.EventArguments;

namespace HuiZ.Makai.Interceptors
{
    public class Default : IInterceptor
    {
        private readonly IEnumerable<IModifier> _modifiers;

        public Default(IEnumerable<IModifier> modifiers)
        {
            _modifiers = modifiers;
        }

        public IObservable<Unit> ProcessRequest(SessionEventArgs e) => Observable.Defer(() =>
        {
            var url = GetUrl(e);
            if (!url.Contains("app.makaiwars-sp.jp"))
                return Nothing();
            var uri = new Uri(url);
            var path = uri.LocalPath;
            var query = uri.Query;
            if(e.WebSession.Request.ContentLength > 0)
            {
                var print = e.GetRequestBody().ToObservable()
                    .Select(Encoding.UTF8.GetString)
                    .Select(Uri.UnescapeDataString)
                    .Do(payload => Console.WriteLine($"[{path}]{query} {payload}"));
                return Return(print);
            }
            Console.WriteLine($"[{path}]{query}");
            return Nothing();
        });

        public IObservable<Unit> ProcessResponse(SessionEventArgs e) => Observable.Defer(() =>
        {
            var url = GetUrl(e);
            if (!url.Contains("app.makaiwars-sp.jp"))
                return Nothing();
            var uri = new Uri(url);
            var path = uri.LocalPath;
            var contentType = e.WebSession.Response.ContentType;
            if (contentType.Contains("json"))
                return ProcessJsonResponse(GetContext(e), e);
            return Nothing();
        });

        private IObservable<Unit> ProcessJsonResponse(Context ctx, SessionEventArgs e)
        {
            if (_modifiers.Where(m => m.CanModify(ctx)).IsEmpty())
                return Nothing();
            var body = e.GetResponseBody().ToObservable();
            var payload = Deserialize<dynamic>(body);
            var modified = payload.Select(p => ProcessModify(ctx, p))
                .Select(Serialize)
                .Do(e.SetResponseBody);
            return Return(modified);
        }

        private object ProcessModify(Context ctx, object json) 
            => _modifiers 
            .Where(m => m.CanModify(ctx))
            .Aggregate(json, (acc, m) => m.Process(ctx, acc));

        private IObservable<T> Deserialize<T>(IObservable<byte[]> source)
        {
            var json = source.Select(bytes => Encoding.UTF8.GetString(bytes));
            return json.Select(JsonConvert.DeserializeObject<T>);
        }
        private byte[] Serialize(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(json);
        }
        private IObservable<Unit> Return<T>(IObservable<T> source)
            => source.Select(_ => Unit.Default).IgnoreElements().DefaultIfEmpty();
        private IObservable<Unit> Nothing() => Observable.Return(Unit.Default);

        private string GetUrl(SessionEventArgs e) => e.WebSession.Request.Url;
        private Context GetContext(SessionEventArgs e)
        {
            return new Context
            {
                Url = e.WebSession.Request.Url,
                Token = e.WebSession.Request.Headers.GetFirstHeader("X-TOKEN").Value,
                Path = new Uri(e.WebSession.Request.Url).LocalPath,
            };
        }
    }
}
