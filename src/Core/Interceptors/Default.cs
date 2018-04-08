using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using Titanium.Web.Proxy.EventArguments;

namespace HuiZ.Makai.Interceptors
{
    public class Default : IInterceptor
    {
        private readonly IEnumerable<IModifier> _modifiers;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

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
            if (e.WebSession.Request.ContentLength > 0)
            {
                var print = e.GetRequestBody().ToObservable()
                    .Select(Encoding.UTF8.GetString)
                    .Select(Uri.UnescapeDataString)
                    .Do(payload => _logger.Trace($"[{path}]{query} {payload}"));
                return Return(print);
            }
            _logger.Trace($"[{path}]{query}");
            return Nothing();
        }).Catch((Exception ex) =>
        {
            _logger.Error(ex);
            return Nothing();
        }); 

        public IObservable<Unit> ProcessResponse(SessionEventArgs e) => Observable.Defer(() =>
        {
            var url = GetUrl(e);
            if (!url.Contains("app.makaiwars-sp.jp"))
                return Nothing();
            var uri = new Uri(url);
            var path = uri.LocalPath;
            var contentType = e.WebSession.Response.ContentType ?? "";
            if (contentType.Contains("json"))
                return ProcessJsonResponse(GetContext(e), e);
            return Nothing();
        }).Catch((Exception ex) =>
        {
            _logger.Error(ex);
            return Nothing();
        });

        private IObservable<Unit> ProcessJsonResponse(Context ctx, SessionEventArgs e)
        {
            if (GetModifiers(ctx).IsEmpty())
                return Nothing();
            var body = e.GetResponseBody().ToObservable();
            var payload = Deserialize<dynamic>(body);
            var modified = payload
                .Select(p => ProcessModify(ctx, new Reply
                {
                    Body = p,
                    Code =e.WebSession.Response.StatusCode
                }))
                .Do(reply =>
                {
                    e.WebSession.Response.StatusCode = reply.Code;
                    e.SetResponseBody(Serialize(reply.Body));
                });
            return Return(modified);
        }

        private IEnumerable<IModifier> GetModifiers(Context ctx)
            => _modifiers.Where(m => m.CanModify(ctx));

        private Reply ProcessModify(Context ctx, Reply reply)
        {
            return GetModifiers(ctx)
                .OrderBy(m => m.Priority)
                .Aggregate(reply, (acc, m) => m.Process(ctx, acc));
        }

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
                OsType = e.WebSession.Request.Headers.GetFirstHeader("X-OS-TYPE").Value,
                Path = new Uri(e.WebSession.Request.Url).LocalPath,
                RequestBody = e.GetRequestBodyAsString().ToObservable(),
            };
        }
    }
}
