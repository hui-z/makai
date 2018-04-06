using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.EventArguments;

namespace HuiZ.Makai.Modifiers
{
    public class Battle : IModifier
    {
        private readonly IRequesterFactory _requesters;

        public Battle(IRequesterFactory requesters)
        {
            _requesters = requesters;
        }

        public bool CanModify(Context ctx) => ctx.Path == "/asg/battlej/ready";

        public dynamic Process(Context ctx, dynamic json)
        {
            var waves = json.data.replace[0].battle.waves;
            foreach(dynamic wave in waves)
            {
                foreach(dynamic monster in wave.monsters)
                {
                    monster.atk = 1;
                    monster.def = 1;
                    monster.hp = 1;
                    monster.spd = 1;
                }
            }
            Console.WriteLine($"[battle ready]: response modified");
            RecoveryAp(ctx);
            return json;
        }

        private void RecoveryAp(Context ctx)
        {
            var requester = _requesters.Get(ctx.Token);
            requester.Recovery().Subscribe(
                result => Console.WriteLine($"[recovery ap] {result}"),
                ex => Console.WriteLine($"[recovery ap] {ex.Message}")
            );
        }
    }
}
