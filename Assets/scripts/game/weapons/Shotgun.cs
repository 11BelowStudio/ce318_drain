using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.scripts.game.players;

namespace Assets.scripts.game.weapons
{
    [Serializable]
    public class Shotgun : GenericWeapon
    {
        [SerializeField] private int pellets = 10;

        [SerializeField] private float regainRate = 0.9f;

        [SerializeField] private float damage = 1f;

        [SerializeField] private float inaccuracy = 0.05f;

        [SerializeField] private float dropOffStart = 8f;

        [SerializeField] private float dropOffEnd = 30;
        [SerializeField] private float minimumDamage = 0.5f;

        [SerializeField] private float totalRange = 40;

        [SerializeField] private float shotCooldownTime = 1.5f;

        [SerializeField] private AudioClip shootNoise; 

        
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
            float totalHealthChange = 0;
            for (int i = pellets; i > 0; i--)
            {
                totalHealthChange += base.ActuallyDoTheShooting(shooter, isFirstShot);
            }
            return totalHealthChange;
        }

        protected override float Deprecated_ActuallyDoTheShooting(Vector3 lookVector, Vector3 viewOriginVector, int layers, bool isFirstShot)
        {

            float totalHealthChange = 0;
            for (int i = 0; i < pellets; i++) //well yeah its a shotgun they have multiple pellets dont they?
            {
                totalHealthChange += base.Deprecated_ActuallyDoTheShooting(lookVector, viewOriginVector, layers, isFirstShot);
            }
            return totalHealthChange;
        }
    }

}
