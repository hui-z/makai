using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using LazyCache;
using Newtonsoft.Json;
using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;
using RestSharp;

namespace HuiZ.Makai
{
    public interface IRequester
    {
        IObservable<string> Recovery(Context ctx);
        IObservable<string> SellEquips(Context ctx, params long[] ids);
        IObservable<string> EnhanceCard(Context ctx, long id);
    }
    public class Requester : IRequester
    {
        private readonly RestClient _rest = new RestClient("https://app.makaiwars-sp.jp");

        public IObservable<string> Recovery(Context ctx) => ApiCall(ctx, "asg/shopj/recovery", Method.GET);

        public IObservable<string> SellEquips(Context ctx, params long[] ids)
        {
            var payload = new
            {
                t_member_eq_id = ids.Select(id => id.ToString()).ToList()
            };
            return ApiCall(ctx, "asg/eqj/sell", Method.POST, payload);
        }

        public IObservable<string> EnhanceCard(Context ctx, long id)
        {
            var payload = new
            {
                t_member_card_id = id,
                ops = new []
                {
                    new [] { 2, 0, 1 }
                }
            };
            return ApiCall(ctx, "asg/mycardj/enh", Method.POST, payload);
        }

        private IObservable<T> Call<T>(RestRequest req)
            where T : new()
        {
            var async = new AsyncSubject<T>();
            var handle = _rest.ExecuteAsync(req, rep =>
            {
                if (rep.ErrorException != null)
                    async.OnError(rep.ErrorException);
                else
                {
                    var json = Encoding.UTF8.GetString(rep.RawBytes);
                    dynamic obj = JsonConvert.DeserializeObject(json);
                    async.OnNext(obj);
                    async.OnCompleted();
                }
            });
            return async.AsObservable();
        }

        private IObservable<string> ApiCall(Context ctx, string path, Method method, object payload = null)
        {
            var request = new RestRequest(path, method);
            request.AddHeader("X-TOKEN", ctx.Token);
            request.AddHeader("X-OS-TYPE", ctx.OsType);
            if(payload != null)
                request.AddParameter("payload", JsonConvert.SerializeObject(payload));
            return Call<dynamic>(request).Select(ResultInfo);
        }

        private string ResultInfo(dynamic r)
        {
            dynamic error = r?.data?.error;
            if (error == null)
                return $"code: {r?.error_cd}";
            return $"code: {r?.error_cd}, {error.message}, {error.system}";
        }
    }

}
