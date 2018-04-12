using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Force.DeepCloner;
using NLog;
using static HuiZ.Makai.Extensions;

namespace HuiZ.Makai.Modifiers
{
    public class BattleReady : IModifier
    {
        private readonly IRequester _rest;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public BattleReady(IRequester rest)
        {
            _rest = rest;
        }

        public int Priority => 0;

        public bool CanModify(Context ctx) => ctx.Path == "/asg/battlej/ready";

        public Reply Process(Context ctx, Reply reply)
        {
            reply = reply.DeepClone();
            var json = reply.Body;
            if (IsError(json))
                return reply;
            var waves = json.data.replace[0].battle.waves;
            foreach(var wave in waves)
            {
                foreach(var monster in wave.monsters)
                {
                    monster.atk = 1;
                    monster.def = 1;
                    monster.hp = 1;
                    monster.spd = 1;
                }
            }
            _logger.Info($"[battle ready]: weak monster applied");
            return reply;
        }
    }
}
