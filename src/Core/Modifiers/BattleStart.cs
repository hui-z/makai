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
        private readonly Options _opt;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public BattleStart(IRequester rest, Options opt)
        {
            _rest = rest;
            _opt = opt;
        }

        public int Priority => 0;

        public bool CanModify(Context ctx) => ctx.Path == "/asg/battlej/start";

        public Reply Process(Context ctx, Reply reply)
        {
            var json = reply.Body;
            if (json.data.error != null)
                return reply;
            Protect(() => ProcessMember(ctx, json));
            return reply;
        }

        private void ProcessMember(Context ctx, dynamic json)
        {
            var member = json.data.replace[0].t_members;
            int max = member.act_max;
            DateTime maxDatetime = member.act_max_date;
            int overflow = member.act_overflow;
            var now = GetCurrentTokyoDateTime();
            var ap = overflow > 0 ? max + overflow : Convert.ToInt32(max - (maxDatetime - now).TotalMinutes / 3);

            string name = member.name;
            int level = member.lv;
            _logger.Info($"[{name}]: lv {level}, ap {ap}");
            if (ap < 10) RecoveryAp(ctx);
        }
        private DateTime GetCurrentTokyoDateTime()
        {
            var utc = DateTime.UtcNow;
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            var tokyo = TimeZoneInfo.ConvertTimeFromUtc(utc, tz);
            return tokyo;
        }

        private void RecoveryAp(Context ctx)
        {
            if (_opt.ItemRecovery)
                _rest.ItemRecovery(ctx).SubscribeWithLog(_logger, "recovery ap by item");
            else
                _rest.Recovery(ctx).SubscribeWithLog(_logger, "recovery ap");
        }

    }
}
