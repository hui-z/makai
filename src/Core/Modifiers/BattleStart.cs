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

        public bool CanModify(Context ctx) => ctx.Path == "/asg/battlej/start";

        public dynamic Process(Context ctx, dynamic json)
        {
            Protect(() => RecoveryAp(ctx, json));
            return json;
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
