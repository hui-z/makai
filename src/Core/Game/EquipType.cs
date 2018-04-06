using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuiZ.Makai.Game
{
    public enum EpKind
    {
        Atk = 1,
        Def = 2,
        Spd = 3,
        CriPer = 4,
        CriDmg = 5,
    }
    public enum EpType
    {
        Sword = 1,
        Spear = 2,
        Ax = 3,
        Fist = 4,
        Bow = 5,
        Gun = 6,
        Staff = 7,
        Book = 8,
        Armor = 9,
        Helmet = 10,
        Gauntlet = 11,
        Shoes = 12,
        Muscle = 13,
        Ring = 14,
        Necklace = 15,
        Earring = 16,
        Glasses = 17,
        Wing = 18,
        MasekiHp = 51,
        MasekiAtk = 52,
        MasekiDef = 53,
        MasekiSpd = 54,
        MasekiRes = 55,
        MasekiCriPer = 56,
        MasekiCriDmg = 57,
    }
    public enum EpCategory
    {
        Weapon = 1,
        Guard = 2,
        Accessory = 3,
        Maseki = 4
    }
    public class EquipType
    {
        private readonly long _typeId;
        public static bool IsEquipTypeId(long typeId) => typeId >= 10000 && typeId <= 50000;
        public EquipType(long typeId)
        {
            if (!IsEquipTypeId(typeId)) throw new InvalidOperationException();
            _typeId = typeId;
        }

        public int Rare => Convert.ToInt32(_typeId.ToString().Substring(4, 1));
        public EpKind Kind => (EpKind)Convert.ToInt32(_typeId.ToString().Substring(3, 1));
        public EpType Type => (EpType)Convert.ToInt32(_typeId.ToString().Substring(1, 2));
        public EpCategory Category => (EpCategory)Convert.ToInt32(_typeId.ToString().Substring(0, 1));

        public override string ToString()
        {
            return $"{_typeId}: {Category}, {Type}, {Kind}, {Rare}";
        }
    }
}
