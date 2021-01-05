using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace Assets.scripts.game.weapons
{
    [Serializable]
    public class Pistol: GenericWeapon
    {

        [SerializeField] private float regainRate = 0.8f;

        [SerializeField] private float damage = 5;

        [SerializeField] private float inaccuracy = 0.01f;

        [SerializeField] private float dropOffStart = 7.5f;

        [SerializeField] private float dropOffEnd = 20;
        [SerializeField] private float minimumDamage = 3f;

        [SerializeField] private float totalRange = 30;

        [SerializeField] protected float shotCooldownTime = 1f;

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

    }
}
