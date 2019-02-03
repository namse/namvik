using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace namvik
{
    public static class FloatExtension
    {
        public static float NextFloat(this Random random, double min, double max)
        {
            var randomValue = random.NextDouble();
            if (min > max)
            {
                throw new ArgumentException("min must be smaller than max");
            }

            return (float) (randomValue * (max - min) + min);
        }
    }
}
