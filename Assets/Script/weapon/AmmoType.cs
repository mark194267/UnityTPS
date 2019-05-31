using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.weapon
{
    public class AllAmmoType
    {
        public string Type;
        public int NowAmmo;
        public int MaxAmmo;

        public List<Ammo> GetAmmoType()
        {
            List<Ammo> list = new List<Ammo>();
            Ammo ammo1 = new Ammo { Type = "bullet", MaxAmmo = 99999, NowAmmo = 99999 };
            list.Add(ammo1);
            Ammo ammo2 = new Ammo { Type = "rocket", MaxAmmo = 99999, NowAmmo = 99999 };
            list.Add(ammo2);
            Ammo ammo3 = new Ammo { Type = "sword", MaxAmmo = 999, NowAmmo = 99999 };
            list.Add(ammo3);

            Type = ammo1.Type;
            NowAmmo = ammo1.NowAmmo;
            MaxAmmo = ammo1.MaxAmmo;

            return list;
        }
    }

    public class Ammo
    {
        public string Type;
        public int MaxAmmo;
        public int NowAmmo;
    }
}
