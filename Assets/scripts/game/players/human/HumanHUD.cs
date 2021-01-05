using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.scripts.game.UserInterface;
using Assets.scripts.game.weapons;
using Assets.scripts.game.control;

namespace Assets.scripts.game.players.human
{
    public class HumanHUD : GenericHUD
    {

        [SerializeField] private HUDObject theActualHUD;

        [SerializeField] private WeaponViewmodelControl viewModel;

        // Use this for initialization
        public override void Start()
        {
            viewModel.gameObject.SetActive(false);
            theActualHUD.gameObject.SetActive(false);
        }

        // Update is called once per frame
        public override void Update()
        {

        }

        public override void GameHasStarted()
        {
            theActualHUD.gameObject.SetActive(true);
            theActualHUD.enabled = true;
            SetHealth(60);
            theActualHUD.GameHasStarted();
            viewModel.gameObject.SetActive(true);
            viewModel.TimeToStart();
        }

        public override void SetHealth(float health)
        {

            theActualHUD.SetHealth(health);


          
        }

        public override void SetWeapon(TheWeaponEnum weapon)
        {
            viewModel.EquipWeapon(weapon);
        }

        

        public override void Pause()
        {
            theActualHUD.Pause();
        }

        public override void Unpause()
        {
            theActualHUD.Unpause();
        }

        public override void GameOver(bool won)
        {
            theActualHUD.GameOver(won);
            viewModel.RightThatsIt();
        }

        public override void Shoot()
        {
            viewModel.Shoot();
        }
    }
}