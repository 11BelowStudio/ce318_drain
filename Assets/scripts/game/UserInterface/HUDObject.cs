using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Assets.scripts.game.players;
using TMPro;
using Assets.scripts.game.weapons;
using Assets.scripts.game.control;

namespace Assets.scripts.game.UserInterface
{
    public class HUDObject : GenericHUD, IPlayerHUD
    {



        [SerializeField] private HealthHUD healthHUD;

        [SerializeField] private EndGameTextDisplay startEndText;


        [SerializeField] private RawImage crosshair;


        [SerializeField] private PauseMenu thePauseMenu;

        private bool youAreDead;



        // Use this for initialization
        public override void Start()
        {
            crosshair.enabled = true;

            startEndText.enabled = false;

            healthHUD.enabled = false;

            thePauseMenu.enabled = false;
            youAreDead = false;
        }



        public override void SetHealth(float health)
        {
            if (!youAreDead)
            {
                healthHUD.SetHealth(health);
            }
        }

       

        public override void GameHasStarted()
        {
            startEndText.enabled = false;
            //healthHUD.gameObject.SetActive(true);
            healthHUD.enabled = true;
            crosshair.enabled = true;

        }

  

        public override void GameOver(bool won)
        {
            youAreDead = true;
            if (!won)
            {
                healthHUD.SetHealth(0f);
            }
            crosshair.enabled = false;
            //healthHUD.enabled = false;
            startEndText.enabled = true;
            startEndText.GameOver(won);
        }

        public override void Pause()
        {
            Debug.Log("pls show pause menu");
            crosshair.enabled = false;
            thePauseMenu.enabled = true;
            thePauseMenu.gameObject.SetActive(true);
        }

        public override void Unpause()
        {
            Debug.Log("pls hide pause menu");
            crosshair.enabled = true;
            thePauseMenu.enabled = false;
            thePauseMenu.gameObject.SetActive(false);
        }

    }
}