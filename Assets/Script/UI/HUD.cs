using UnityEngine;
using UnityEngine.UI;
using Assets.Script.Avater;
using System.Collections.Generic;
namespace Assets.Script.UI
{
    public class HUD :MonoBehaviour
    {
        public int slotNum;

        public Slider HealthBar;
        public Button Weapon1;
        public Button Weapon2;
        public Button Weapon3;
        public Button Weapon4;


        public void Setup(int MaxHealth,int hp)
        {
            HealthBar.maxValue = MaxHealth;
            HealthBar.value = hp;
        }
        public void ChangeHealth(int hp)
        {
            HealthBar.value = hp;
        }
        public void ChangeWeapon(int weaponNum)
        {
            switch (weaponNum)
            {
                case 1:
                    Weapon1.image.color = Color.red;
                    Weapon2.image.color = Color.white;
                    Weapon3.image.color = Color.white;
                    Weapon4.image.color = Color.white;
                    break;
                case 2:
                    Weapon1.image.color = Color.white;
                    Weapon2.image.color = Color.red;
                    Weapon3.image.color = Color.white;
                    Weapon4.image.color = Color.white;
                    break;
                case 3:
                    Weapon1.image.color = Color.white;
                    Weapon2.image.color = Color.white;
                    Weapon3.image.color = Color.red;
                    Weapon4.image.color = Color.white;
                    break;
                case 4:
                    Weapon1.image.color = Color.white;
                    Weapon2.image.color = Color.white;
                    Weapon3.image.color = Color.white;
                    Weapon4.image.color = Color.red;
                    break;
            }
        }
    }
}
