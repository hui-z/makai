using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using HuiZ.Makai.Game;
using NLog;
using static HuiZ.Makai.Extensions;

namespace HuiZ.Makai.Modifiers
{
    public class BattleResult : IModifier
    {
        private readonly IRequester _rest;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public BattleResult(IRequester rest)
        {
            _rest = rest;
        }

        public bool CanModify(Context ctx) => ctx.Path == "/asg/battlej/result";

        public dynamic Process(Context ctx, dynamic json)
        {
            Protect(() => SellEquip(ctx, json));
            Protect(() => EnhanceCards(ctx, json));
            return json;
        }

        private void EnhanceCards(Context ctx, dynamic json)
        {
            var cards = json.data.replace[0].t_member_cards;
            foreach(var card in cards)
            {
                long id = card.id;
                int level = card.lv;
                int lvMax = card.lv_max;
                int rare = card.rare;
                if(level == lvMax && rare <= 5)
                    _rest.EnhanceCard(ctx, id).SubscribeWithLog(_logger, "enhance card");
            }
        }

        private void SellEquip(Context ctx, dynamic json)
        {
            var drops = json.data.extra.drop_reward_items;
            foreach(dynamic drop in drops)
            {
                int itemType = drop.item_type;
                if(itemType == 19)
                {
                    long id = drop.id;
                    var epType = new EquipType((long)drop.item_id);
                    if (IsSell(epType))
                    {
                        drops.Remove(drop);
                        json.data.replace[0].t_member_eqs.Clear();
                        _rest.SellEquips(ctx, id).SubscribeWithLog(_logger, "sell equip", epType.ToString());
                    }
                    return;
                }
            }
        }
        private bool IsSell(EquipType ept)
        {
            if (ept.Category == EpCategory.Weapon && ept.Rare >= 7)
            {
                if(ept.Kind == EpKind.CriPer || ept.Kind == EpKind.CriDmg)
                    return false;
            }
            if (ept.Type == EpType.Shoes)
                return false;
            if (ept.Type == EpType.Wing)
                return false;
            if (ept.Type == EpType.Armor)
                return true;
            if (ept.Type == EpType.Gauntlet)
                return true;
            if (ept.Type == EpType.Helmet && ept.Rare >= 6)
                return false;
            if (ept.Type == EpType.MasekiCriPer && ept.Rare >= 6)
                return false;
            if (ept.Type == EpType.MasekiSpd && ept.Rare >= 6)
                return false;
            if (ept.Type == EpType.MasekiCriDmg && ept.Rare >= 6)
                return false;
            if (ept.Rare >= 7)
                return false;
            return true;
        }
    }
}
