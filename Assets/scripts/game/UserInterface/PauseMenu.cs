using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.scripts.game.control;

namespace Assets.scripts.game.UserInterface
{
    public class PauseMenu : MonoBehaviour
    {

        [SerializeField]
        private TextMeshProUGUI pauseText;

        [SerializeField]
        private Button resumeButton;

        [SerializeField]
        private Button quitButton;

        // Use this for initialization
        void Start()
        {
            
        }

        

        void OnEnable()
        {
            //pauseText.gameObject.SetActive(true);
            pauseText.enabled = true;
            resumeButton.gameObject.SetActive(true);
            quitButton.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            pauseText.enabled = false;
            resumeButton.gameObject.SetActive(false);
            quitButton.gameObject.SetActive(false);
        }



        // Update is called once per frame
        void Update()
        {

        }


        public void Unpause()
        {
            FindObjectOfType<GameControl>().Unpause();
        }

        public void GaveUp()
        {
            FindObjectOfType<GameControl>().GaveUp();
        }
    }
}