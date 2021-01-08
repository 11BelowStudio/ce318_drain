using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets.scripts.game.UserInterface
{
    public class HealthHUD : HealthDisplay
    {

        [SerializeField] private TextMeshProUGUI healthTMP;

        [SerializeField] private Slider healthbar;
        [SerializeField] private Slider overhealBar;
        private bool overhealed = false;

        //[SerializeField]
        //private RawImage hurtIndicator;
        [SerializeField]
        private CanvasGroup hurtUnderlay;

        //[SerializeField]
        //private Color hurtColour;

        private float weaponHealthDrain;

        


        // Use this for initialization
        void Start()
        {
            //healthTMP.enabled = false;
            //overhealBar.enabled = false;
            healthbar.enabled = false;
            //overhealBar.gameObject.SetActive(false);
            //healthbar.gameObject.SetActive(false);
            
            //hurtColour = hurtIndicator.color;
            //hurtColour.a = 0;
            //hurtIndicator.color = hurtColour;
        }



        private void OnEnable()
        {
            healthbar.gameObject.SetActive(true);
            healthbar.enabled = true;
            //overhealBar.enabled = false;
            overhealBar.gameObject.SetActive(false);
            overhealBar.enabled = false;
            hurtUnderlay.gameObject.SetActive(true);
            //hurtIndicator.enabled = true;
            healthTMP.enabled = true;
            //healthTMP.gameObject.SetActive(true);
            //healthTMP.enabled = true;
            //healthbar.fillRect.gameObject.SetActive(true);
            //healthbar.value = 60;
            //hurtIndicator.enabled = true;
            SetHealth(60);
        }

        private void OnDisable()
        {
            //healthTMP.enabled = false;
            //overhealBar.gameObject.SetActive(false);
            //healthbar.gameObject.SetActive(false);
        }

        public void SetWeaponHealthDrain(float weaponDrain)
        {
            weaponHealthDrain = weaponDrain;
        }

        public override void SetHealth(float health)
        {

            healthTMP.SetText($"{health.ToString("0.0")}");

            if (health > 60)
            {
                if (!overhealed)
                {
                    overhealed = true;
                    overhealBar.gameObject.SetActive(true);
                    //hurtIndicator.enabled = false;
                }
                overhealBar.value = health;
            }
            else
            {
				if (health < 0f){
					healthTMP.SetText("0");
					health = 0f;
				}
                healthbar.value = health;
                if (overhealed)
                {
                    overhealBar.gameObject.SetActive(false);
                    overhealed = false;
                    //hurtIndicator.enabled = true;
                }
                
            }
            //hurtColour.a = Mathf.Clamp01(1 - (health / 60));
            //hurtIndicator.color = hurtColour;
            hurtUnderlay.alpha = Mathf.Clamp01(1 - (health / 60));

        }
		
		public void UrDed()
		{
			healthTMP.SetText("0");
			healthbar.fillRect.gameObject.SetActive(false);
			if (overhealed)
			{
				overhealBar.gameObject.SetActive(false);
				overhealed = false;
				//hurtIndicator.enabled = true;
			}
			hurtUnderlay.alpha = 1f;
		}

        // Update is called once per frame
        void Update()
        {

        }
    }
}