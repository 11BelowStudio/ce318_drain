using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.scripts.game.control;
using Assets.scripts.game.players;

namespace Assets.scripts.game
{
    public class WeaponBox : MonoBehaviour
    {

        private bool isUsable;

        public float boxRechargeTime = 30f;

        private Vector3 speen = new Vector3(0, 30, 0);


        // Start is called before the first frame update
        void Start()
        {
            isUsable = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (isUsable)
            {
                //speen
                transform.Rotate(speen * Time.deltaTime);
            }
        }


        
        public void OnTriggerEnter(Collider other)
        {
            if (isUsable)
            {
                switch (other.gameObject.tag)
                {
                    //if a contestant hits it
                    case "Enemy":
                    case "Player":
                        {
                            //initially makes box unusable/invisible
                            this.isUsable = false;
                            this.gameObject.GetComponent<Renderer>().enabled = false;
                            //give it to the contestant
                            //other.gameObject.GetComponent<ModifiedFirstPersonController>().ChangeWeapon();
                            other.gameObject.GetComponent<Contestant>().ChangeWeapon();
                            //respawn it
                            StartCoroutine(RespawnCoroutine());
                            break;
                        }

                }

            }
        }

        //coroutine for respawning the weapon box
        private IEnumerator RespawnCoroutine()
        {
            //need to wait for the boxRechargeTime
            yield return new WaitForSeconds(boxRechargeTime);
            //make it usable/visible again
            this.isUsable = true;
            this.gameObject.GetComponent<Renderer>().enabled = true;
            //done
            yield break;
        }



    }

}
