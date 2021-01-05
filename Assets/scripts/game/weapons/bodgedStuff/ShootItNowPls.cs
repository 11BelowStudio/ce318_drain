using System.Collections;
using UnityEngine;

namespace Assets.scripts.game.weapons.bodge
{
    public class ShootItNowPls : StateMachineBehaviour
    {

        [SerializeField]
        private GameObject weapon;



        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        

        void OnStateEnter()
        {
            weapon.GetComponentInChildren<ParticleStarter>().ShootIt();

        }

        void OnStateExit()
        {
            weapon.GetComponent<Animator>().ResetTrigger("Shoot");
        }
    }
}