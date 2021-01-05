using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Assets.scripts.game.players;
using Assets.scripts.game.control;

namespace Assets.scripts.game.weapons
{
    public class Projectile : MonoBehaviour
    {


        [SerializeField]
        private const float directHitDamage = 30f;

        [SerializeField]
        private float maxSplash = 15f;

        [SerializeField]
        private float splashRadius = 10f;

        [SerializeField]
        private float regen = 0.8f;

        private IAmTheContestantWhoHasShot shooter;

        [SerializeField]
        private const float rocketSpeed = 5f;

        [SerializeField]
        private ParticleSystem afterburner;

        [SerializeField]
        private ParticleSystem trail;

        
        private ConstantForce theForce;

        [SerializeField]
        private GameObject explosion;

        private Rigidbody rb;


        public WeaponInfo info = new WeaponInfo(directHitDamage, rocketSpeed);

        public void ProperlySetItUp(IAmTheContestantWhoHasShot c)
        {
            shooter = c;
            gameObject.layer = c.GetLayer();
            //rb.position = c.GetViewPos();
            //rb.rotation = Quaternion.Euler(c.GetLookVector());
            //rb.freezeRotation = true;

            //rb.position += this.transform.forward;

            //this.transform.forward = c.GetLookVector().normalized;
            //this.transform.position = c.GetViewPos();
            //this.transform.Rotate(0, 0, 0, Space.Self);

            //theForce = gameObject.AddComponent<ConstantForce>();

            //theForce.relativeForce = new Vector3(0, 0, -rocketSpeed);
            
            

            //theForce.enabled = true;
            //velocity = d * speed;
            
        }

        // Use this for initialization
        void Start()
        {
            
            
            //force.relativeForce = velocity;
            //theForce.enabled = true;
            rb = GetComponent<Rigidbody>();

            rb.useGravity = false;

            rb.position = rb.position + this.transform.forward;

            //theForce = gameObject.AddComponent<ConstantForce>();

            //theForce.relativeForce = new Vector3(0, 0, rocketSpeed);
            // theForce.enabled = true;

            rb.AddRelativeForce(new Vector3(0, 0, rocketSpeed), ForceMode.Impulse);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            //rb.velocity = (this.transform.forward * rocketSpeed);

            
        }

        

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("hit something!");

            IAmTheContestantWhoGetsShot directHit = collision.gameObject.GetComponentInParent<Contestant>();

            float shooterHealthChange = 0f;

            Vector3 thisPos = this.transform.position;

            int mask = LayerMask.GetMask("PlayerLayer", "EnemyLayer");
            Collider[] splashColliders = Physics.OverlapSphere(thisPos, splashRadius, mask);

            //ISet<ISharedWeaponContestant> all = (ISet<ISharedWeaponContestant>)gc.GetAllContestants();

            ISet<IAmTheContestantWhoGetsShot> allHit = new HashSet<IAmTheContestantWhoGetsShot>();

            foreach (Collider c in splashColliders)
            {
                allHit.Add(c.gameObject.GetComponentInParent<Contestant>());
            }

            

            if (directHit != null)
            {
                if (directHit.Equals(shooter))
                {
                    return;
                }
                else
                {
                    directHit.WeaponChangeHealth(-directHitDamage);
                    shooterHealthChange += (directHitDamage * regen);
                    //all.Remove(directHit);
                    allHit.Remove(directHit);
                }
            }

            
            int otherMask = ~mask;
            float dist;
            Vector3 closest;
            if (allHit.Contains(shooter))
            {
                closest = shooter.GetClosestPointTo(thisPos);
                if(!Physics.Linecast(thisPos, closest, otherMask))
                {
                    //if false, it has line of sight.
                    dist = Vector3.Distance(thisPos, closest);
                    shooterHealthChange -= (maxSplash * (dist/splashRadius));
                }
                allHit.Remove(shooter);
            }
            
            foreach(IAmTheContestantWhoGetsShot c in allHit)
            {
                closest = c.GetClosestPointTo(thisPos);
                if(!Physics.Linecast(thisPos, closest, otherMask))
                {
                    //if false, it has line of sight.
                    dist = Vector3.Distance(thisPos, closest);
                    float dmg = (dist / splashRadius) * maxSplash;
                    shooterHealthChange += (dmg + (dmg * regen));
                    c.WeaponChangeHealth(-dmg);
                }
                //allHit.Remove(c);
            }

            shooter.WeaponChangeHealth(shooterHealthChange, true);
            

            GameObject boom = Instantiate(explosion);
            boom.transform.position = thisPos;
            Destroy(this.gameObject);
        }
    }
}