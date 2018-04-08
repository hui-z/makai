using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using static HuiZ.Makai.Extensions;

namespace HuiZ.Makai.Modifiers
{
    public class BattleStart : IModifier
    {
        private readonly IRequester _rest;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public BattleStart(IRequester rest)
        {
            _rest = rest;
        }

        public int Priority => 0;

        public bool CanModify(Context ctx) => ctx.Path == "/asg/battlej/start";

        public Reply Process(Context ctx, Reply reply)
        {
            var json = reply.Body;
            if (json.data.error != null)
                return reply;
            Protect(() => RecoveryAp(ctx, json));
            return reply;
        }

        private void RecoveryAp(Context ctx, dynamic json)
        {
            var member = json.data.replace[0].t_members;
            int actOverflow = member.act_overflow;
            int eqMax = member.eq_max;
            if(eqMax >= 400 && actOverflow == 0)
                _rest.Recovery(ctx).SubscribeWithLog(_logger, "recovery ap");
        }
    }
}
