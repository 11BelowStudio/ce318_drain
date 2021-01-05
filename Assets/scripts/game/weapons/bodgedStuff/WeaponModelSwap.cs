using System.Collections;
using UnityEngine;
using Assets.scripts.game.players;

namespace Assets.scripts.game.weapons.bodge
{
    public class WeaponModelSwap : MonoBehaviour
    {

        [SerializeField]
        public GameObject pistol;

        [SerializeField]
        private GameObject shotgun;

        [SerializeField]
        private GameObject sbg;

        private deprecated_WeaponEnum current;

        // Use this for initialization
        void Start()
        {
            pistol.SetActive(false);
            shotgun.SetActive(false);
            sbg.SetActive(false);
            current = deprecated_WeaponEnum.Pistol;
            //thisOneIsActiveBTW.GetComponent<Animator>().StartPlayback();
        }

        public void TimeToStart()
        {
            //pistol.SetActive(true);
            shotgun.SetActive(false);
            sbg.SetActive(false);
        }

        public void RightThatsIt()
        {
            /*
            switch (current)
            {
                case WeaponEnum.Pistol:
                    pistol.SetActive(false);
                    //pistol.GetComponent<Animator>().SetTrigger("Unequip");
                    break;
                case WeaponEnum.Shotgun:
                    shotgun.SetActive(false);
                    //shotgun.GetComponent<Animator>().SetTrigger("Unequip");
                    break;
                case WeaponEnum.SBG:
                    sbg.SetActive(false);
                    //sbg.GetComponent<Animator>().SetTrigger("Unequip");
                    break;
            }
            //thisOneIsActiveBTW.GetComponent<Animator>().SetTrigger("Unequip");
            */
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SwapWeapon(deprecated_WeaponEnum newWeapon)
        {
            /*
            //thisOneIsActiveBTW.GetComponent<Animator>().SetTrigger("Unequip");
            //thisOneIsActiveBTW.SetActive(false);
            //thisOneIsActiveBTW.GetComponent<Animator>().enabled = false;
            //thisOneIsActiveBTW.SetActive(false);
            switch (current)
            {
                case WeaponEnum.Pistol:
                    pistol.SetActive(false);
                    //pistol.GetComponent<Animator>().SetTrigger("Unequip");
                    break;
                case WeaponEnum.Shotgun:
                    shotgun.SetActive(false);
                    //shotgun.GetComponent<Animator>().SetTrigger("Unequip");
                    break;
                case WeaponEnum.SBG:
                    sbg.SetActive(false);
                    //sbg.GetComponent<Animator>().SetTrigger("Unequip");
                    break;
            }

            switch (newWeapon)
            {
                case WeaponEnum.Pistol:
                    pistol.SetActive(true);
                    //pistol.GetComponent<Animator>().ResetTrigger("Shoot");
                    //pistol.GetComponent<Animator>().ResetTrigger("Unequip");
                    break;
                case WeaponEnum.Shotgun:
                    shotgun.SetActive(true);
                    //shotgun.GetComponent<Animator>().ResetTrigger("Shoot");
                    //shotgun.GetComponent<Animator>().ResetTrigger("Unequip");
                    break;
                case WeaponEnum.SBG:
                    sbg.SetActive(true);
                    //sbg.GetComponent<Animator>().ResetTrigger("Shoot");
                    //sbg.GetComponent<Animator>().ResetTrigger("Unequip");
                    break;
            }
            current = newWeapon;
            //thisOneIsActiveBTW.GetComponent<Animator>().StartPlayback();
            */
        }

        public void Shoot()
        {
            /*
            switch (current) {
                case WeaponEnum.Pistol:
                    //pistol.GetComponent<Animator>().SetTrigger("Shoot");
                    pistol.GetComponentInChildren<ParticleStarter>().ShootIt();
                    break;
                case WeaponEnum.Shotgun:
                    //shotgun.GetComponent<Animator>().SetTrigger("Shoot");
                    shotgun.GetComponentInChildren<ParticleStarter>().ShootIt();
                    break;
                case WeaponEnum.SBG:
                    //sbg.GetComponent<Animator>().SetTrigger("Shoot");
                    //sbg.GetComponent<Animator>().SetTrigger("Shoot");
                    sbg.GetComponentInChildren<ParticleStarter>().ShootIt();
                    break;
            }
            */

            //thisOneIsActiveBTW.GetComponent<Animator>().SetTrigger("Shoot");
        }
    }
}