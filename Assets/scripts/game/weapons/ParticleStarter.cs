using System.Collections;
using UnityEngine;

namespace Assets.scripts.game.weapons
{
    public class ParticleStarter : MonoBehaviour
    {

        [SerializeField]
        private ParticleSystem fireball;

        [SerializeField]
        private ParticleSystem smoke;


        public void ShootIt()
        {
            fireball.Play();
            smoke.Play();
        }

        private IEnumerator StopCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            fireball.Stop();
            smoke.Stop();
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}