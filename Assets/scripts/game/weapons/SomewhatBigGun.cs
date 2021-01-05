using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.scripts.game.players;

namespace Assets.scripts.game.weapons
{
    [Serializable]
    public class SomewhatBigGun : GenericWeapon
    {
        [SerializeField] private float regainRate = 0.8f;

        [SerializeField] private float damage = 15;

        [SerializeField] private float inaccuracy = 0f;

        [SerializeField] private float dropOffStart = 10f;

        [SerializeField] private float dropOffEnd = 40;
        [SerializeField] private float minimumDamage = 2f;

        [SerializeField] private float totalRange = 80;

        [SerializeField] private float shotCooldownTime = 2f;

        [SerializeField] private AudioClip shootNoise;

        [SerializeField] private Projectile projectile;

        public override void Start()
        {
            base.Start();
            regen = regainRate;
            dmg = damage;
            inacc = inaccuracy;
            dropStart = dropOffStart;
            dropEnd = dropOffEnd;
            minDmg = minimumDamage;
            dist = totalRange;

            cooldown = shotCooldownTime;

            fireNoise = shootNoise;

            SetupDropOffCurve();
        }

        protected override float ActuallyDoTheShooting(IAmTheContestantWhoHasShot shooter, bool isFirstShot)
        {
            float shooterHealthChange = 0f;
            if (!isFirstShot)
            {
                shooterHealthChange -= damage;
            }

            Projectile rocket = Instantiate(projectile, shooter.GetViewPos(), shooter.GetLookRotation());

            rocket.ProperlySetItUp(shooter);

            return shooterHealthChange;

        }

        public override WeaponInfo GetWeaponInfo()
        {
            return projectile.info;
        }
    }

}