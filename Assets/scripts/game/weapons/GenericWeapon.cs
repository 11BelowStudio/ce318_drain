using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.scripts.game.players;
using Random = UnityEngine.Random;

namespace Assets.scripts.game.weapons
{
    [Serializable]
    public abstract class GenericWeapon : MonoBehaviour, IAmAWeaponThatCanBeShot
    {
        protected float regen;

        protected float dmg;

        protected float inacc;

        protected float dropStart;

        protected float dropEnd;
        protected float minDmg;

        protected float dist;

        protected float cooldown;

        public float Cooldown { get { return cooldown; } }

        protected AnimationCurve dropOffCurve;

        protected Vector3 start;

        protected Vector3 rayLine;


        protected AudioClip fireNoise;

        WeaponInfo weaponInfo;


        public virtual void Start()
        {

            weaponInfo = new WeaponInfo(dmg, dist, dropStart, dropEnd);
            
        }

        public void CooldownStuff(CooldownOverDelegate callThisWhenDone)
        {
            StartCoroutine(CooldownCoroutine(callThisWhenDone));
        }

        private IEnumerator CooldownCoroutine(CooldownOverDelegate callThisWhenDone)
        {
            yield return new WaitForSeconds(cooldown);
            callThisWhenDone();
            yield break;
        }

        public delegate void CooldownOverDelegate();
        
        protected void SetupDropOffCurve()
        {
            dropOffCurve = new AnimationCurve(
            new Keyframe[]
                {
                    new Keyframe(0,dmg),
                    new Keyframe(dropStart,dmg),
                    new Keyframe(dropEnd, minDmg)
                }
            );
        }


        public float Deprecated_Shoot(deprecated_ModifiedFirstPersonController shooter)
        {
            Vector3 lookVector = shooter.lookVector;
            Vector3 viewOriginVector = shooter.playerView.transform.position;
            float shooterHealthChange = -dmg;
            int layers;
            if (shooter.IsPlayer)
            {
                layers = LayerMask.GetMask("Arena", "EnemyLayer");
            }
            else
            {
                layers = LayerMask.GetMask("Arena", "PlayerLayer");
            }
            
            shooterHealthChange = Deprecated_ActuallyDoTheShooting(lookVector, viewOriginVector, layers, shooter.DoTheShotCooldownShotNoiseAndSeeIfThisIsTheFirstShot(cooldown, fireNoise)); //do the shooting


            //this will make shooter wait before they can shoot again, and play the shot noise.
            //additionally, it will check if this is the first shot they made with this weapon (first shot is free after all)
            /*
            if (shooter.DoTheShotCooldownShotNoiseAndSeeIfThisIsTheFirstShot(cooldown, fireNoise))
            {
               // if (shooterHealthChange <= 0) { shooterHealthChange = 0;  } //if it's the first shot (which is free), any health loss from the shooting will be nullified.
            } */
            Debug.Log(shooterHealthChange);

            //TODO: noises
            //shooter.PlayNoise(fireNoise);

            return shooterHealthChange; //return the health change from the shooter

        }

        public void Shoot(IAmTheContestantWhoHasShot shooter, bool isFirstShot = false)
        {

            


            float healthChange = ActuallyDoTheShooting(shooter, isFirstShot);
            //float healthChange = ActuallyDoTheShooting(lookVector, viewOriginVector, layers, isFirstShot);
            Debug.Log(healthChange);
            shooter.WeaponChangeHealth(healthChange, true);
            //shooter.WeaponGetHealth().ChangeHealth(ActuallyDoTheShooting(lookVector, viewOriginVector, layers, isFirstShot), true);

            shooter.ActuallyShoot(fireNoise);

        }

        protected virtual float ActuallyDoTheShooting(IAmTheContestantWhoHasShot shooter, bool isFirstShot)
        {
            Debug.Log("pew pew");
            int layers = LayerMask.GetMask("Arena", "EnemyLayer", "PlayerLayer", "EnemyLayer1", "EnemyLayer2", "EnemyLayer3", "EnemyLayer4");
            layers -= shooter.GetLayer();

            
            float shooterHealthChange = 0;
            if (!isFirstShot)
            {
                shooterHealthChange -= dmg;
            }

            Vector3 lookVector = shooter.GetLookVector();
            Vector3 viewOriginVector = shooter.GetViewPos();


            Quaternion q = Random.rotation;

            Vector3 randomVector3 = Random.onUnitSphere;

            Quaternion diff = Quaternion.Slerp(Quaternion.Euler(lookVector), Random.rotationUniform, inacc);

            //Vector3 shootVector = diff.eulerAngles;


            //Vector3 shootVector = lookVector + (Random.insideUnitSphere * inacc);
            
            Vector3 shootVector = new Vector3(
                    (lookVector.x += ((Random.value * 2) - 1) * inacc),
                    (lookVector.y += ((Random.value * 2) - 1) * inacc),
                    (lookVector.z += ((Random.value * 2) - 1) * inacc)
                );
            
            

            Debug.Log($"Look: {lookVector}, shoot: {shootVector}");
            //Vector3 shootVector = (Quaternion.Euler((Random.value * inacc), (Random.value * inacc), (Random.value * inacc)) *= Quaternion.Euler(lookVector)).Euler;
            //Vector3 shootVector = GetRandomSpreadDirection() + lookVector;
            //Vector3 shootVector = lookVector;
            RaycastHit hitInfo = new RaycastHit();
            start = viewOriginVector;



            if (shooter.AreYouAHuman() || weaponInfo.attackType.Equals(WeaponAttackTypes.Projectile))
            {

                if (Physics.Raycast(
                        new Ray(viewOriginVector, shootVector),
                        out hitInfo,
                        dist,
                        layers
                        )
                    )
                {

                    rayLine = shootVector.normalized * hitInfo.distance;



                    GameObject hitThing = hitInfo.collider.gameObject;

                    Debug.Log(hitThing.name);
                    Debug.Log(LayerMask.LayerToName(hitThing.layer));

                    Debug.Log($"{(hitThing.Equals(shooter) ? "shot urself" : "seems cool")  }");
                    switch (LayerMask.LayerToName(hitThing.layer))
                    {

                        case "Arena":
                            {
                                break;
                            }
                        default:
                            {
                                Debug.Log("Hit something");
                                float damageDealt = dropOffCurve.Evaluate(hitInfo.distance);
                                IAmTheContestantWhoGetsShot other = hitThing.GetComponentInParent<Contestant>();
                                if (other == null)
                                {
                                    other = hitThing.GetComponentInChildren<Contestant>();
                                }
                                Debug.Log($"{(other.Equals(shooter) ? "shot urself" : "seems cool")  }");
                                other.WeaponChangeHealth(-damageDealt);
                                shooterHealthChange += (damageDealt * regen);
                                break;
                            }
                    }
                }
                else
                {
                    rayLine = (shootVector.normalized * dist);
                    Debug.Log("Nothing hit");

                }

            }
            else
            {
                Transform target = shooter.GetMyTarget();
                GameObject hitThing = target.GetComponentInChildren<Contestant>().gameObject;
                IAmTheContestantWhoGetsShot cont = hitThing.GetComponent<Contestant>();

                float range = Vector3.Distance(viewOriginVector, target.position);

                if (range < dist)
                {
                    rayLine = target.position - viewOriginVector;

                    Debug.Log(hitThing.name);
                    Debug.Log("Hit something");
                    float damageDealt = dropOffCurve.Evaluate(range/dist);
                    Debug.Log($"dealt {damageDealt} damage");
                    cont.WeaponChangeHealth(-damageDealt);
                    shooterHealthChange += (damageDealt * regen);
                }
                /*
                layers -= LayerMask.NameToLayer("Arena");
                if(Physics.SphereCast(
                        new Ray(viewOriginVector + shootVector, shootVector),
                        0.25f,
                        out hitInfo,
                        dist,
                        layers
                    ))
                {
                    rayLine = shootVector.normalized * hitInfo.distance;




                    Debug.Log(hitThing.name);
                    Debug.Log(LayerMask.LayerToName(hitThing.layer));

                    Debug.Log($"{(hitThing.Equals(shooter) ? "shot urself" : "seems cool")  }");
                    Debug.Log("Hit something");
                    float damageDealt = dropOffCurve.Evaluate(hitInfo.distance);
                    IAmTheContestantWhoGetsShot other = hitThing.GetComponentInParent<Contestant>();
                    if (other == null)
                    {
                        other = hitThing.GetComponentInChildren<Contestant>();
                    }
                    Debug.Log($"{(other.Equals(shooter) ? "shot urself" : "seems cool")  }");
                    other.WeaponChangeHealth(damageDealt);
                    shooterHealthChange += (damageDealt * regen);
                }
                */
            }


            return shooterHealthChange;

            
        }

        protected virtual float Deprecated_ActuallyDoTheShooting(Vector3 lookVector, Vector3 viewOriginVector, int layers, bool isFirstShot)
        {
            float shooterHealthChange = 0;
            if (!isFirstShot)
            {
                shooterHealthChange -= dmg;
            }

            //
            //Quaternion lookQuat = Quaternion.Euler(lookVector);
            //Quaternion spread = Quaternion.Euler((((Random.value * 2) - 1) * inacc), (((Random.value * 2) - 1) * inacc), (((Random.value * 2) - 1) * inacc)).normalized;

            //lookQuat.x += spread.x;
            //lookQuat.y += spread.y;
            //lookQuat.z += spread.z;

            /*
            Quaternion lookQuat = Quaternion.Euler(
                new Vector3(
                    (lookVector.x += ((Random.value * 2) - 1) * inacc),
                    (lookVector.y += ((Random.value * 2) - 1) * inacc),
                    (lookVector.z += ((Random.value * 2) - 1) * inacc)
                )
            );
            */
            //lookQuat = Quaternion.RotateTowards(lookQuat, spread, 3);

            

            
            Vector3 shootVector = new Vector3(
                    (lookVector.x += ((Random.value * 2) - 1) * inacc),
                    (lookVector.y += ((Random.value * 2) - 1) * inacc),
                    (lookVector.z += ((Random.value * 2) - 1) * inacc)
                );
            

            //Vector3 shootVector = (Quaternion.Euler((Random.value * inacc), (Random.value * inacc), (Random.value * inacc)) *= Quaternion.Euler(lookVector)).Euler;
            //Vector3 shootVector = GetRandomSpreadDirection() + lookVector;
            //Vector3 shootVector = lookVector;
            RaycastHit hitInfo = new RaycastHit();
            start = viewOriginVector;
            rayLine = viewOriginVector + (shootVector.normalized * dist);

            if (Physics.Raycast(
                    viewOriginVector,
                    shootVector,
                    out hitInfo,
                    dist,
                    layers
                    )
                )
            {
                GameObject hitThing = hitInfo.collider.gameObject;
                switch (LayerMask.LayerToName(hitThing.layer))
                {

                    case "EnemyLayer":
                    case "PlayerLayer":
                        {

                            float damageDealt = dropOffCurve.Evaluate(hitInfo.distance);
                            hitThing.GetComponent<HealthScript_deprecated>().ChangeHealth(-damageDealt);
                            shooterHealthChange = (regen * damageDealt);
                            hitThing.GetComponent<deprecated_ModifiedFirstPersonController>().PlayHurt();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            


            return shooterHealthChange;
        }

        float GetSpreadComponent() { return ((Random.value * 2) - 1) * inacc; }

        Vector3 GetRandomSpreadDirection()
        {
            //Makes a spherical coordinate for the random spread

            float radialDist = 1f; 
            float polarAngle = ((Random.value * 2) - 1) * (360 * inacc) * Mathf.Deg2Rad; //between +180 and -180 degrees (converted to radians)
            float azimuthalAngle = ((Random.value * 2) - 1) * (inacc * 360) * Mathf.Deg2Rad; //between +180 and -180 degrees (converted to radians)

            //converts the spherical coordinate stuff into cartesian

            return new Vector3(
                radialDist * Mathf.Sin(polarAngle) * Mathf.Cos(azimuthalAngle),
                radialDist * Mathf.Cos(polarAngle) * Mathf.Sin(azimuthalAngle),
                radialDist * Mathf.Cos(polarAngle)
            );

        }


        protected void OnDrawGizmos()
        {
            Gizmos.DrawRay(start, rayLine);
        }

        public virtual WeaponInfo GetWeaponInfo()
        {
            return weaponInfo;
        }
        

    }

    public struct WeaponInfo
    {
        public readonly float damage;
        public readonly float distance;
        public readonly float dropoffStart;
        public readonly float dropoffEnd;
        public readonly WeaponAttackTypes attackType;
        public readonly float projectileSpeed;

        public WeaponInfo(float dmg, float dist, float dropoffS, float dropoffE)// WeaponAttackTypes type = WeaponAttackTypes.Hitscan, float projSpeed = float.MaxValue)
        {
            damage = dmg;
            distance = dist;
            //attackType = type;
            //projectileSpeed = projSpeed;
            attackType = WeaponAttackTypes.Hitscan;
            dropoffStart = dropoffS;
            dropoffEnd = dropoffE;
            projectileSpeed = float.PositiveInfinity;
        }

        public WeaponInfo(float dmg, float projSpeed)
        {
            damage = dmg;
            attackType = WeaponAttackTypes.Projectile;
            projectileSpeed = projSpeed;
            dropoffStart = projSpeed * 2;
            dropoffEnd = projSpeed * 4;
            distance = projSpeed * 6;
        }
    }

    public enum WeaponAttackTypes
    {
        Hitscan,
        Projectile
    }

    public interface IAmAWeapon
    {

    }

    public interface IAmAWeaponThatHasInfo : IAmAWeapon
    {
        WeaponInfo GetWeaponInfo();
    }

    public interface IAmAWeaponThatCanBeShot: IAmAWeaponThatHasInfo
    {
        void Shoot(IAmTheContestantWhoHasShot shooter, bool isFirstShot = false);

        void CooldownStuff(GenericWeapon.CooldownOverDelegate callThisWhenDone);
    }

    

}
