using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.game.players
{

    /// <summary>
    /// Basically this is a wrapper class that the ModifiedFirstPersonController (and LookScript) read inputs and such from.
    /// there's seperate implementations for the player and the bot.
    /// </summary>

    public abstract class Inputs
    {
        /// <summary>
        /// Obtains the player movement input as a normalized vector2
        /// </summary>
        public Vector2 MovementInput
        {
            get
            {
                Vector2 tempMove = GetMovement();
                if (tempMove.magnitude > 1)
                {
                    return tempMove.normalized;
                }
                return tempMove;

            }//.normalized; }
        }

        protected abstract Vector2 GetMovement();
        virtual internal void SetMovement(Vector2 move) { }


        internal void SetMovement(float x, float y)
        {
            SetMovement(new Vector2(x, y));
        }

        /// <summary>
        /// Obtains the look input rotation input as a Vector2
        /// </summary>
        public Vector2 LookInput
        {
            get { return GetLook(); }
        }

        protected abstract Vector2 GetLook();
        virtual internal void SetLook(Vector2 lookInput) { }

        /// <summary>
        /// Whether or not the player is trying to shoot
        /// </summary>
        public bool ShootInput_deprecated
        {
            get
            {
                bool shoot = GetShoot() && CanShoot;
                if (shoot)
                {
                    CanShoot = false;
                }
                return shoot;
                //return (GetShoot() && canShoot); }
            }
        }

        public bool ShootInput
        {
            set => SetShoot(value);  get => GetShoot();
        }

        private bool canShoot;
        public bool CanShoot { set => canShoot = value; get => canShoot; }


        protected abstract bool GetShoot();
        virtual internal void SetShoot(bool canIntoShoot) { }


        /// <summary>
        /// basically just a pause input
        /// </summary>
        public bool PauseInput
        {
            get { return GetPause(); }
        }

        protected virtual bool GetPause() { return false; }

        
    }

    


    public struct LookInputs
    {
        public float yRotation;
        public float xRotation;

        public LookInputs(float y = 0, float x = 0)
        {
            yRotation = y;
            xRotation = x;
        }
    }
}