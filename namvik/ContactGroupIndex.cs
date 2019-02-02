using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace namvik
{
    public static class ContactGroupIndex
    {
        public static short Monster = (short)Math.Pow(2, 0);
        public static short Character = (short)Math.Pow(2, 1);
        public static short Armor = (short)Math.Pow(2, 2);
    }

    public enum CategoryBits
    {
        Unknown = 0b0,
        Ground = 0b1,
        Monster = 0b10,
        Character = 0b100,
        Armor = 0b1000,
    }

    public enum MaskBits
    {
        Unknown = 0b0,
        Ground = 0b1111_1111_1111_1111,
        Monster = CategoryBits.Ground | CategoryBits.Character,
        Character = CategoryBits.Ground | CategoryBits.Monster,
        Armor = CategoryBits.Ground,
    }
}
