using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Assets.scripts.game.players;
using Assets.scripts.game.players.human;
using Assets.scripts.game.players.bot;


namespace Assets.scripts.game.control
{


    //okay so this was a poorly-planned attempt at refactoring the health script into something that would apply a single coroutine to the health of all constestants at once
    //I've been up for like 24 hours at this point so yeah 


    [Serializable]
    struct HealthDrainInfo
    {
        public float healthDrainIncrement;
        public float healthDrainInterval;

        public float overhealDrainIncrement;

        public float delayForIncreasedDrain;

        public float increasedDrainIncrement;
        public float increasedDrainInterval;

        public HealthDrainInfo(float drainIncrement, float drainInterval, float overhealDrain, float bigDrainDelay, float biggerDrainIncrement, float biggerDrainInterval)
        {
            healthDrainIncrement = drainIncrement;
            healthDrainInterval = drainInterval;
            overhealDrainIncrement = overhealDrain;
            delayForIncreasedDrain = bigDrainDelay;
            increasedDrainIncrement = biggerDrainIncrement;
            increasedDrainInterval = biggerDrainInterval;

        }
    }

    [Serializable]
    class ContestantInfo
    {
        deprecated_ModifiedFirstPersonController contestant;
        float health = unused_HealthControl.startHealth;
        public bool isDead = false;

        private float healthChangeThisFrame = 0;

        public ContestantInfo(deprecated_ModifiedFirstPersonController c)
        {
            contestant = c;

        }

        public void DrainHealth(float healthDrain)
        {
            healthChangeThisFrame += healthDrain;

        }

        public void updateHealth()
        {
            health += healthChangeThisFrame;
            healthChangeThisFrame = 0;
            if (health <= 0)
            {
                isDead = true;
            }
            
        }
    }

    [Serializable]
    class PlayerInfo: ContestantInfo
    {
        public PlayerInfo(DEPRECATED_HumanController c): base(c)
        {
            
        }
    }

    public class unused_HealthControl : MonoBehaviour
    {

        [SerializeField] internal const float startHealth = 60;
        [SerializeField] internal const float maxHealth = 90;

        private float drainRate;

        [SerializeField]
        private HealthDrainInfo healthDrain = new HealthDrainInfo(
            1,
            1,
            1,
            60,
            0.1f,
            10);


        [SerializeField] internal const float startSpeed = 10;
        [SerializeField]
        internal AnimationCurve speedCurve = new AnimationCurve(
            new Keyframe[] {
                new Keyframe(0f,2f),
                new Keyframe(0.1f,2f),
                new Keyframe(1f,1f),
                new Keyframe(1.5f,0f)
            }
        );


        public float getSpeedForHealth(float currentHealth)
        {
            return startSpeed * (speedCurve.Evaluate(currentHealth / startHealth));
        }

        public IEnumerator HealthDrain()
        {
            StartCoroutine(MoreHealthDrain());
            while (true)
            {
                yield return new WaitForSeconds(healthDrain.healthDrainInterval);
                DrainHealth();
            }
        }

        public IEnumerator MoreHealthDrain()
        {
            yield return new WaitForSeconds(healthDrain.delayForIncreasedDrain);
            while (true)
            {
                drainRate += healthDrain.increasedDrainIncrement;
                yield return new WaitForSeconds(healthDrain.increasedDrainInterval);
            }
        }

        void DrainHealth()
        {

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
