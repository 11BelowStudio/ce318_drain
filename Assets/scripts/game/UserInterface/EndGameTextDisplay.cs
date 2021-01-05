using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.scripts.game.control;

namespace Assets.scripts.game.UserInterface
{
    public class EndGameTextDisplay : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI endMessageTMP;
        //[SerializeField] private TextMeshProUGUI resetTextTMP;


        [SerializeField] private string winMessage;
        [SerializeField] private string drainedMessage;

        //[SerializeField] private string resetMessage;

        [SerializeField] private Button resetButton;

        [SerializeField] private Button quitButton;

        private bool victory;

        // Use this for initialization
        void Start()
        {
            

        }

        private void OnEnable()
        {
            endMessageTMP.enabled = true;
            resetButton.gameObject.SetActive(true);
            resetButton.enabled = true;
            quitButton.gameObject.SetActive(true);
            quitButton.enabled = true;
        }

        

        public void GameOver(bool won)
        {
            victory = won;
            endMessageTMP.enabled = true;
            if (won)
            {
                endMessageTMP.SetText(winMessage);
            }
            else
            {
                endMessageTMP.SetText(drainedMessage);
            }
            ShowQuitText();
        }

        public void ShowLost()
        {
            endMessageTMP.enabled = true;
            endMessageTMP.SetText(drainedMessage);
            ShowQuitText();
        }

        public void ShowWon()
        {
            endMessageTMP.enabled = true;
            endMessageTMP.SetText(winMessage);
            ShowQuitText();
        }

        private void ShowQuitText()
        {
            //resetTextTMP.enabled = true;
            //resetTextTMP.SetText(resetMessage);
            //resetButton.gameObject.SetActive(true);
            resetButton.enabled = true;
            //quitButton.gameObject.SetActive(true);
            quitButton.enabled = true;
        }

        public void QuitButton()
        {
            GameObject.FindObjectOfType<GameControl>().QuitPressed(victory);
        }

        public void ResetButton()
        {
            GameObject.FindObjectOfType<GameControl>().ResetScene(victory);
        }

        public void OnMouseDown()
        {
            Debug.Log("deez nutz!");
        }

        void OnDisable()
        {
            //endMessageTMP.SetText("");
            endMessageTMP.enabled = false;
            resetButton.gameObject.SetActive(false);
            quitButton.gameObject.SetActive(false);
            //resetTextTMP.SetText("");
            // resetTextTMP.enabled = false;

            //resetButton.gameObject.SetActive(false);
            resetButton.enabled = false;
            //quitButton.gameObject.SetActive(false);
            quitButton.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}