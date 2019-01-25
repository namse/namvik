using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace namvik
{
    public static class ScaleBetweenPixelAndMeter
    {
        public static float ScaleFactorPixelToMeter = (5f / 4f) * (49f / 100f) * (1f / 190f);
        public static float ToMeter(this float value)
        {
            return value * ScaleFactorPixelToMeter;
        }
        public static float ToPixel(this float value)
        {
            return value / ScaleFactorPixelToMeter;
        }
    }
}
