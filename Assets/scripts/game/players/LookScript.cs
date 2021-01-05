using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Assets.scripts.game.players
{
	//Edited slightly from 'MouseLook.cs' from Unity Standard Assets - Characters - First Person Character
    [Serializable]
    public class LookScript
    {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public float MinimumX;
        public float MaximumX;
        public bool smooth;
        public float smoothTime = 5f;
        

        protected Inputs inputs;




        protected Quaternion m_CharacterTargetRot;
        protected Quaternion m_CameraTargetRot;


        virtual public void Init(Transform character, Transform camera, ref Inputs theInputs)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
            inputs = theInputs;

        }

        

        public void ChangeCamera(Transform camera)
        {
            m_CameraTargetRot = camera.localRotation;
        }

        
        public virtual void LookRotation(Transform character, Transform camera, Vector3 lookVector = new Vector3())
        {

            
            Vector2 looks = inputs.LookInput;

            float leftRightRotation = looks.x * XSensitivity;
            float upDownRotation = looks.y * YSensitivity;
            
            
            

            m_CharacterTargetRot *= Quaternion.Euler(0f, leftRightRotation, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-upDownRotation, 0f, 0f);

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (smooth)
            {
                character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }


        }
        

        public virtual void SetCursorLock(bool value)
        {
            
        }

        public virtual void UpdateCursorLock()
        {

        }

        protected virtual void InternalLockUpdate()
        {

        }



        protected Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }
    }

}
