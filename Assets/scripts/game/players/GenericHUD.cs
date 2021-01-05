using System.Collections;
using UnityEngine;
using Assets.scripts.game.weapons;
using Assets.scripts.game.control;

namespace Assets.scripts.game.players
{
    public class GenericHUD : MonoBehaviour, IPlayerHUD
    {

        // Use this for initialization
        public virtual void Start()
        {

        }

        // Update is called once per frame
        public virtual void Update()
        {

        }


        public virtual void GameHasStarted() { }

        public virtual void SetHealth(float health) { }

        

        public virtual void SetWeapon(TheWeaponEnum weapon) { }

        public virtual void GameOver(bool won) { }

        public virtual void Pause() {
            Debug.Log("Base pause");
        }

        public virtual void Unpause() { }


        public virtual void Shoot() { }

    }
}