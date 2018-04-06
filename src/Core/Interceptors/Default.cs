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

        public IObservable<Unit> ProcessRequest(SessionEventArgs e) => Observable.Defer(() =>
        {
            var url = GetUrl(e);
            if (!url.Contains("app.makaiwars-sp.jp"))
                return Observable.Return(Unit.Default);
            //Console.WriteLine(url);
            return Observable.Return(Unit.Default);
        });

        public IObservable<Unit> ProcessResponse(SessionEventArgs e) => Observable.Defer(() =>
        {
            var url = GetUrl(e);
            if (!url.Contains("app.makaiwars-sp.jp"))
                return Nothing();
            var uri = new Uri(url);
            var path = uri.LocalPath;
            Console.WriteLine(path);
            var contentType = e.WebSession.Response.ContentType;
            if (contentType.Contains("json"))
                return ProcessJsonResponse(path, e);
            return Nothing();
        });

        private IObservable<Unit> ProcessJsonResponse(string path, SessionEventArgs e)
        {
            Console.WriteLine(path);
            if(path == "/asg/battlej/ready")
            {
                var body = e.GetResponseBody().ToObservable();
                var payload = Deserialize<dynamic>(body);
                var modified = payload.Select(ModifyBattleReady)
                    .Select(Serialize)
                    .Do(e.SetResponseBody);
                return Return(modified);
            }
            return Nothing();
        }

        private dynamic ModifyBattleReady(dynamic json)
        {
            var waves = json.data.replace[0].battle.waves;
            foreach(dynamic wave in waves)
            {
                foreach(dynamic monster in wave.monsters)
                {
                    monster.atk = 1;
                    monster.hp = 1;
                    monster.spd = 1;
                }
            }
            return json;
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
    }
}
