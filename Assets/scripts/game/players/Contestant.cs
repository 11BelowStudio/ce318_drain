using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Assets.scripts.game.control;
using Assets.scripts.game.weapons;
using System;
using Random = UnityEngine.Random;

namespace Assets.scripts.game.players {

    //Modified from the Unity Standard Assets First Person Character FirstPersonController


    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(NavMeshAgent))]

    public class Contestant : MonoBehaviour,
        IMayOrMayNotBeTryingToShoot, IAmAContestantWhoGetsPausedAndUnpaused, ICanGetStartedAndStopped, IGetAskedIfImDead, IMayOrMayNotHaveJustShot,
        ICanHearYou, ICanPickUpAWeapon, IHaveATransform, ICanBeGivenATransformToTarget, IAlsoHaveAVelocity
    {
       


        protected GameControl gameControl;

        public HealthScript helf;

        protected float movementSpeed;

        protected bool canMove;

        protected bool ded;

        protected NavMeshAgent meshAgent;

        [SerializeField] protected float m_JumpSpeed = 0;
        [SerializeField] protected float m_StickToGroundForce = 10;
        [SerializeField] protected float m_GravityMultiplier = 2;

        [SerializeField] protected float m_StepInterval;
        [SerializeField] protected AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] protected AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] protected AudioClip m_LandSound;           // the sound played when character touches back on ground.

        [SerializeField] protected AudioClip hurtClip;



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

        [SerializeField]
        protected GenericHUD hud;

        public Vector3 lookVector; //holds a vector indicating the direction the camera is looking in

        public FSMWeapons weaponFSM;

        protected Inputs inputs;

        public WeaponModelControl playermodelWeapon;

        [SerializeField] protected Animator playermodelAnimator;

        protected int forwardHash = Animator.StringToHash("Forward");

        protected int turnHash = Animator.StringToHash("Turn");

        bool justToggledPause;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            gameControl = GameObject.Find("GameControl").GetComponent<GameControl>();

            canMove = false;
            ded = false;
            m_CharacterController = GetComponent<CharacterController>();

            helf = GetComponent<HealthScript>();

            playerView = transform.Find("ViewObject").gameObject.transform;
            m_OriginalCameraPosition = playerView.transform.localPosition;


            m_StepCycle = 0f;
            m_NextStep = m_StepCycle * 4;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();

            weaponFSM = gameControl.GetFSMWeapons(this);

            meshAgent = gameObject.GetComponent<NavMeshAgent>();

            meshAgent.isStopped = true;
            meshAgent.updateRotation = false;
            meshAgent.updatePosition = false;
            meshAgent.autoRepath = true;
            meshAgent.autoBraking = false;

            justToggledPause = false;

            
        }

        //TODO: STILL NEED TO FINISH OFF CONTESTANT!

        //TODO: ALSO NEED TO REFACTOR GAMECONTROL TO ACCOMMODATE CONTESTANT


        // Update is called once per frame
        protected void Update()
        {
            if (canMove)
            {

                IndividualUpdate();
                RotateView();

                weaponFSM.ActAndThenReason();

                if (weaponFSM.HaveIJustShot())
                {
                    Debug.Log("shootin");
                }

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
            PauseInput();
        }

        protected virtual void PauseInput() { }

        protected virtual void IndividualUpdate() { }


        protected void FixedUpdate()
        {
            if (canMove)
            {

                IndividualFixedUpdate();

                
            }
        }

        protected virtual void IndividualFixedUpdate() { }

        protected virtual CollisionFlags ActuallyDoTheMoving(Vector3 moveDirection)
        {
            CollisionFlags c = m_CharacterController.Move(moveDirection * Time.fixedDeltaTime);
            meshAgent.velocity = m_CharacterController.velocity;
            return c;
        }

        public IControlHealth ControlGetHealth()
        {
            return helf;
        }

        public void WeaponChangeHealth(float healthChange) { WeaponChangeHealth(healthChange, false); }
        public void WeaponChangeHealth(float healthChange, bool isShooter)
        {
            helf.ChangeHealth(healthChange, isShooter);
            hud.SetHealth(helf.currentHealth);
            if (!isShooter)
            {
                PlayNoise(hurtClip);
            }
        }

        protected void RotateView()
        {
            Vector3 currentLook = playerView.transform.forward;
            ActuallyRotateTheView();
            playermodelAnimator.SetFloat(turnHash, Vector3.SignedAngle(currentLook, playerView.transform.forward, Vector3.up));
            UpdateLookVector();
        }

        protected virtual void ActuallyRotateTheView() { }

        public Vector3 GetClosestPointTo(Vector3 other)
        {
            return m_CharacterController.ClosestPoint(other);
        }

        public List<Collider> GetHitboxes()
        {
            return new List<Collider>(GetComponentsInChildren<Collider>());
        }

        public bool AmITryingToShoot()
        {
            if (justToggledPause)
            {
                Debug.Log("nope just a pause");
                justToggledPause = false;
                return false;
            }
            return inputs.ShootInput;
        }

        public bool HaveIJustShot()
        {
            return weaponFSM.HaveIJustShot();
        }

        public GenericHUD GetHUD()
        {
            return hud;
        }

        public void PlayNoise(AudioClip noise)
        {
            m_AudioSource.PlayOneShot(noise);
        }

        public void ActuallyShoot(AudioClip fireNoise)
        {
            playermodelWeapon.Shoot();
            hud.Shoot();
            PlayNoise(fireNoise);
        }

        public void ChangeWeapon()
        {
            TheWeaponEnum next;
            switch (weaponFSM.CurrentStateID)
            {
                case TheWeaponEnum.Pistol:
                    next = ((Random.value > 0.5) ? TheWeaponEnum.Shotgun : TheWeaponEnum.SBG);
                    break;
                case TheWeaponEnum.SBG:
                    next = ((Random.value > 0.5) ? TheWeaponEnum.Shotgun : TheWeaponEnum.Pistol);
                    break;
                case TheWeaponEnum.Shotgun:
                    next = ((Random.value > 0.5) ? TheWeaponEnum.Pistol : TheWeaponEnum.SBG);
                    break;
                default:
                    next = TheWeaponEnum.Pistol;
                    break;
            }
            weaponFSM.EquipTransition(next);
            hud.SetWeapon(next);
            playermodelWeapon.EquipWeapon(next);
        }

        protected void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
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
            int n = UnityEngine.Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip, 0.125f);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }

        protected void UpdateCameraPosition()
        {
            Vector3 newCameraPosition;

            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {

                newCameraPosition = playerView.transform.localPosition;
                newCameraPosition.y = playerView.transform.localPosition.y;
            }
            else
            {
                newCameraPosition = playerView.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y;
            }
            playerView.transform.localPosition = newCameraPosition;
            UpdateLookVector();
        }

        protected virtual void UpdateLookVector()
        {
            //lookVector = viewTransform.position + viewTransform.forward;
            //debugLine.SetPosition(0, viewTransform.position);
            lookVector = playerView.transform.forward;
        }

        private void OnDrawGizmos()
        {

            Gizmos.DrawRay(playerView.transform.position, lookVector);
        }

        public virtual void SetCanMove(bool setTo)
        {
            canMove = setTo;
            inputs.CanShoot = setTo;
            if (!setTo)
            {
                playermodelAnimator.SetFloat(forwardHash, 0);
                playermodelAnimator.SetFloat(turnHash, 0);
            }
        }

        protected virtual void GetMovementInput(out float speed)
        {

            // get current speed from the helf script
            speed = helf.CurrentSpeed;


            //gets the movement input vector2 from the inputs object
            m_Input = inputs.MovementInput;

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            playermodelAnimator.SetFloat(forwardHash, m_Input.y);
        }

        public Vector3 GetViewPos()
        {
            return playerView.transform.position;
        }

        public virtual Vector3 GetLookVector()
        {
            return playerView.transform.forward;
        }

        public virtual void Pause()
        {
            
            SetCanMove(false);
            justToggledPause = true;
            hud.Pause();
        }

        public virtual void Unpause()
        {
            SetCanMove(true);
            justToggledPause = true;
            hud.Unpause();
        }

        public void WillYouStartTheDrainsPlease()
        {
            hud.GameHasStarted();
            helf.WillYouStartTheDrainsPlease();
        }

        public void WillYouStopTheDrainsPlease()
        {
            helf.WillYouStopTheDrainsPlease();
        }

        public bool AreYouDeadYet()
        {
            return helf.ded;
        }

        public Vector3 IAmHere()
        {
            return this.transform.position;
        }

        public virtual bool AreYouAHuman()
        {
            return false;
        }

        public virtual void GameOver(bool won = false)
        {
            hud.GameOver(won);
            WillYouStopTheDrainsPlease();
            SetCanMove(false);
        }

        public virtual void IHeardYouWereAround(Vector3 thatGeneralArea) { }

        public void SetDisplayHealth(float health)
        {
            hud.SetHealth(health);
        }

        public bool CanIShoot()
        {
            return weaponFSM.AmIActuallyAbleToShoot();
        }

        public WeaponInfo WhatDoIHaveEquipped()
        {
            return weaponFSM.GetWeaponInfo();
        }

        public Transform HeresMyTransform()
        {
            return this.transform;
        }

        public virtual void SetTarget(Transform newTarget, bool targetIsMoving = true)
        {

        }
        public virtual void SetDifficulty(int level) { }

        public Quaternion GetLookRotation()
        {
            return Quaternion.LookRotation(this.transform.forward, playerView.transform.up);
        }

        public Vector3 GetVelocity()
        {
            return m_CharacterController.velocity;
        }

        public int GetLayer()
        {
            return gameObject.layer;
        }

        public virtual Transform GetMyTarget()
        {
            throw new NotImplementedException();
        }
    }

    public interface IAmTheParentOfAllContestants { }

    public interface ICanBeLocated : IAmTheParentOfAllContestants
    {
        Vector3 IAmHere();
    }

    public interface ICanHearYou : ICanBeLocated
    {
        void IHeardYouWereAround(Vector3 thatGeneralArea);
    }

    public interface IHaveATransform: ICanBeLocated, ICanSeeFromHere
    {
        Transform HeresMyTransform();
    }

    public interface IAlsoHaveAVelocity: IHaveATransform
    {
        Vector3 GetVelocity();
    }

    public interface ICanBeGivenATransformToTarget: ICanHearYou, IHaveATransform
    {
        void SetTarget(Transform newTarget, bool targetIsMoving = true);
    }

    public interface IAmTheContestantWhoGetsShot : IAmTheParentOfAllContestants
    {
        Vector3 GetClosestPointTo(Vector3 other);

        void WeaponChangeHealth(float healthChange);
    }

    public interface ICanSeeFromHere: IAmTheParentOfAllContestants
    {
        Vector3 GetViewPos();

        Vector3 GetLookVector();

        Quaternion GetLookRotation();

        int GetLayer();
    }

    public interface IAmTheContestantWhoHasShot: IAmTheContestantWhoGetsShot, ICanSeeFromHere, IGetAskedIfImARobot
    {
        
        void ActuallyShoot(AudioClip fireNoise);

        void WeaponChangeHealth(float healthChange, bool isShooter = false);

        Transform GetMyTarget();
    }

    public interface IMayOrMayNotHaveJustShot: ICanBeLocated
    {
        bool HaveIJustShot();
    }

    public interface IMayOrMayNotBeTryingToShoot: IAmTheContestantWhoHasShot
    {
        bool AmITryingToShoot();
    }


    public interface IGetPausedAndIGetUnpaused
    {
        void Pause();
        void Unpause();
    }

    public interface IAmAContestantWhoGetsPausedAndUnpaused : IGetPausedAndIGetUnpaused, IAmTheParentOfAllContestants { }


    public interface ICanGetStartedAndStopped: IAmTheParentOfAllContestants
    {
        void SetCanMove(bool canMove);

        void WillYouStartTheDrainsPlease();

        void WillYouStopTheDrainsPlease();
    }

    public interface IGetAskedIfImARobot: IAmTheParentOfAllContestants
    {
        bool AreYouAHuman();
    }

    public interface IGetAskedIfImDead: IGetAskedIfImARobot
    {
        bool AreYouDeadYet();

        void GameOver(bool won = false);
    }

    public interface ICanPickUpAWeapon : IAmTheParentOfAllContestants
    {
        void ChangeWeapon();
    }

    public interface IListenToMyHealthScript: IAmTheParentOfAllContestants
    {
        void SetDisplayHealth(float health);
    }

    public interface IAmArmedButIMayNotBeDangerous: IAmTheParentOfAllContestants
    {
        bool CanIShoot();

        WeaponInfo WhatDoIHaveEquipped();
    }

    public interface ICanHaveMyDifficultySet: IAmTheParentOfAllContestants
    {
        void SetDifficulty(int level);
    }
}
