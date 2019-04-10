using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Avater
{
    public class AvaterStatus
    {
        public int Hp { get; set; }
        public int Atk { get; set; }

        public double MaxStun { get; set; }
        public double NowStun { get; set; }
    }
}
