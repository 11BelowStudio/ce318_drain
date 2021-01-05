using System.Collections;
using System;
using UnityEngine;

namespace Assets.scripts.game.players.bot
{
    [Serializable]
    public class BotLookScript : LookScript
    {

        public override void LookRotation(Transform character, Transform camera, Vector3 lookVector)
        {

            

            
            
            Quaternion leftRightQuaternion = Quaternion.FromToRotation(character.forward, lookVector);
            Quaternion upDownQuaternion = Quaternion.FromToRotation(camera.forward, lookVector);

            //Debug.Log(leftRightQuaternion);

            float leftRightRotation = leftRightQuaternion.eulerAngles.y * XSensitivity;
            //float upDownRotation = upDownQuaternion.eulerAngles.z * YSensitivity;

            //Vector2 looks = inputs.LookInput;

            //float leftRightRotation = looks.x * XSensitivity;
            //float upDownRotation = looks.y * YSensitivity;


            float upDownRotation = 0;

            //m_CharacterTargetRot *= Quaternion.Euler(0f, leftRightRotation, 0f);
            m_CharacterTargetRot = Quaternion.LookRotation(lookVector, Vector3.up);
            m_CameraTargetRot *= Quaternion.Euler(-upDownRotation, 0f, 0f);

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (smooth)
            {
                character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                /*
                camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
                */
            
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                //camera.localRotation = m_CameraTargetRot;
            }
            


        }
    }
}