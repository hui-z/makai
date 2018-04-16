using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuiZ.Makai.Game
{
    public class Card
    {
        public long Id { get; set; }
        public string NickName { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{NickName} {Name}";
        }

        public static Card Unknown => new Card
        {
            Id = -1,
            NickName = "Unknown",
            Name = "Unknown",
        };
    }
}
