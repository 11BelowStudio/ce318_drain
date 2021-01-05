using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

//Modified from the Unity Standard Assets First Person Character MouseLook

namespace Assets.scripts.game.players.human
{
    [Serializable]
    public class HumanLook : LookScript
    {


        public bool lockCursor = true;

        //private Quaternion m_CharacterTargetRot;
        //private Quaternion m_CameraTargetRot;
        private bool m_cursorIsLocked = true;

        //private PlayerInputs inputs;

        
        override public void Init(Transform character, Transform camera, ref Inputs theInputs)
        {
            base.Init(character, camera, ref theInputs);
            //MinimumX = -90;
            //MaximumX = 90;
            //inputs = theInputs;
            
        }

        
        override public void LookRotation(Transform character, Transform camera, Vector3 lookVector = new Vector3())
        {

            if (lockCursor)
            {
                base.LookRotation(character, camera);
            }
            UpdateCursorLock();

        }



        public override void SetCursorLock(bool value)
        {
            lockCursor = value;
            m_cursorIsLocked = value;
            if(lockCursor)
            {
                //lock it
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {//we force unlock the cursor if the user disable the cursor locking helper
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public override void UpdateCursorLock()
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (lockCursor)
                InternalLockUpdate();
        }

        protected override void InternalLockUpdate()
        {
            
            if(Input.GetMouseButtonUp(0))
            {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }


    }
}
