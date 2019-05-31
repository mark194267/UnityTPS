using Assets.Script.weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Assets.Script.Avater
{
    public class AvaterDataLoader
    {
        public AvaterStatus LoadStatus(string avaterName)
        {
            AvaterStatus avaterStatus = new AvaterStatus();
            XmlDocument doc = new XmlDocument();
            doc.Load("Assets\\data\\avater\\" + avaterName + "Status.xml");
            XmlNode node = doc.SelectSingleNode(avaterName);
            avaterStatus.Hp = Convert.ToInt16(node.SelectSingleNode("hp").Attributes.GetNamedItem("value").Value);
            avaterStatus.Stun = Convert.ToDouble(node.SelectSingleNode("stun").Attributes.GetNamedItem("value").Value);
            avaterStatus.Atk = Convert.ToInt16(node.SelectSingleNode("atk").Attributes.GetNamedItem("value").Value);
            return avaterStatus;
        }
        /*
        public PlayerStatus LoadPlayerStatus(string avaterName)
        {
            PlayerStatus avaterStatus = new PlayerStatus();
            XmlDocument doc = new XmlDocument();
            doc.Load("Assets\\data\\avater\\" + avaterName + "Status.xml");
            XmlNode node = doc.SelectSingleNode(avaterName);
            avaterStatus.Hp = Convert.ToInt32(node.SelectSingleNode("hp").Attributes.GetNamedItem("value").Value);
            avaterStatus.Stun = Convert.ToDouble(node.SelectSingleNode("stun").Attributes.GetNamedItem("value").Value);
            avaterStatus.Atk = Convert.ToInt32(node.SelectSingleNode("atk").Attributes.GetNamedItem("value").Value);
            foreach (XmlNode nodenode in node.SelectNodes("Ammo"))
            {
                Ammo ammo;
                ammo.Type = nodenode.Attributes.GetNamedItem("type").Value;
                ammo.MaxAmmo = Convert.ToInt32(nodenode.Attributes.GetNamedItem("MaxAmmo").Value);
                ammo.NowAmmo = Convert.ToInt32(nodenode.Attributes.GetNamedItem("NowAmmo").Value);
            }
            return avaterStatus;
        }
        */
    }
}
