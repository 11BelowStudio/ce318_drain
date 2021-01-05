using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using UnityStandardAssets.Characters.FirstPerson;
using Random = UnityEngine.Random;
using Assets.scripts.game.weapons;
using Assets.scripts.game.control;
using UnityEngine.AI;

//Modified from the Unity Standard Assets First Person Character FirstPersonController

#pragma warning disable 618, 649
namespace Assets.scripts.game.players
{

    //Edited slightly from 'FirstPersonController.cs' from Unity Standard Assets - Characters - First Person Character
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public abstract class deprecated_ModifiedFirstPersonController : MonoBehaviour
    {

        protected old_GameControl gameControl;

        //public enum weaponEnum { Pistol, Shotgun, SBG };
        private deprecated_WeaponEnum equipped;
        public deprecated_WeaponEnum EquippedWeapon
        {
            get { return equipped; }
        }



        public HealthScript_deprecated helf;

        public float CurrentHealth { get { return helf.currentHealth; } }



        [SerializeField] protected float m_JumpSpeed = 0;
        [SerializeField] protected float m_StickToGroundForce = 10;
        [SerializeField] protected float m_GravityMultiplier = 2;
        protected LookScript lookScript;

        [SerializeField] protected float m_StepInterval;
        [SerializeField] protected AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] protected AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] protected AudioClip m_LandSound;           // the sound played when character touches back on ground.


        [SerializeField] protected AudioClip hurtClip;

        public Inputs inputs;

        public IPlayerHUD theHUD;

        private bool ded;

        protected bool canMove = false;

        private bool canShoot;

        private bool firstShot = false;  //first shot is free. but not with the starting pistol.

        protected GenericWeapon currentWeapon;


        public Component playerView;
        protected bool isPlayer = false;
        protected bool m_Jump;
        protected float m_YRotation;
        protected Vector2 m_Input;
        protected Vector3 m_MoveDir = Vector3.zero;
        protected CharacterController m_CharacterController;
        protected CollisionFlags m_CollisionFlags;
        protected bool m_PreviouslyGrounded;
        protected Vector3 m_OriginalCameraPosition;
        protected float m_StepCycle;
        protected float m_NextStep;
        protected bool m_Jumping;
        protected AudioSource m_AudioSource;
        protected float movementSpeed;

        public bool IsPlayer{ get { return isPlayer; } }

        [SerializeField] protected Animator playermodelAnimator;

        protected int forwardHash = Animator.StringToHash("Forward");

        protected int turnHash = Animator.StringToHash("Turn");



        protected LineRenderer debugLine;

        public Vector3 lookVector; //holds a vector indicating the direction the camera is looking in

        public WeaponModelControl playermodelWeapon;

        // Use this for initialization
        virtual protected void Start()
        {
            gameControl = GameObject.Find("GameControl").GetComponent<old_GameControl>();

            canMove = false;
            ded = false;
            m_CharacterController = GetComponent<CharacterController>();

            playerView = transform.Find("ViewObject").gameObject.transform;
            m_OriginalCameraPosition = playerView.transform.localPosition;


            m_StepCycle = 0f;
            m_NextStep = m_StepCycle * 8f ;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();



            debugLine = playerView.gameObject.AddComponent<LineRenderer>();
            debugLine.SetPosition(0, playerView.transform.position);
            debugLine.SetPosition(1, playerView.transform.position + playerView.transform.forward);
            debugLine.startWidth = 0.5f;
            debugLine.endWidth = 0.1f;
            debugLine.enabled = true;

   
            //movement speed is tied to health
            movementSpeed = helf.CurrentSpeed;

            //you have a pistol at the start
            equipped = deprecated_WeaponEnum.Pistol;

            firstShot = false;
            
        }



        public virtual void SetCanMove(bool setTo) {
            canMove = setTo;
            inputs.CanShoot = setTo;
        }

        // Update is called once per frame
        virtual protected void Update()
        {
            if (canMove)
            {
                RotateView();
                

                if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
                {
                    
                    //PlayLandingSound();
                    m_MoveDir.y = 0f;
                    m_Jumping = false;
                }
                if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
                {
                    m_MoveDir.y = 0f;
                }

                m_PreviouslyGrounded = m_CharacterController.isGrounded;

                
            }
        }


        public void YouAreDedNotBigSoupRice()
        {
            
        }

        protected void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }

        public abstract void FixedUpdate();

        /*
        virtual protected void FixedUpdate()
        {
            if (canMove)
            {

                float speed;
                GetMovementInput(out speed);
                movementSpeed = speed;
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

                // get a normal for the surface that is being touched to move along it
                RaycastHit hitInfo;
                Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                                   m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
                desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

                m_MoveDir.x = desiredMove.x * speed;
                m_MoveDir.z = desiredMove.z * speed;


                if (m_CharacterController.isGrounded)
                {
                    m_MoveDir.y = -m_StickToGroundForce;

                    if (m_Jump)
                    {
                        m_MoveDir.y = m_JumpSpeed;
                        PlayJumpSound();
                        m_Jump = false;
                        m_Jumping = true;
                    }
                }
                else
                {
                    m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
                }
                //m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);
                m_CollisionFlags = ActuallyDoTheMoving(m_MoveDir);//* Time.fixedDeltaTime));

                ProgressStepCycle(speed);
                UpdateCameraPosition(speed);


                
                //lookScript.UpdateCursorLock();
            }
        }
        */

        protected virtual CollisionFlags ActuallyDoTheMoving(Vector3 moveDirection)
        {
            return m_CharacterController.Move(moveDirection * Time.fixedDeltaTime);
        }

        /*
        protected virtual void SetMoveDirectionVector()
        {
            float speed;
            GetMovementInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x * speed;
            m_MoveDir.z = desiredMove.z * speed;
        }
        */
        

        protected void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }


        protected void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                /*
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
                */
                m_StepCycle += (m_CharacterController.velocity.magnitude + speed) *
                             Time.fixedDeltaTime;
                
            }
            

            if (!(m_StepCycle > m_NextStep))
            {
               
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            if (!m_CharacterController.isGrounded)
            {
               
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip, 0.125f);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        protected void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            /*
            if (!m_UseHeadBob)
            {
                return;
            }*/
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                
                //playerView.transform.localPosition = m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude + speed);
                                      //(speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = playerView.transform.localPosition;
                newCameraPosition.y = playerView.transform.localPosition.y;// - m_JumpBob.Offset();
                /*
                viewTransform.localPosition = m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude + speed);
                newCameraPosition = viewTransform.localPosition;
                newCameraPosition.y = viewTransform.localPosition.y - m_JumpBob.Offset();
                */
            }
            else
            {
                //newCameraPosition = viewTransform.localPosition;
                newCameraPosition = playerView.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y;// - m_JumpBob.Offset();
            }
            //viewTransform.localPosition = newCameraPosition;
            playerView.transform.localPosition = newCameraPosition;
            UpdateLookVector();
        }

        

        protected virtual void UpdateLookVector()
        {
            //lookVector = viewTransform.position + viewTransform.forward;
            //debugLine.SetPosition(0, viewTransform.position);
            lookVector = playerView.transform.forward;
            debugLine.SetPosition(0, playerView.transform.position);
            debugLine.SetPosition(1, playerView.transform.position + lookVector);
        }

        


        protected virtual void GetMovementInput(out float speed)
        {
            // Read input
            //float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            //float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            //bool waswalking = m_IsWalking;


            // get current speed from the helf script
            speed = helf.CurrentSpeed;

            //speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            //m_Input = new Vector2(horizontal, vertical);

            //gets the movement input vector2 from the inputs object
            m_Input = inputs.MovementInput;

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            playermodelAnimator.SetFloat(forwardHash, m_Input.y);


            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            /*
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
            */
        }


        protected virtual void RotateView()
        {
            Vector3 currentLook = playerView.transform.forward;
            lookScript.LookRotation(transform, playerView.transform);
            playermodelAnimator.SetFloat(turnHash, Vector3.SignedAngle(currentLook, playerView.transform.forward, Vector3.up));
            UpdateLookVector();
        }


        protected void OnControllerColliderHit(ControllerColliderHit hit)
        {
            //Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            //if (body == null || body.isKinematic)
            //{
               // return;
            //}
            //body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }


        //Plays a noise. Used for weapon sounds + hurt noise
        public void PlayNoise(AudioClip theNoise)
        {
            m_AudioSource.PlayOneShot(theNoise);
        }

        //plays the hurt noise
        public void PlayHurt()
        {
            PlayNoise(hurtClip); //play hurt noise
        }



        public deprecated_WeaponEnum ChangeWeapon()
        {
            switch (equipped)
            {
                case deprecated_WeaponEnum.Pistol:
                    {
                        equipped = (Random.value > 0.5) ? deprecated_WeaponEnum.Shotgun : deprecated_WeaponEnum.SBG;
                        break;
                    }
                case deprecated_WeaponEnum.Shotgun:
                    {
                        equipped = (Random.value > 0.5) ? deprecated_WeaponEnum.Pistol : deprecated_WeaponEnum.SBG;
                        break;
                    }
                case deprecated_WeaponEnum.SBG:
                    {
                        equipped = (Random.value > 0.5) ? deprecated_WeaponEnum.Pistol : deprecated_WeaponEnum.Shotgun;
                        break;
                    }
            }
            firstShot = true; //first shot with a new weapon is free
            canShoot = true; //also no cooldown when equipping a new weapon
            inputs.CanShoot = true;
            //theHUD.SetWeapon(equipped);
            TheWeaponEnum twe = TheWeaponEnum.NullState;
            switch (equipped)
            {
                case deprecated_WeaponEnum.Pistol:
                    {
                        twe = TheWeaponEnum.Pistol;
                        break;
                    }
                case deprecated_WeaponEnum.Shotgun:
                    {
                        twe = TheWeaponEnum.Shotgun;
                        break;
                    }
                case deprecated_WeaponEnum.SBG:
                    {
                        twe = TheWeaponEnum.SBG;
                        break;
                    }
            }
            playermodelWeapon.EquipWeapon(twe);
            theHUD.SetWeapon(twe);
            return equipped;
        }

        /// <summary>
        /// This method is called to do the shot cooldown stuff for the contestant
        /// </summary>
        /// <param name="cooldownLength">length of cooldown to use for the contestant</param>
        /// <param name="weaponNoise">the appropriate weapon noise to play</param>
        public bool DoTheShotCooldownShotNoiseAndSeeIfThisIsTheFirstShot(float cooldownLength, AudioClip weaponNoise)
        {
            StartCoroutine(ShootCooldown(cooldownLength));

            PlayNoise(weaponNoise);
            theHUD.Shoot();
            playermodelWeapon.Shoot();

            bool isFirst = firstShot;
            if (isFirst) { firstShot = false; }

            return isFirst;
        }

        /// <summary>
        /// Coroutine that basically stops 
        /// </summary>
        /// <param name="cooldownLength"></param>
        /// <returns></returns>
        protected IEnumerator ShootCooldown(float cooldownLength)
        {
            inputs.CanShoot = false;
            Debug.Log($"{(isPlayer? "player" : "enemy") } shot cooldown started, pls wait {cooldownLength} secs");
            yield return new WaitForSeconds(cooldownLength);
            inputs.CanShoot = true;
            Debug.Log($"{(isPlayer ? "player" : "enemy") } cooldown over");
            yield break;
        }


        protected virtual void OnDrawGizmos()
        {
            Gizmos.DrawRay(playerView.transform.position, lookVector);
        }

    }
}
