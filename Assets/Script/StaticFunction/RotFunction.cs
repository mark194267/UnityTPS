using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.StaticFunction
{
    public static class RotFunction
    {
        public static float Clamp180(float Num)
        {
            if (Num < -180)
            {
                Num += 360;
            }
            if (Num > 180)
            {
                Num -= 360;
            }
            return Num;
        }
    }
}
