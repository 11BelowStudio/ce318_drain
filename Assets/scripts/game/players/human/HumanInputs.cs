using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;


namespace Assets.scripts.game.players.human
{

    //Player instance of the 'Inputs' abstract class (used for player inputs).

    //disclaimer: CrossPlatformInputManager is from Unity Standard Assets

    public class HumanInputs : Inputs
    {

        protected override Vector2 GetMovement()
        {
            return new Vector2(
                CrossPlatformInputManager.GetAxis("Horizontal"),
                CrossPlatformInputManager.GetAxis("Vertical")
            );   
        }


        protected override Vector2 GetLook()
        {
            return new Vector2(
                CrossPlatformInputManager.GetAxis("Mouse X"),
                CrossPlatformInputManager.GetAxis("Mouse Y")
            );
        }

        

        protected override bool GetShoot()
        {
            return CrossPlatformInputManager.GetButtonDown("Fire1");
        }

        protected override bool GetPause()
        {
            return CrossPlatformInputManager.GetButtonDown("Cancel");
        }

    }


}