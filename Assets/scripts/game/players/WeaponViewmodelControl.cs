using Assets.scripts.game.weapons;
using System.Collections;
using UnityEngine;

namespace Assets.scripts.game.players
{
    public class WeaponViewmodelControl : WeaponModelControl
    {

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            modelAnimator = pistol.GetComponent<Animator>();
        }

        public void TimeToStart()
        {
            pistol.SetActive(true);
            modelAnimator = pistol.GetComponent<Animator>();
            modelAnimator.SetTrigger(equipHash);
            shotgun.SetActive(false);
            sbg.SetActive(false);
            
        }

        public override void EquipWeapon(TheWeaponEnum nextWeapon)
        {
            modelAnimator.SetTrigger(unequipHash);
            switch (equipped)
            {
                case TheWeaponEnum.Pistol:
                    pistol.SetActive(false);
                    break;
                case TheWeaponEnum.SBG:
                    sbg.SetActive(false);
                    break;
                case TheWeaponEnum.Shotgun:
                    shotgun.SetActive(false);
                    break;
            }
            switch (nextWeapon)
            {
                case TheWeaponEnum.Pistol:
                    pistol.SetActive(true);
                    modelAnimator = pistol.GetComponent<Animator>();
                    break;
                case TheWeaponEnum.SBG:
                    sbg.SetActive(true);
                    modelAnimator = sbg.GetComponent<Animator>();
                    break;
                case TheWeaponEnum.Shotgun:
                    shotgun.SetActive(true);
                    modelAnimator = shotgun.GetComponent<Animator>();
                    break;
                default:
                    modelAnimator.SetTrigger(unequipHash);
                    break;
            }
            modelAnimator.SetTrigger(equipHash);
            equipped = nextWeapon;
        }

        public void RightThatsIt()
        {
            modelAnimator.SetTrigger(unequipHash);
            pistol.SetActive(false);
            shotgun.SetActive(false);
            sbg.SetActive(false);
            
        }


        // Update is called once per frame

    }
}