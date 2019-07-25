using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.AIGroup
{
    public class HotAreaManager
    {
        public List<HotArea> hotAreas = new List<HotArea>();

        public void AddArea(HotArea hot)
        {
            if (hotAreas.Count > 10) return;//檢查大小
            hotAreas.Add(hot);
        }
        public void CheckAndMerge()
        {

        }
    }
}
