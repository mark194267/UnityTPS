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
        public int maxSize = 20;
        public List<HotArea> hotAreas = new List<HotArea>();

        public void AddArea(HotArea hot)
        {
            if (hotAreas.Count > maxSize)
                DeleteOldest();//檢查大小
            hotAreas.Add(hot);
            hot.hotAreaManager = this;
        }
        public void DeleteOldest()
        {
            //Debug.Log("Try delete oldest");
            var hot = hotAreas.First();
            hot.DeleteMe();
            hotAreas.Remove(hot);
        }
        public void DeleteHotArea(HotArea hot)
        {
            hotAreas.Remove(hot);
            hot.DeleteMe();
        }
    }
}
