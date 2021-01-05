using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

namespace Assets.scripts.DrainMenu
{
    public class SettingsMenu : MonoBehaviour
    {
        public DrainMenuControl drainMenu;

        public Slider volumeSlider;

        public TMP_Dropdown difficulty;

        public Button backButton;

        public TextMeshProUGUI settingsTitle;

        public TextMeshProUGUI volumeText;

        public TextMeshProUGUI difficultyText;

        public AudioMixer mixer;

        public CanvasGroup settingsGroup;

        // Use this for initialization
        void Start()
        {
            volumeSlider.value = PlayerPrefs.GetFloat("volume", 0.75f);
            mixer.SetFloat("volume", (volumeSlider.value == 0) ? -80f : Mathf.Log10(volumeSlider.value) * 20);

            difficulty.value = PlayerPrefs.GetInt("difficulty", 1);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            volumeSlider.gameObject.SetActive(true);
            difficulty.gameObject.SetActive(true);
            backButton.gameObject.SetActive(true);

            settingsTitle.gameObject.SetActive(true);
            volumeText.gameObject.SetActive(true);
            difficultyText.gameObject.SetActive(true);


        }

        private void OnDisable()
        {
            volumeSlider.gameObject.SetActive(false);
            difficulty.gameObject.SetActive(false);
            backButton.gameObject.SetActive(false);

            settingsTitle.gameObject.SetActive(false);
            volumeText.gameObject.SetActive(false);
            difficultyText.gameObject.SetActive(false);
        }

        public void VolumeSliderUpdate()
        {
            
            PlayerPrefs.SetFloat("volume", volumeSlider.value);
            mixer.SetFloat("volume", (volumeSlider.value == 0)? -80f :  Mathf.Log10(volumeSlider.value) * 20);

        }

        public void DifficultyUpdate()
        {
            PlayerPrefs.SetInt("difficulty", difficulty.value);
        }

        public void Done()
        {
            PlayerPrefs.Save();
            drainMenu.BackToMain();
        }



    }
}