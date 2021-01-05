using System.Collections;
using UnityEngine;

namespace Assets.scripts.game.players
{
    public class HealthScript: MonoBehaviour, IWeaponHealth, IControlHealth
    {
        [SerializeField] private Contestant contestant;

        //[SerializeField] private GameControl gc;
        [SerializeField] internal float startHealth = 60;
        [SerializeField] private float maxHealth = 90;
        [SerializeField] private bool isPlayer = false;

        [SerializeField]
        private HealthDrainInfo healthDrain = new HealthDrainInfo(
            1,
            1,
            1,
            60,
            0.1f,
            10);


        [SerializeField] internal const float startSpeed = 7.5f;
        //[SerializeField] private float maxSpeed;





        private AnimationCurve speedCurve;
        

        private Coroutine drainCoroutine;
        private Coroutine extraDrainCoroutine;

        public bool ded = false;

        public float currentHealth;
        private float drainRate;
        private float currentSpeed = startSpeed;

        public float CurrentHealth
        {
            get
            {
                return currentHealth;
            }
        }


        public float CurrentSpeed
        {
            get
            {
                if (ded) { return 0; }
                else
                {
                    return currentSpeed;
                }
            }
        }

        // Start is called before the first frame update
        public void Start()
        {
            ded = false;
            currentHealth = startHealth;
            currentSpeed = startSpeed;
            drainRate = healthDrain.healthDrainIncrement;

            speedCurve = new AnimationCurve(
                new Keyframe[] {
                    new Keyframe(0f,1.5f),
                    new Keyframe(0.1f,1.5f),
                    new Keyframe(1f,1f),
                    new Keyframe(1.5f,0f)
                }
            );

        }

        /// <summary>
        /// Starts the health draining
        /// </summary>
        public void WillYouStartTheDrainsPlease()
        {
            drainCoroutine = StartCoroutine("HealthDrain");
            extraDrainCoroutine = StartCoroutine("MoreHealthDrain");
        }

        /// <summary>
        /// stops the health draining
        /// </summary>
        public void WillYouStopTheDrainsPlease()
        {
            StopCoroutine("HealthDrain");
            StopCoroutine("MoreHealthDrain");
        }


        public IEnumerator HealthDrain()
        {
            //this drains health
            do
            {
                //waits for the health drain interval
                yield return new WaitForSeconds(healthDrain.healthDrainInterval);
            } while (DrainHealth()); //and drains health
            yield break;
            //continues until player is ded
        }

        public IEnumerator MoreHealthDrain()
        {
            //waits for the initital increased drain delay to pass
            yield return new WaitForSeconds(healthDrain.delayForIncreasedDrain);
            while (!ded)
            {
                //increases the drainRate by the appropriate increment
                drainRate += healthDrain.increasedDrainIncrement;
                //waits for the next interval to pass
                yield return new WaitForSeconds(healthDrain.increasedDrainInterval);
            }
            yield break;
        }

        //this actually drains the health
        bool DrainHealth()
        {
            if (currentHealth > startHealth)
            { //drains the additional overheal health drain if contestant has more than the starting health
                ChangeHealth(-healthDrain.overhealDrainIncrement);
            }
            //drains the standard amount of health
            ChangeHealth(-drainRate);

            //returns true if ded
            return !ded;
        }




        public void ChangeHealth(float healthChange, bool leeway = false)
        {
            currentHealth += healthChange; //changes contestant health by the given amount
            if (currentHealth > maxHealth)
            {
                //caps the contestant's health at maxHealth
                currentHealth = maxHealth;
            }
            if (currentHealth <= 0) //if they have no health left
            {
                if (leeway)
                {
                    currentHealth = 1; //you get 1hp if you have leeway. otherwise you are dead
                }
                else
                {
                    currentHealth = 0; //looks neater as a flat 0
                }

            }

        }

        public bool IsDed()
        {
            return ded;
        }


        private void FixedUpdate()
        {

        }

        private void LateUpdate()
        {
            if (!ded) //if you aint ded yet
            {
                contestant.SetDisplayHealth(currentHealth);

                if (currentHealth <= 0) //if you should be dead
                {
                    ded = true; //you are now dead
                    //stop the draining
                    //WillYouStopTheDrainsPlease();
                    //contestant.YouAreDedNotBigSoupRice();
                }
                else
                {
                    //sets the currentSpeed to be the start speed multiplied by the appropriate part of the speedCurve 
                    currentSpeed = startSpeed * speedCurve.Evaluate(currentHealth / startHealth);
                }
            }

        }
    }
}