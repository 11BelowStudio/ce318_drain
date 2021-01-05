using System.Collections;
using UnityEngine;

namespace Assets.scripts.game.weapons
{
    public class speen : MonoBehaviour
    {

        //private Quaternion speenQuat = Quaternion.AngleAxis(120f / Time.fixedDeltaTime, Vector3.forward);
        private Rigidbody rb;
        private Vector3 rotate = new Vector3(0f, 0f, 120f);

        // Use this for initialization
        void Start()
        {
            rb = GetComponent<Rigidbody>();

            rb.AddRelativeTorque(rotate, ForceMode.VelocityChange);
            
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            rb.position = transform.root.position;

            //this.transform.Rotate(rotate * Time.fixedDeltaTime);

        }

    }
}