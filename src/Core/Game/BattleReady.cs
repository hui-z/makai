using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuiZ.Makai.Game
{
    public class BattleReady
    {
        public object extra { get; set; }
        public List<Replace> replace { get; set; }
    }

    public class Replace
    {
        public object t_member_cards { get; set; }
        public object t_member_eqs { get; set; }
        public object t_member_event_stages { get; set; }
        public object t_member_formation { get; set; }
        public object t_member_id { get; set; }
        public object t_member_story { get; set; }
    }
}
