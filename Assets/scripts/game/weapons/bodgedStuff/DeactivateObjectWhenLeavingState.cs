using System.Collections;
using UnityEngine;

namespace Assets.scripts.game.weapons.bodge
{
    public class DeactivateObjectWhenLeavingState : StateMachineBehaviour
    {
        [SerializeField]
        private GameObject theParentObject;

        void OnStateExit()
        {
            theParentObject.SetActive(false);
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}