using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.Script.Avater
{
    class AvaterDataLoader
    {
        public AvaterStatus LoadStatus(string avaterName)
        {
            AvaterStatus avaterStatus = new AvaterStatus();
            XmlDocument doc = new XmlDocument();
            doc.Load("Assets\\data\\avater\\" + avaterName + "Status.xml");
            XmlNode node = doc.SelectSingleNode(avaterName);
            avaterStatus.Hp = Convert.ToInt16(node.SelectSingleNode("hp").Attributes.GetNamedItem("value").Value);
            avaterStatus.MaxStun = Convert.ToDouble(node.SelectSingleNode("maxstun").Attributes.GetNamedItem("value").Value);
            avaterStatus.MaxStun = Convert.ToDouble(node.SelectSingleNode("nowstun").Attributes.GetNamedItem("value").Value);
            avaterStatus.Atk = Convert.ToInt16(node.SelectSingleNode("atk").Attributes.GetNamedItem("value").Value);
            return avaterStatus;
        }
    }
}
