using Assets.scripts.utilities.fader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;


namespace Assets.scripts.DrainMenu
{
    public class DrainMenuControl : MonoBehaviour
    {

        DrainMenuStates currentState;

        public Button levelButton;

        public Button settingsButton;

        public Button quitButton;

        public ChooseLevelScript choose;

        public SettingsMenu settings;

        public AudioMixer mixer;

        public RawImage mainImage;

        public RawImage bgImage;

        public CanvasGroup mainCanvas;

        public CanvasGroup bgCanvas;

        //public float fadeTime = 1f;

        //private float fadeTimeLeft;

        private FadeStates fadeState;

        // Start is called before the first frame update
        void Start()
        {
            mainCanvas.alpha = 1f;
            bgCanvas.alpha = 0f;
            //bgImage.enabled = false;
            float vol = PlayerPrefs.GetFloat("volume", 0.75f);
            mixer.SetFloat("volume", (vol == 0) ? -80f : Mathf.Log10(vol) * 20);
            currentState = DrainMenuStates.main;
            ShowThis();
            choose.chooseGroup.alpha = 0f;
            choose.gameObject.SetActive(false);
            settings.settingsGroup.alpha = 0f;
            settings.gameObject.SetActive(false);

        }

        // Update is called once per frame
        void Update()
        {
            switch (fadeState)
            {
                case FadeStates.none:
                    break;
                case FadeStates.fadeToOther:
                    float alpha = Mathf.Clamp01(bgCanvas.alpha += Time.deltaTime);
                    mainCanvas.alpha = (1 - alpha);
                    switch (currentState)
                    {
                        case DrainMenuStates.chooseLevel:
                            choose.chooseGroup.alpha = (alpha);
                            break;
                        case DrainMenuStates.settings:
                            settings.settingsGroup.alpha = (alpha);
                            break;
                    }
                    if (alpha == 1f)
                    {
                        mainImage.enabled = false;
                        levelButton.gameObject.SetActive(false);
                        settingsButton.gameObject.SetActive(false);
                        quitButton.gameObject.SetActive(false);
                        fadeState = FadeStates.none;
                    }
                    break;
                case FadeStates.fadeToMainFromLevels:
                    float alpha2 = Mathf.Clamp01(bgCanvas.alpha -= Time.deltaTime);
                    choose.chooseGroup.alpha = (alpha2);
                    mainCanvas.alpha = (1 - alpha2);
                    if (alpha2 == 0f)
                    {
                        bgImage.enabled = false;
                        choose.enabled = false;
                        choose.gameObject.SetActive(false);
                        fadeState = FadeStates.none;
                    }
                    break;
                case FadeStates.fadeToMainFromSettings:
                    float alpha3 = Mathf.Clamp01(bgCanvas.alpha -= Time.deltaTime);
                    settings.settingsGroup.alpha = (alpha3);
                    mainCanvas.alpha = (1 - alpha3);
                    if (alpha3 == 0f)
                    {
                        bgImage.enabled = false;
                        settings.enabled = false;
                        settings.gameObject.SetActive(false);
                        fadeState = FadeStates.none;
                    }
                    break;
            }
        }

        public void GiveUp()
        {
            Application.Quit(1);
        }

        public void Settings()
        {
            HideThis();
            bgImage.enabled = true;
            settings.enabled = true;
            settings.gameObject.SetActive(true);
            currentState = DrainMenuStates.settings;
        }

        public void ChooseLevel()
        {
            HideThis();
            bgImage.enabled = true;
            choose.enabled = true;
            choose.gameObject.SetActive(true);
            currentState = DrainMenuStates.chooseLevel;
        }


        public void ShowThis()
        {
            mainImage.enabled = true;
            levelButton.gameObject.SetActive(true);
            settingsButton.gameObject.SetActive(true);
            quitButton.gameObject.SetActive(true);
        }

        public void HideThis()
        {
            //mainImage.enabled = false;
            //levelButton.gameObject.SetActive(false);
            //settingsButton.gameObject.SetActive(false);
            //quitButton.gameObject.SetActive(false);
            fadeState = FadeStates.fadeToOther;
        }

        public void BackToMain()
        {
            switch (currentState)
            {
                case DrainMenuStates.chooseLevel:
                    //choose.enabled = false;
                    //choose.gameObject.SetActive(false);
                    fadeState = FadeStates.fadeToMainFromLevels;
                    break;
                case DrainMenuStates.settings:
                    //settings.enabled = false;
                    fadeState = FadeStates.fadeToMainFromSettings;
                    //settings.gameObject.SetActive(false);
                    break;
            }
            currentState = DrainMenuStates.main;
            //bgImage.enabled = false;
            ShowThis();
        }
    }


    public enum DrainMenuStates
    {
        main,
        settings,
        chooseLevel
    }

    public enum FadeStates
    {
        none,
        fadeToMainFromSettings,
        fadeToMainFromLevels,
        fadeToOther
    }
}