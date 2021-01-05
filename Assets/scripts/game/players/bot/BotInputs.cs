using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.game.players.bot
{

    /// <summary>
    /// yeah basically this is what the bots will use to walk around and shoot and such
    /// </summary>
    public class BotInputs : Inputs
    {

        private Vector2 cantStickAroundAndChatGottaMove = new Vector2(0,0);

        private Vector2 whatIsThisWeirdThingImLookingAt = new Vector2();

        private bool muricaTime = false;

        

        protected override Vector2 GetMovement()
        {
            return cantStickAroundAndChatGottaMove;
        }

        internal override void SetMovement(Vector2 move)
        {
            cantStickAroundAndChatGottaMove = move.normalized;
        }

        protected override Vector2 GetLook()
        {
            return whatIsThisWeirdThingImLookingAt;
        }

        internal override void SetLook(Vector2 look)
        {
            whatIsThisWeirdThingImLookingAt = look;
            //whatIsThisWeirdThingImLookingAt = (look/Time.deltaTime);
            //whatIsThisWeirdThingImLookingAt.x = Mathf.Clamp(whatIsThisWeirdThingImLookingAt.x, -1f, 1f);
            //whatIsThisWeirdThingImLookingAt.y = Mathf.Clamp(whatIsThisWeirdThingImLookingAt.y, -1f, 1f);
        }


        protected override bool GetShoot()
        {
            bool temp = muricaTime;
            muricaTime = false;
            if (temp)
            {
                Debug.Log("murica");
            }
            return temp;
        }


        internal override void SetShoot(bool canIntoShoot) {
            muricaTime = canIntoShoot;
        }
    }

}
