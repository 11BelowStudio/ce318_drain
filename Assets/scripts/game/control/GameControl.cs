using Assets.scripts.game.players;
using Assets.scripts.game.weapons;
using Assets.scripts.utilities.fader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

namespace Assets.scripts.game.control
{
    public class GameControl : MonoBehaviour, IListenToThePlayers, IDoThePauseMenuStuff, IListenToTheBots
    {

        private ISet<Contestant> contestants;

        private GameStates currentState = GameStates.WAITING_TO_START;


        [SerializeField] private Pistol pistol;
        [SerializeField] private Shotgun shotgun;
        [SerializeField] private SomewhatBigGun sbg;


        [SerializeField] private AudioClip startEndNoise;

        private AudioSource noiseSource;

        public AudioMixer mixer;

        // Use this for initialization
        void Start()
        {
            float vol = PlayerPrefs.GetFloat("volume", 0.75f);
            mixer.SetFloat("volume", (vol == 0) ? -80f : Mathf.Log10(vol) * 20);


            gameObject.GetComponent<Pistol>().enabled = true;
            pistol = this.gameObject.GetComponent<Pistol>();

            gameObject.GetComponent<Shotgun>().enabled = true;
            shotgun = gameObject.GetComponent<Shotgun>();

            gameObject.GetComponent<SomewhatBigGun>().enabled = true;
            sbg = gameObject.GetComponent<SomewhatBigGun>();



            currentState = GameStates.WAITING_TO_START;

            Time.timeScale = 1;

            noiseSource = gameObject.GetComponent<AudioSource>();
            contestants = new HashSet<Contestant>(FindObjectsOfType<Contestant>());

            //setting difficulty of bots
            int difficulty = PlayerPrefs.GetInt("difficulty", 1);
            foreach(Contestant c in contestants)
            {
                c.SetDifficulty(difficulty);
            }
            



        }

        void Update()
        {
            switch (currentState)
            {
               
                case GameStates.JUST_STARTED:
                    {
                        noiseSource.Stop();
                        foreach (ICanGetStartedAndStopped c in contestants)
                        {
                            c.SetCanMove(true);
                            c.WillYouStartTheDrainsPlease();
                        }

                        currentState = GameStates.GAME_STARTED;
                        PlayStartEndNoise();

                        break;
                    }
                case GameStates.GAME_STARTED:
                    {
                        
                        //hearing if anyone has shot their weapon, and letting the others hear if if someone has shot
                        foreach(IMayOrMayNotHaveJustShot sus in contestants)
                        {
                            if (sus.HaveIJustShot())
                            {
                                Vector3 thisArea = sus.IAmHere();
                                foreach(ICanHearYou hey in GetAllOtherContestants(sus))
                                {
                                    hey.IHeardYouWereAround(thisArea);
                                }
                                break;
                            }
                        }
                        
                        break;
                    }
                case GameStates.GAME_OVER:
                    {
                        break;
                    }
            }


        }

        void LateUpdate()
        {
            switch (currentState)
            {
                case GameStates.GAME_STARTED:
                    {
                        ISet<Contestant> stillAlive = GetAllContestants();
                        foreach (IGetAskedIfImDead c in stillAlive)
                        {
                            if (c.AreYouDeadYet())
                            {
                                contestants.Remove((Contestant)c);
                                c.GameOver();
                                if (contestants.Count == 1)
                                {
                                    break;
                                }
                            }


                        }
                        if (contestants.Count == 1)
                        {
                            contestants.First<IGetAskedIfImDead>().GameOver(true);
                            currentState = GameStates.GAME_OVER;
                        }
                        else if (!contestants.Any( c => c.AreYouAHuman()))
                        {
                            currentState = GameStates.GAME_OVER;
                            //just throw in the towel now if there's no humans left.
                            foreach (Contestant c in contestants)
                            {
                                c.GameOver(true);
                            }
                        }
                        break;
                    }
            }

        }


        public FSMWeapons GetFSMWeapons(IMayOrMayNotBeTryingToShoot whoAsked)
        {
            return new FSMWeapons(pistol, shotgun, sbg, whoAsked);
        }

        public ISet<Contestant> GetAllContestants()
        {
            return new HashSet<Contestant>(contestants);
        }

        public ISet<Contestant> GetAllOtherContestants(IAmTheParentOfAllContestants butNotMeImBiggerThanThat)
        {
            ISet<Contestant> everyoneElse = GetAllContestants();
            everyoneElse.Remove((Contestant)butNotMeImBiggerThanThat);
            return everyoneElse;
        }

        public void StartGame()
        {
            switch (currentState)
            {
                case GameStates.WAITING_TO_START:
                    //TODO: this

                    currentState = GameStates.JUST_STARTED;
                    break;
                
            }
        }

        public void PauseGame()
        {
            switch (currentState)
            {
                case GameStates.GAME_STARTED:

                    //「ZA WARUDO」! Toki yo Tomare!
                    Time.timeScale = 0;

                    Debug.Log("ZA WARUDO!");
                    //let everyone know it's paused
                    foreach(IAmAContestantWhoGetsPausedAndUnpaused c in contestants)
                    {
                        c.Pause();
                    }

                    currentState = GameStates.GAME_PAUSED;
                    break;
                case GameStates.GAME_PAUSED:
                    Debug.Log("toki wa ugokidasu");
                    Unpause();
                    break;
            }
        }

        public void Unpause()
        {
            switch (currentState)
            {
                case GameStates.GAME_PAUSED:
                    //let everyone know it's unpaused
                    foreach (IAmAContestantWhoGetsPausedAndUnpaused c in contestants)
                    {
                        c.Unpause();
                    }

                    //Toki wa Ugokidasu
                    Time.timeScale = 1;
                    currentState = GameStates.GAME_STARTED;
                    break;
            }
        }

       

        public void GaveUp()
        {
            EndingTheGame();
            FindObjectOfType<FaderScript>().ChangeLevel(0, FaderScript.FadeType.WHITE);
        }

        public void QuitPressed(bool won = false)
        {
            EndingTheGame();
            FindObjectOfType<FaderScript>().ChangeLevel(0,(won)? FaderScript.FadeType.BLACK : FaderScript.FadeType.BLOOD_ORANGE);
        }

        public void ResetScene(bool won = false)
        {
            EndingTheGame();
            FindObjectOfType<FaderScript>().ChangeLevel(SceneManager.GetActiveScene().buildIndex, (won) ? FaderScript.FadeType.WHITE : FaderScript.FadeType.BLOOD_ORANGE);
        }

        private void EndingTheGame()
        {
            currentState = GameStates.QUIT_GAME;
            //stop everyone for good
            foreach (ICanGetStartedAndStopped c in contestants)
            {
                c.SetCanMove(false); //「ZA WARUDO」!
                c.WillYouStopTheDrainsPlease(); //as well as the drains
            }

            PlayStartEndNoise();

            //Toki wa Ugokidasu
            Time.timeScale = 1;
        }

        private void PlayStartEndNoise()
        {
            noiseSource.PlayOneShot(startEndNoise);
        }
    }


    public interface IAmTheRootOfAllGameControl {
        ISet<Contestant> GetAllContestants();
    }
    

    /// <summary>
    /// An interface that players call
    /// </summary>
    public interface IListenToThePlayers: IAmTheRootOfAllGameControl
    {
        FSMWeapons GetFSMWeapons(IMayOrMayNotBeTryingToShoot whoAsked);

        void StartGame();

        void PauseGame();

        
    }

    public interface IListenToTheBots: IAmTheRootOfAllGameControl
    {
        ISet<Contestant> GetAllOtherContestants(IAmTheParentOfAllContestants butNotMeImBiggerThanThat);
    }

    public interface IDoThePauseMenuStuff: IAmTheRootOfAllGameControl
    {
        void Unpause();

        void GaveUp();
    }


}