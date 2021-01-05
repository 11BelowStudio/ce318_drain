using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.scripts.utilities.fader;
using TMPro;
using UnityStandardAssets.CrossPlatformInput;

namespace drained
{
    /// <summary>
    /// HERE'S A GAME WHICH IS INFINITELY MORE ENTERTAINING THAN THE MAIN GAME.
    /// 
    /// YES I HATE THE MAIN GAME ALREADY
    /// 
    /// AND IT'S TOO LATE TO GO BACK AND CHANGE IT NOW
    /// </summary>
    public class Drained : MonoBehaviour
    {

        public string[] buttonMessages;

        public Button pressThis;

        public TextMeshProUGUI buttonText;

        public RawImage unityMachineBroke;

        public RawImage cSharpMachineBroke;

        public Slider timeLeft;

        public Slider progress;

        public Slider energy;

        public TextMeshProUGUI introText;

        private bool hitButton;

        private bool gameOver;

        private readonly Object LOCK_OBJECT = new Object();

        private float energyDrainAmount;

        private bool settingUp;

        private float energyWait;

        private bool regenEnergy;

        public AudioSource badMusic;

        bool audioLooped;

        private float GetEnergyDrain()
        {
            float old = 0.1f;
            lock (LOCK_OBJECT)
            {
                old = energyDrainAmount;
                energyDrainAmount += 0.1f;
                StartCoroutine(DecreaseEnergyDrain());
            }
            return old;
        }

        private IEnumerator DecreaseEnergyDrain()
        {
            yield return new WaitForSeconds(1);
            lock (LOCK_OBJECT)
            {
                energyDrainAmount -= 0.1f;
                if (energyDrainAmount < 0.1f)
                {
                    energyDrainAmount = 0.1f;
                }
            }
            yield break;
        }

        // Start is called before the first frame update
        void Start()
        {
            hitButton = false;
            gameOver = false;
            unityMachineBroke.enabled = false;
            cSharpMachineBroke.enabled = false;
            pressThis.gameObject.SetActive(false);
            introText.enabled = true;
            energyDrainAmount = 0.1f;
            settingUp = true;
            regenEnergy = false;
            audioLooped = false;
            StartCoroutine(HahaYes());
        }

        // Update is called once per frame
        void Update()
        {
            if (settingUp)
            {
                energy.value += ((energy.maxValue / 3) * Time.deltaTime);
                
                timeLeft.value += ((timeLeft.maxValue / 3) * Time.deltaTime);
            }
            else
            {
                timeLeft.value -= ((timeLeft.maxValue / 110) * Time.deltaTime);
                if (timeLeft.value == timeLeft.minValue)
                {
                    gameOver = true;
                }
                if (regenEnergy)
                {
                    energy.value += (((energy.maxValue) / 10) / energyWait) * Time.deltaTime;
                }
            }
        }

        public void hitIt()
        {
            if (energy.value >= 0.1f)
            {
                hitButton = true;
                energy.value -= GetEnergyDrain();
                progress.value += (Random.value * 0.15f);
                if (progress.value > 0.8 || Random.value > 0.75f)
                {
                    progress.value -= (0.1f + (Random.value * 0.15f));
                    if (progress.value == 1)
                    {
                        progress.value = 0.9f;
                    }
                }
            }
        }

        private IEnumerator EnergyRegen()
        {
            do
            {
                yield return new WaitUntil(() => energy.value <= energy.maxValue);
                float waitTime = Random.Range(2f, 5f);
                energyWait = waitTime;
                regenEnergy = true;
                yield return new WaitForSeconds(waitTime);
                regenEnergy = false;
                //energy.value += 0.1f;
            } while (!gameOver);
            yield break;
        }

        private IEnumerator HahaYes()
        {
            yield return new WaitForSeconds(3);
            energy.value = energy.maxValue;
            timeLeft.value = timeLeft.maxValue;
            settingUp = false;
            introText.enabled = false;
            //StartCoroutine(Countdown());
            StartCoroutine(EnergyRegen());
            do
            {
                yield return new WaitForSeconds(Random.value * 0.5f);
                float f = Random.value;
                if (f > 0.25f)
                {
                    
                    //pressThis.transform.position = new Vector3(((Random.value * 2) - 1) * 760, ((Random.value * 2) - 1) * 233);
                    pressThis.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(((Random.value * 2) - 1) * 760, ((Random.value * 2) - 1) * 233);
                    buttonText.SetText(buttonMessages[Random.Range(0, buttonMessages.Length)]);
                    pressThis.gameObject.SetActive(true);
                    yield return new WaitUntil( () => (hitButton || gameOver));
                    pressThis.gameObject.SetActive(false);
                    hitButton = false;
                }
                else
                {
                    if (f > 0.125f)
                    {
                        unityMachineBroke.enabled = true;
                        //progress.value -= 0.2f;
                        yield return new WaitForSeconds(5);
                        unityMachineBroke.enabled = false;
                    }
                    else
                    {
                        cSharpMachineBroke.enabled = true;
                        //progress.value -= 0.2f;
                        yield return new WaitForSeconds(5);
                        cSharpMachineBroke.enabled = false;
                    }
                }
            } while (!gameOver);
            introText.enabled = true;
            introText.SetText("congrats ur game sucks");

            
            Coroutine waitForLoop = StartCoroutine(WaitingForTheAudioToLoop());
            yield return new WaitUntil( () => ( CrossPlatformInputManager.GetButtonDown("Fire1") || audioLooped ) );
            FindObjectOfType<FaderScript>().ChangeLevel(0, FaderScript.FadeType.WHITE);
            StopCoroutine(waitForLoop);
            yield break;

        }

        private IEnumerator WaitingForTheAudioToLoop()
        {
            float currentAudioTime = badMusic.time;
            yield return new WaitForSeconds(badMusic.clip.length - currentAudioTime);
            audioLooped = true;
            yield break;
        }


        private IEnumerator Countdown()
        {
            while (timeLeft.value > 0)
            {
                yield return new WaitForSeconds(1f);
                timeLeft.value -= 0.01f;
            }
            gameOver = true;
            yield break;
        }
    }

}