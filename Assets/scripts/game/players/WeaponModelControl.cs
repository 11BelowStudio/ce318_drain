using Assets.scripts.game.weapons;
using System.Collections;
using UnityEngine;

namespace Assets.scripts.game.players
{
    public class WeaponModelControl : MonoBehaviour
    {

        [SerializeField]
        protected GameObject pistol;

        [SerializeField]
        protected GameObject shotgun;

        [SerializeField]
        protected GameObject sbg;

        protected TheWeaponEnum equipped;

        protected Animator modelAnimator;

        protected int shootHash = Animator.StringToHash("Shoot");

        protected int pistHash = Animator.StringToHash("Pistol");

        protected int sbgHash = Animator.StringToHash("SBG");

        protected int shotgunHash = Animator.StringToHash("Shotgun");

        protected int unequipHash = Animator.StringToHash("Unequip");

        protected int equipHash = Animator.StringToHash("Equip");

        // Use this for initialization
        protected virtual void Start()
        {
            modelAnimator = GetComponent<Animator>();
            equipped = TheWeaponEnum.Pistol;
            pistol.SetActive(true);
            shotgun.SetActive(false);
            sbg.SetActive(false);


        }

        public void Shoot()
        {
            modelAnimator.SetTrigger(shootHash);
            switch (equipped)
            {
                case TheWeaponEnum.Pistol:
                    pistol.GetComponent<ParticleStarter>().ShootIt();
                    break;
                case TheWeaponEnum.SBG:
                    sbg.GetComponent<ParticleStarter>().ShootIt();
                    break;
                case TheWeaponEnum.Shotgun:
                    shotgun.GetComponent<ParticleStarter>().ShootIt();
                    break;
            }
        }

        public virtual void EquipWeapon(TheWeaponEnum nextWeapon)
        {
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
            //modelAnimator.SetTrigger(unequipHash);
            switch (nextWeapon)
            {
                case TheWeaponEnum.Pistol:
                    pistol.SetActive(true);
                    modelAnimator.SetTrigger(pistHash);
                    break;
                case TheWeaponEnum.SBG:
                    sbg.SetActive(true);
                    modelAnimator.SetTrigger(sbgHash);
                    break;
                case TheWeaponEnum.Shotgun:
                    shotgun.SetActive(true);
                    modelAnimator.SetTrigger(shotgunHash);
                    break;
                default:
                    modelAnimator.SetTrigger(unequipHash);
                    break;
            }
            equipped = nextWeapon;
        }

        // Update is called once per frame
        protected void Update()
        {

        }
    }
}