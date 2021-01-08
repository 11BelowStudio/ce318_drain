using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.scripts.utilities.fader;

namespace Assets.scripts.DrainMenu
{
    public class ChooseLevelScript : MonoBehaviour
    {

        public Button chooseText;

        public Button theGutter;

        public Button theDitch;

        public Button back;

        public DrainMenuControl drainMenu;

        private int GUTTER_BUILD_INDEX = 1;

        private int DITCH_BUILD_INDEX = 2;

        private int DRAINED_BUILD_INDEX = 3;

        public CanvasGroup chooseGroup;

        // Use this for initialization
        void Start()
        {

        }

        private void OnEnable()
        {
            chooseText.gameObject.SetActive(true);
            theGutter.gameObject.SetActive(true);
            theDitch.gameObject.SetActive(true);
            back.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            chooseText.gameObject.SetActive(false);
            theGutter.gameObject.SetActive(false);
            theDitch.gameObject.SetActive(false);
            back.gameObject.SetActive(false);

        }
		
		private void DisableMenu(){
			chooseGroup.interactable = false;
		}

        public void TheGutter()
        {
			DisableMenu();
            FindObjectOfType<FaderScript>().ChangeLevel(GUTTER_BUILD_INDEX);
        }

        public void TheDitch()
        {
			DisableMenu();
            FindObjectOfType<FaderScript>().ChangeLevel(DITCH_BUILD_INDEX);
        }

        public void Drained()
        {
			DisableMenu();
            FindObjectOfType<FaderScript>().ChangeLevel(DRAINED_BUILD_INDEX, FaderScript.FadeType.WHITE);
        }

        public void GoBack()
        {
            drainMenu.BackToMain();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}