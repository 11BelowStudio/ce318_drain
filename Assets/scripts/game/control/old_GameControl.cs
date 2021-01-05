using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Assets.scripts.game.weapons;
using Assets.scripts.game.players;
using Assets.scripts.game.players.human;
using Assets.scripts.game.players.bot;
using static Assets.scripts.game.players.deprecated_ModifiedFirstPersonController;
using UnityEngine.SceneManagement;
using Assets.scripts.utilities.fader;


namespace Assets.scripts.game.control {
    public class old_GameControl : MonoBehaviour, IListenToThePlayers
    {


        public deprecated_ModifiedFirstPersonController thePlayer;
        public deprecated_BotController theBot;



        private HashSet<deprecated_BotController> bots = new HashSet<deprecated_BotController>();

        private HashSet<deprecated_ModifiedFirstPersonController> contestants_deprecated = new HashSet<deprecated_ModifiedFirstPersonController>();

        private ISet<Contestant> contestants = new HashSet<Contestant>();



        //public Text healthText;
        //public Text weaponText;
        //public Text startEndText;
        //public Text resetText;

        //public TextMeshProUGUI healthTMP;
        //public TextMeshProUGUI weaponTMP;
        //public TextMeshProUGUI startEndTMP;
        //public TextMeshProUGUI resetTextTMP;



        //[SerializeField] private string startMessage = "SPACEBAR TO START";
        //[SerializeField] private string winMessage = "WON.";
        //[SerializeField] private string drainedMessage = "DRAINED.";
        //public RawImage crosshair;

        
        private GameStates currentState = GameStates.WAITING_TO_START;


        [SerializeField] private Pistol pistol;
        [SerializeField] private Shotgun shotgun;
        [SerializeField] private SomewhatBigGun sbg;

        [SerializeField] LineRenderer shotLine;

        private AudioSource noiseSource;
        [SerializeField] private AudioClip startEndClip;

        // Start is called before the first frame update
        void Start()
        {

            //GameObject.FindGameObjectWithTag("Player").GetComponent<ModifiedFirstPersonController>().theHUD.PromptStart();
            
            
            //box.setGameControl(this);
            //UpdatePlayerHealthText();
            //UpdateWeaponText();
            //startEndText.text = startMessage;
            //startEndTMP.SetText(startMessage);
            //resetText.enabled = false;
            //crosshair.enabled = false;
            //resetTextTMP.enabled = false;

            //thePlayer.theHUD.PromptStart();

            
            //shotLine = gameObject.GetComponent<LineRenderer>();
            
            //shotLine.enabled = true;
            

            gameObject.GetComponent<Pistol>().enabled = true;
            pistol = this.gameObject.GetComponent<Pistol>();

            gameObject.GetComponent<Shotgun>().enabled = true;
            shotgun = gameObject.GetComponent<Shotgun>();

            gameObject.GetComponent<SomewhatBigGun>().enabled = true;
            sbg = gameObject.GetComponent<SomewhatBigGun>();

            currentState = GameStates.WAITING_TO_START;

            noiseSource = gameObject.GetComponent<AudioSource>();

            contestants = new HashSet<Contestant>(GameObject.FindObjectsOfType<Contestant>());
            
        }

        public FSMWeapons GetFSMWeapons(IMayOrMayNotBeTryingToShoot whoAsked)
        {
            return new FSMWeapons(pistol, shotgun, sbg, whoAsked);
        }

        public void HiThisHumanExists(DEPRECATED_HumanController human)
        {
            contestants_deprecated.Add(human);
        }

        public void HiThisBotExists(deprecated_BotController bot)
        {
            contestants_deprecated.Add(bot);
            bots.Add(bot);
        }

        // Update is called once per frame
        void Update()
        {
            switch (currentState)
            {
                case GameStates.WAITING_TO_START:
                    {
                        /*
                        if (thePlayer.inputs.JumpInput)
                        {
                            //start the game
                            thePlayer.theHUD.GameHasStarted();
                            //theBot.SetCanMove(true);
                            thePlayer.SetCanMove(true);
                            thePlayer.helf.WillYouStartTheDrainsPlease();
                            theBot.helf.WillYouStartTheDrainsPlease();

                            currentState = GameStates.GAME_STARTED;
                            theBot.Patrol();
                            thePlayer.inputs.CanShoot = true;
                            theBot.inputs.CanShoot = true;
                            noiseSource.PlayOneShot(startEndClip);
                        }
                        */
                        break;
                    }
                case GameStates.JUST_STARTED:
                    {
                        
                        foreach(deprecated_ModifiedFirstPersonController c in contestants_deprecated)
                        {
                            c.theHUD.GameHasStarted();
                            c.SetCanMove(true);
                            c.helf.WillYouStartTheDrainsPlease();
                            c.inputs.CanShoot = true;
                        }

                        //IEnumerator<ModifiedFirstPersonController> cIter = contestants.GetEnumerator();
                        //cIter.MoveNext();
                        //cIter.Current.helf.WillYouStartTheDrainsPlease();

                        foreach (deprecated_BotController b in bots)
                        {
                            b.Patrol();
                        }
                        //start the game
                        //thePlayer.theHUD.GameHasStarted();
                        //theBot.SetCanMove(true);
                        //thePlayer.SetCanMove(true);
                        //thePlayer.helf.WillYouStartTheDrainsPlease();
                        //theBot.helf.WillYouStartTheDrainsPlease();

                        currentState = GameStates.GAME_STARTED;
                        //theBot.Patrol();
                        //thePlayer.inputs.CanShoot = true;
                        //theBot.inputs.CanShoot = true;
                        noiseSource.PlayOneShot(startEndClip);

                        break;
                    }
                case GameStates.GAME_STARTED:
                    {
                        //shooting stuff
                        foreach (deprecated_ModifiedFirstPersonController c in contestants_deprecated)
                        {
                            if (c.inputs.ShootInput_deprecated)
                            {
                                c.helf.ChangeHealth(ShootHandler(c), true);
                            }
                        }
                        /*
                        if (thePlayer.inputs.ShootInput)
                        {
                            thePlayer.helf.ChangeHealth(ShootHandler(thePlayer), true);
                        }
                        if (theBot.inputs.ShootInput)
                        {
                            theBot.helf.ChangeHealth(ShootHandler(theBot), true);
                        }
                        */
                        //UpdatePlayerHealthText();
                        break;
                    }
                case GameStates.GAME_OVER:
                    {
                        if (thePlayer.inputs.PauseInput)
                        {
                            GameObject.Find("Fader").GetComponent<FaderScript>().ChangeLevel(SceneManager.GetActiveScene().buildIndex,FaderScript.FadeType.BLACK);
                            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //reload the scene
                        }
                        break;
                    }
            }

           
        }

        //shooting is handled by GameController because I don't trust the players to do it themselves.
        public float ShootHandler(deprecated_ModifiedFirstPersonController contestant)
        {
            bool doINeedToCheckIfThisIsTheBot = contestant.IsPlayer;
            foreach(deprecated_BotController b in bots)
            {
                //All bots will 'hear' the shot, and know where it was shot from.
                if (contestant.IsPlayer)
                {
                    b.SetLastKnownPositionOfPlayer(contestant.transform.position);
                } else if (!contestant.Equals(b))
                {
                    b.SetLastKnownPositionOfPlayer(contestant.transform.position);
                }
            }
            

            switch (contestant.EquippedWeapon)
            {
                case deprecated_WeaponEnum.Pistol:
                    {
                        return pistol.Deprecated_Shoot(contestant);
                    }
                case deprecated_WeaponEnum.Shotgun:
                    {
                        return shotgun.Deprecated_Shoot(contestant);
                    }
                case deprecated_WeaponEnum.SBG:
                    {
                        return sbg.Deprecated_Shoot(contestant);
                    }
            }
            return 0;
        }

        void LateUpdate()
        {
            switch (currentState)
            {
                case GameStates.GAME_STARTED:
                    {
                        ISet<deprecated_ModifiedFirstPersonController> stillAlive = new HashSet<deprecated_ModifiedFirstPersonController>();
                        foreach (deprecated_ModifiedFirstPersonController c in contestants_deprecated)
                        {
                            if (c.helf.ded)
                            {
                                if (c.IsPlayer)
                                {
                                    Drained();
                                }
                                else
                                {

                                    bots.Remove((deprecated_BotController)c);
                                    if (bots.Count == 0)
                                    {
                                        Win();
                                    }
                                }
                            }
                            else
                            {
                                stillAlive.Add(c);
                            }
                            
                        }
                        contestants_deprecated.UnionWith(stillAlive);
                        break;
                    }
            }
            
        }

        public void StartGame()
        {
            currentState = GameStates.JUST_STARTED;
        }

        public ISet<Contestant> GetAllPlayers()
        {
            //TODO: contestants
            return contestants;
        }

        public void PauseGame()
        {
            switch (currentState)
            {
                case GameStates.GAME_STARTED:
                    Time.timeScale = 0;
                    //TODO: let everyone know it's paused
                    currentState = GameStates.GAME_PAUSED;
                    break;

                case GameStates.GAME_PAUSED:
                    Time.timeScale = 1;

                    //TODO: let everyone know it's unpaused
                    currentState = GameStates.GAME_STARTED;
                    break;

            }
        }


        private void Win()
        {
            if (currentState == GameStates.GAME_OVER) { return; } //cant win if already drained
            currentState = GameStates.GAME_OVER;
            thePlayer.theHUD.GameOver(true);
            EndTheGame();
        }

        private void Drained()
        {
            thePlayer.theHUD.GameOver(false);
            EndTheGame();

        }

        private void EndTheGame()
        {
            currentState = GameStates.GAME_OVER;
            thePlayer.SetCanMove(false);
            theBot.SetCanMove(false);
            thePlayer.helf.WillYouStopTheDrainsPlease();
            theBot.helf.WillYouStopTheDrainsPlease();
            noiseSource.PlayOneShot(startEndClip);
        }

        public ISet<Contestant> GetAllContestants()
        {
            return new HashSet<Contestant>(contestants);
        }
    }

}