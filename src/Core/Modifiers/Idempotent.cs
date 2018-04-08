using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Force.DeepCloner;
using LazyCache;

namespace HuiZ.Makai.Modifiers
{
    public class Idempotent : IModifier
    {
        private readonly IAppCache _cache;

        public Idempotent(IAppCache cache)
        {
            _cache = cache;
        }

        public int Priority => int.MaxValue;

        public bool CanModify(Context ctx) => new[]
        {
            "/asg/battlej/result"
        }.Contains(ctx.Path);

        public Reply Process(Context ctx, Reply reply)
        {
            var key = ctx.Url + ctx.Token + ctx.RequestBody.Wait();
            return _cache.GetOrAdd(key, reply.DeepClone);
            //var result = _cache.GetOrAdd(key, () => json, TimeSpan.FromMinutes(5));
        }
    }
}
