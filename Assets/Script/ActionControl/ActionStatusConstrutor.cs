using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEngine;

namespace Assets.Script.ActionControl
{
    public class ActionStatusDictionary
    {
        //初始化
        private string Charactername;
        //1.工廠模式返回 2.字典集
        private ActionBasic ActionBasic;

        private List<ActionStatus> _moveList;
        private List<ActionStatus> _attackList;
        private List<ActionStatus> _specialList;

        public Dictionary<string, ActionStatus> AllActionStatusDictionary;
        public Dictionary<string, ActionStatus> MoveDictionary;
        public Dictionary<string, ActionStatus> CloseRangeDictionary;
        public Dictionary<string, ActionStatus> LongRangeDictionary;
        //actionScript.ActionStatus = actionStatuses.Find(x => x.ActionName == name);

        /// <summary>
        /// 創造腳色動作資料庫，完成後透過ApplyAction來
        /// </summary>
        /// <param name="charactername"></param>
        /// 
        public void Init(string charactername)
        {
            this.Charactername = charactername;

            var CommonMoveDictionary = LoadAttributesDictionary("move", "Common");
            var CommonCloseRangeDictionary = LoadAttributesDictionary("close", "Common");
            var CommonLongRangeDictionary = LoadAttributesDictionary("long", "Common");

            this.MoveDictionary = LoadAttributesDictionary("move",Charactername);
            this.CloseRangeDictionary = LoadAttributesDictionary("close", Charactername);
            this.LongRangeDictionary = LoadAttributesDictionary("long", Charactername);

            this.AllActionStatusDictionary = 
                MoveDictionary.Concat(CloseRangeDictionary).Concat(LongRangeDictionary)
                .Concat(CommonMoveDictionary).Concat(CommonCloseRangeDictionary).Concat(CommonLongRangeDictionary)
                .GroupBy(d => d.Key).ToDictionary(d => d.Key, d => d.First().Value);
        }

        //將XML的值載入人物的各動作表(數值)
        private Dictionary<string,ActionStatus> LoadAttributesDictionary(string type,string Charactername)
        {
            Dictionary<string, ActionStatus> actDictionary = new Dictionary<string, ActionStatus>();
            XmlDocument doc = new XmlDocument();
            doc.Load("Assets\\data\\actions\\" + Charactername + "Action.xml");
            XmlNodeList nodeList = doc.SelectNodes(Charactername + "/" + type);

            foreach (XmlNode node in nodeList)
            {
                string actname = node.Attributes["name"].Value;
                float f1 = (float)Convert.ToDouble(node.SelectSingleNode("f1").Attributes.GetNamedItem("value").Value);
                float f2 = (float)Convert.ToDouble(node.SelectSingleNode("f2").Attributes.GetNamedItem("value").Value);
                float f3 = (float)Convert.ToDouble(node.SelectSingleNode("f3").Attributes.GetNamedItem("value").Value);
                float f4 = (float)Convert.ToDouble(node.SelectSingleNode("f4").Attributes.GetNamedItem("value").Value);
                float x = (float)Convert.ToDouble(node.SelectSingleNode("vector3").Attributes.GetNamedItem("x").Value);
                float y = (float)Convert.ToDouble(node.SelectSingleNode("vector3").Attributes.GetNamedItem("y").Value);
                float z = (float)Convert.ToDouble(node.SelectSingleNode("vector3").Attributes.GetNamedItem("z").Value);
                string ignore = node.SelectSingleNode("ignore").Attributes.GetNamedItem("list").Value;
                string[] ignorelist = ignore.Split(',');

                ActionStatus ac = new ActionStatus();

                ac.ActionName = actname;
                ac.f1 = f1;
                ac.f2 = f2;
                ac.f3 = f3;
                ac.f4 = f4;

                if (ignore != "0")
                {
                    ac.ignorelist = ignorelist;
                }
                actDictionary.Add(actname,ac);
            }
            return actDictionary;
        }

        public ActionStatus FindByNameDic(string name, string type)
        {
            switch (type)
            {
                case "move":
                    return MoveDictionary[name];
                case "close":
                    return CloseRangeDictionary[name];
                case "long":
                    return LongRangeDictionary[name];
            }

            return null;
        }
    }

    /// <summary>
    /// 動作列表，未來加上一個virturltype在上面作為工廠模式原始
    /// </summary>
    public class ActionStatus
    {
        public string ActionName { get; set; }
        public float f1 { get; set; }
        public float f2 { get; set; }
        public float f3 { get; set; }
        public float f4 { get; set; }
        public Vector3 Vector3 { get; set; }
        public string[] ignorelist { get; set; }
    }
}
