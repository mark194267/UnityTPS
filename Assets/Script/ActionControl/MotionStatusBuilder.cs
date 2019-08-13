using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Assets.Script.ActionControl
{
    class MotionStatusBuilder
    {
        public Dictionary<string, MotionStatus> GetMotionList(string Charactername)
        {
            Dictionary<string, MotionStatus> msDictionary = new Dictionary<string, MotionStatus>();
            XmlDocument doc = new XmlDocument();
            doc.Load("Assets\\data\\StatusByAniName\\" + Charactername + "StatusByAniName.xml");
            XmlNodeList nodeList = doc.SelectNodes("motion")[0].ChildNodes;
            foreach (XmlNode node in nodeList)
            {
                MotionStatus ms = new MotionStatus();
                string actname = node.Attributes["name"].Value;
                ms.camX = (float)Convert.ToDouble(node.Attributes["x"].Value);
                ms.camY = (float)Convert.ToDouble(node.Attributes["y"].Value);
                ms.camZ = (float)Convert.ToDouble(node.Attributes["z"].Value);

                ms.motionAtk = Convert.ToInt16(node.Attributes["motionAtk"].Value);
                ms.motionDef = Convert.ToInt16(node.Attributes["motionDef"].Value);
                ms.motionSpd = Convert.ToInt16(node.Attributes["motionSpd"].Value);
                ms.motionStun = Convert.ToInt16(node.Attributes["motionStun"].Value);
                ms.IsRotH = Convert.ToBoolean(node.Attributes["IsRotH"].Value);
                ms.IsRotV = Convert.ToBoolean(node.Attributes["IsRotV"].Value);

                ms.String = node.Attributes["String"].Value;

                msDictionary.Add(actname, ms);
            }
            return msDictionary;
        }
    }
    public class MotionStatus
    {
        public float camX { get; set; }
        public float camY { get; set; }
        public float camZ { get; set; }

        public int motionAtk { get; set; }
        public int motionDef { get; set; }
        public int motionStun { get; set; }
        public int motionSpd { get; set; }
        public string String { get; set; }

        public bool IsRotH { get; set; }
        public bool IsRotV { get; set; }
    }
}
