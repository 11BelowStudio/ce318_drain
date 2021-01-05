using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.scripts.utilities.dbt
{
    public class DestroyByTime : MonoBehaviour
    {

        //yep, this is also pretty much from lab 2

        [SerializeField]
        private float destructionTime;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Destroy());
        }


        private IEnumerator Destroy()
        {
            yield return new WaitForSeconds(destructionTime);
            Destroy(this.gameObject);
            yield break;
        }



    }

}
