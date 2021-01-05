using System.Collections;
using UnityEngine;
using Assets.scripts.game.players;
using UnityEngine.AI;
using System;

namespace Assets.scripts.game.players.bot
{
    public class BotContestant : Contestant, IAmARobot, IAmARobotWhoCanRememberHearingThings, ICanBeGivenATransformToTarget, IHaveADifficulty
    {

        [SerializeField]
        protected GenericHUD theHud;

        private HearInfo hearing;

        [SerializeField]
        protected BotLookScript botLook;


        protected DifficultyStruct easy = new DifficultyStruct(30f, 10f, 20f, 0.5f, 0.5f);

        protected DifficultyStruct hard = new DifficultyStruct(45f, 15f, 30f, 0.95f, 0.7f);
        
        //default difficulty is medium
        protected DifficultyStruct difficulty = new DifficultyStruct(37.5f, 12.5f, 25f, 0.625f, 0.6f);


        float decelerationRadius;

        float stopDistance;

        float decelerateDistance;

        /// <summary>
        /// Used to set a transform as a target (which may move, and the destination of the bot will be updated appropriately every frame)
        /// </summary>
        protected Transform transformTarget;

        /// <summary>
        /// Used to set a static vector3 as a target (which won't move after being set)
        /// </summary>
        protected Vector3 targetPos;

        protected bool overrideLook;

        protected Vector3 lookAtThisInstead;

        bool targetIsMoving = true;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            base.hud = theHud;
            base.inputs = new BotInputs();

            botLook.Init(this.transform, this.playerView.transform, ref inputs);
            hearing = new HearInfo();
            targetPos = new Vector3();

            

            stopDistance = meshAgent.stoppingDistance;

            decelerationRadius = stopDistance;

            decelerateDistance = stopDistance + decelerationRadius;// + decelerationRadius;

            overrideLook = false;

        }

        public override void SetDifficulty(int level)
        {
            if (level == 0)
            {
                difficulty = easy;
            }
            else if (level == 2)
            {
                difficulty = hard;
            }
            //if difficulty is 1, it'll keep the default medium setting (of 'difficulty')
        }

        protected override void IndividualUpdate()
        {
            if (overrideLook) { overrideLook = false; }
            base.IndividualUpdate();
            
            if (targetIsMoving && transformTarget != null)
            {
                IAmTheContestantWhoGetsShot t = transformTarget.GetComponentInChildren<Contestant>();
                if (t == null)
                {
                    targetPos = transformTarget.position;
                }
                else
                {
                    targetPos = t.GetClosestPointTo(playerView.transform.position);
                }
                meshAgent.SetDestination(transformTarget.position);
            }
            //meshAgent.SetDestination(targetPos);

            
        }

        protected override void IndividualFixedUpdate()
        {
            if ((meshAgent.nextPosition - transform.position).magnitude > meshAgent.radius)
            {
                meshAgent.nextPosition = transform.position;
            }

            float speed = helf.CurrentSpeed;
            float destinationDistance = Vector3.Distance(transform.position, meshAgent.destination);


            if (destinationDistance < decelerateDistance && (!meshAgent.Raycast(meshAgent.desiredVelocity, out NavMeshHit hit)))
            {
                float speedScale = (destinationDistance - meshAgent.stoppingDistance) / decelerationRadius;
                if (speedScale < 0.75f) { speedScale = 0.75f; }
            
                speed *= speedScale;
            }
            


            meshAgent.speed = speed;

            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            Vector3 desiredMove = Vector3.ProjectOnPlane(meshAgent.desiredVelocity, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x * speed;
            m_MoveDir.z = desiredMove.z * speed;

            m_Input = new Vector2(desiredMove.x, desiredMove.z);


            playermodelAnimator.SetFloat(forwardHash, Vector3.Project(desiredMove, transform.forward).magnitude);


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
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
        }

        public bool HaveIReachedMyDestination()
        {
            return (meshAgent.remainingDistance <= meshAgent.stoppingDistance);
        }


        public override void IHeardYouWereAround(Vector3 thatGeneralArea)
        {
            hearing = new HearInfo(thatGeneralArea);
        }

        public bool GetHearInfo(out HearInfo hearInfo)
        {
            hearInfo = hearing;
            if (hearing.heardSomething)
            {
                Debug.Log("I heard something!");
            }
            return hearing.heardSomething;
        }

        public void Shoot()
        {
            inputs.ShootInput = true;
        }

        public override void SetCanMove(bool setTo)
        {
            base.SetCanMove(setTo);
            meshAgent.isStopped = !setTo;
        }


        protected override void ActuallyRotateTheView()
        {

            Vector3 lookTarget = (NavMesh.Raycast(transform.position, targetPos, out NavMeshHit hit, meshAgent.areaMask) ? meshAgent.steeringTarget : targetPos);

            Vector3 newLook = lookTarget - transform.position;

            botLook.LookRotation(
                transform,
                playerView.transform,
                newLook.normalized
            );

   

        }

        public override void SetTarget(Transform newTarget, bool targetIsMoving = true)
        {
            transformTarget = newTarget;
            this.targetIsMoving = targetIsMoving;
            targetPos = newTarget.position;
            meshAgent.SetDestination(targetPos);
        }

        public void SetStationaryTarget(Vector3 stationaryTarget)
        {
            targetPos = stationaryTarget;
            meshAgent.SetDestination(targetPos);
            targetIsMoving = false;
        }

        public DifficultyStruct GetDifficulty()
        {
            return difficulty;
        }

        public virtual void DefaultLookBehaviour() { }


        //TODO: make the bots

        public NavMeshAgent GetAgent()
        {
            return meshAgent;
        }

        protected void OnDrawGizmosEnabled()
        {
            Gizmos.DrawLine(transform.position, meshAgent.destination);
            Gizmos.DrawWireSphere(meshAgent.destination, meshAgent.stoppingDistance);
            Color backup = Gizmos.color;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(meshAgent.destination, stopDistance);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(meshAgent.destination, decelerationRadius);

        }

        public void SetLookTarget(Vector3 lookAtThis)
        {
            Debug.Log("Look at this!");
            lookAtThisInstead = lookAtThis;// - transform.position;
            overrideLook = true;
        }

        public override Vector3 GetLookVector()
        {
            return base.GetLookVector();

        }
        public override Transform GetMyTarget()
        {
            return transformTarget;
        }

        public override void GameOver(bool won = false)
        {
            base.GameOver(won);
            gameObject.SetActive(won);
        }
    }

    public class HearInfo
    {
        private readonly Vector3 noiseLocation;
        public bool heardSomething;

        public Vector3 NoiseLocation
        {
            get { 
                heardSomething = false;
                return noiseLocation;
            }
        }

        public HearInfo(Transform location) : this(location.position){}

        public HearInfo(Vector3 location)
        {
            noiseLocation = location;
            heardSomething = true;
        }

        public HearInfo()
        {
            noiseLocation = new Vector3();
            heardSomething = false;
        }

        

    }

    [Serializable]
    public struct DifficultyStruct
    {
        public float botFOV;
        public float focusedFOV;
        public float viewDistance;
        public float hearDistance;
        public float alertness;
        public float aggression;

        public DifficultyStruct(
            float fov = 37.5f,
            float viewDist = 12.5f,
            float hearDist = 25f,
            float alert = 0.625f,
            float aggro = 0.6f
        )
        {
            botFOV = fov;
            focusedFOV = fov / 2;
            viewDistance = viewDist;
            hearDistance = hearDist;
            alertness = alert;
            aggression = aggro;
        }
    }

    public interface IAmARobot: IAmTheParentOfAllContestants
    {
        void Shoot();

        NavMeshAgent GetAgent();
    }

    public interface IAmARobotWithADestinationToGetTo: IAmARobot
    {
        bool HaveIReachedMyDestination();
    }

    public interface IAmARobotWhoCanRememberHearingThings :  IAmARobotWithADestinationToGetTo
    {
        void SetStationaryTarget(Vector3 stationaryTarget);

        bool GetHearInfo(out HearInfo hearInfo);

        void DefaultLookBehaviour();

        void SetLookTarget(Vector3 targetVector);
    }

    public interface IHaveADifficulty: IAmARobot, ICanHaveMyDifficultySet
    {
        DifficultyStruct GetDifficulty();
    }
}