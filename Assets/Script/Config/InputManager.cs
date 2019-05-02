using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Config
{
    public class InputManager:MonoBehaviour
    {
        public float ws;
        public float ad;
        public float maxWSAD;
        public float charge;//蓄力

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

            
            if (ws*ws+ad*ad != 0)
            {
                Animator.SetBool("input_move",true);
            }
            else
            {
                Animator.SetBool("input_move", false);
            }

            if (Input.GetButtonDown("Jump"))
            {
                Animator.SetBool("avatermain_jump",true);
            }
            else
            {
                Animator.SetBool("avatermain_jump", false);
            }

            if (Input.GetButton("Fire1"))
            {
                Animator.SetBool("input_fire", true);
            }
            else
            {
                Animator.SetBool("input_fire", false);
            }

            #region 蓄力
            if (Input.GetButtonUp("Fire1"))
            {
                Animator.SetFloat("input_charge", charge);
                charge = 0;
            }
            if (Input.GetButton("Fire1"))
            {
                charge += Time.deltaTime;
            }
            #endregion

            if (Input.GetButtonDown("dodge"))
            {
                Animator.SetBool("input_dodge",true);
            }
            else
            {
                Animator.SetBool("input_dodge", false);
            }

            
            /*
            if(Input.GetButton("Fire2"))
            {
                Animator.SetTrigger("input_melee");
                //Animator.SetTrigger("avatermain_panicmelee");
            }
            */

            if (Input.GetButtonDown("Fire2"))
            {
                Animator.SetTrigger("input_melee");
                //Animator.SetTrigger("avatermain_panicmelee");
                var anifloat = Animator.GetFloat("input_meleelevel");
                if (anifloat < 10)
                {
                    Animator.SetFloat("input_meleelevel", anifloat + 1);
                }
                else Animator.SetFloat("input_meleelevel", 1);
            }
        }
    }
}
