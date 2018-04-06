using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using HuiZ.Makai.Game;

namespace HuiZ.Makai.Modifiers
{
    public class BattleResult : IModifier
    {
        private readonly IRequesterFactory _requesters;

        public BattleResult(IRequesterFactory requesters)
        {
            _requesters = requesters;
        }

        public bool CanModify(Context ctx) => ctx.Path == "/asg/battlej/result";

        public dynamic Process(Context ctx, dynamic json)
        {
            SellEquip(ctx, json);
            return json;
        }

        private void SellEquip(Context ctx, dynamic json)
        {
            try
            {
                var requester = _requesters.Get(ctx.Token);
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
                            requester.SellEquips(id).Subscribe(
                                    result => Console.WriteLine($"[sell equip] {epType}, {result}"),
                                    ex => Console.WriteLine($"[sell equip] {ex.Message}"));
                        }
                        return;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        private bool IsSell(EquipType ept)
        {
            if (ept.Category == EpCategory.Weapon && ept.Rare >= 7)
                return false;
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
            if (ept.Type == EpType.MasekiCriDmg && ept.Rare >= 5)
                return false;
            if (ept.Rare >= 7)
                return false;
            return true;
        }
    }
}
