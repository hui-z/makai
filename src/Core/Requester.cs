using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ninject;
using Ninject.Parameters;
using Ninject.Syntax;
using RestSharp;

namespace HuiZ.Makai
{
    public interface IRequester
    {
        IObservable<string> Recovery();
    }
    public class User { }
    public class Requester : IRequester
    {
        private RestClient _rest = new RestClient("https://app.makaiwars-sp.jp");
        private readonly string _token;

        public Requester(string token)
        {
            _token = token;
        }

        public IObservable<string> Recovery()
        {
            var request = new RestRequest("asg/shopj/recovery", Method.GET);
            request.AddHeader("X-TOKEN", _token);
            request.AddHeader("X-OS-TYPE", "2");
            return Call<dynamic>(request).Select(r => $"error_cd: {r.error_cd}");
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
    }

    public interface IRequesterFactory
    {
        IRequester Get(string token);
    }
    public class RequesterFactory : IRequesterFactory
    {
        private readonly IResolutionRoot _root;

        public RequesterFactory(IResolutionRoot root)
        {
            _root = root;
        }

        public IRequester Get(string token)
        {
            return _root.Get<IRequester>(new ConstructorArgument(nameof(token), token));
        }
    }
}
