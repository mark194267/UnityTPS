using Assets.Script.Avater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Config
{
    public class InputManager : MonoBehaviour
    {
        public float ws;
        public float ad;
        public float maxWSAD;
        public float charge;//蓄力

        public int WNumber;
        public float W2RunTimer;

        public int weaponSlot { get; set; }//武器欄

        public Animator Animator;

        void Start()
        {
            Animator = gameObject.GetComponent<Animator>();
        }

        void Update()
        {
            ws = Input.GetAxis("ws");
            ad = Input.GetAxis("ad");

            maxWSAD = Mathf.Max(Math.Abs(ws),Math.Abs(ad));
            Animator.SetFloat("input_wsad", maxWSAD);
            Animator.SetFloat("input_ws", ws);
            Animator.SetFloat("input_ad", ad);

            #region Run
            if (Input.GetKeyDown("w"))
            {
                WNumber++;
                if (WNumber > 1)
                {
                    Animator.SetBool("input_run",true);
                }
            }

            if (WNumber == 1)
            {
                W2RunTimer += Time.deltaTime;
                if (W2RunTimer > 0.5)//連按的最大秒數
                {
                    WNumber = 0;
                    W2RunTimer = 0;
                }
            }
            if (Animator.GetBool("input_run"))
            {
                if (Input.GetKeyUp("w"))
                {
                    Animator.SetBool("input_run", false);
                    WNumber = 0;
                    W2RunTimer = 0;
                }
            }
            #endregion

            /*
            if (ws*ws+ad*ad != 0)
            {
                Animator.SetBool("input_move",true);
            }
            else
            {
                Animator.SetBool("input_move", false);
            }
            */
            if (Input.GetButtonDown("Reload"))
            {
                Animator.SetTrigger("input_reload");
            }

            if (Input.GetButtonDown("Jump"))
            {
                Animator.SetBool("input_jump",true);
                ResetToggle();
            }
            else
            {
                Animator.SetBool("input_jump", false);
            }

            if (Animator.GetInteger("avater_weaponslot") != 1)
            {
                if (Input.GetButton("Fire1"))
                {
                    Animator.SetBool("input_fire", true);
                }
                else
                {
                    Animator.SetBool("input_fire", false);
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    Animator.SetTrigger("input_Light");
                }
            }

            #region 蓄力
            if (Input.GetButtonUp("Fire1"))
            {
                //Animator.SetFloat("input_charge", charge);
                //charge = 0;
            }
            if (Input.GetButton("Fire1"))
            {
                //charge += Time.deltaTime;
            }
            #endregion

            if (Input.GetButton("Sprint"))
            {
                Animator.SetBool("input_dodge",true);
                ResetToggle();
            }
            else
            {
                Animator.SetBool("input_dodge", false);
            }

            if (Input.GetButton("Defend"))
            {
                Animator.SetBool("input_defend", true);
                ResetToggle();
            }
            else
            {
                Animator.SetBool("input_defend", false);
            }
            /*
            if (Input.GetButtonDown("holster"))
            {
                GetComponent<PlayerAvater>().ChangeWeapon(0);
            }
            */
            /*
            if(Input.GetButton("Fire2"))
            {
                Animator.SetTrigger("input_melee");
                //Animator.SetTrigger("avatermain_panicmelee");
            }
            */
            if (Input.GetButtonDown("SlowMo"))
            {
                GetComponent<PlayerAvater>().SlowMo();
            }


            if (Input.GetButton("Grenade"))
            {
                Animator.SetBool("input_grenade",true);
            }
            else
                Animator.SetBool("input_grenade", false);

            if (Input.GetButtonDown("Fire2"))
            {
                Animator.SetTrigger("input_Heavy");
                //Animator.SetTrigger("avatermain_panicmelee");
                var anifloat = Animator.GetFloat("input_meleelevel");
                if (anifloat < 5)
                {
                    Animator.SetFloat("input_meleelevel", anifloat + 1);
                }
                else Animator.SetFloat("input_meleelevel", 0);
            }

            if (Input.GetButtonDown("Crouch"))
            {
                if (Animator.GetBool("input_crouch"))
                {
                    Animator.SetBool("input_crouch", false);
                }
                else
                {
                    Animator.SetBool("input_crouch", true);
                }
            }


            if (Input.GetKeyDown("1"))
            {
                weaponSlot = 1;
                GetComponent<PlayerAvater>().CheckCanChangeWeapon(weaponSlot);
            }
            if (Input.GetKeyDown("2"))
            {
                weaponSlot = 2;
                GetComponent<PlayerAvater>().CheckCanChangeWeapon(weaponSlot);
            }
            if (Input.GetKeyDown("3"))
            {
                weaponSlot = 3;
                GetComponent<PlayerAvater>().CheckCanChangeWeapon(weaponSlot);
            }
            if (Input.GetKeyDown("4"))
            {
                weaponSlot = 4;
                GetComponent<PlayerAvater>().CheckCanChangeWeapon(weaponSlot);
            }
            if (Input.GetKeyDown("5"))
            {
                weaponSlot = 5;
                GetComponent<PlayerAvater>().CheckCanChangeWeapon(weaponSlot);
            }
        }
        void ResetToggle()
        {
            Animator.SetBool("input_crouch", false);

        }
    }
}
