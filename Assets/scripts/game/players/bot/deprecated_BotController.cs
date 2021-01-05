using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.scripts.game.players.bot
{
    public class deprecated_BotController : deprecated_ModifiedFirstPersonController
    {

        //Vector3 

        [SerializeField] BotLookScript botLookOptions;

        [SerializeField] private NavMeshAgent meshAgent;

        private Vector3 lastKnownPlayerPosition;

        Vector3 routeToNext;

        //private LineRenderer movementLine;

        Vector3 desiredAimVector;

        

        private bool playerSpotted;
        private bool playerStillSpotted;
        private bool chasingPlayer;

        // Start is called before the first frame update
        override protected void Start()
        {
            base.Start();
            base.inputs = new BotInputs();
            //botLookOptions.Init(transform, playerView.transform, ref base.inputs);
            //lookScript = botLookOptions;
            isPlayer = false;
            inputs.SetMovement(new Vector2(0, 0));

            //movementLine = gameObject.AddComponent<LineRenderer>();

            playerView.transform.localPosition = new Vector3(0, 1.7f, 0);

            //movementLine.startWidth = 0.75f;
            //movementLine.endWidth = 0.1f;


            //this is responsible for navmesh navigation and such
            meshAgent = gameObject.GetComponent<NavMeshAgent>();

            meshAgent.isStopped = true;
            meshAgent.updateRotation = false;
            meshAgent.updatePosition = false;
            meshAgent.autoRepath = true;
            meshAgent.autoBraking = false;

            playerSpotted = false;
            playerStillSpotted = false;
            chasingPlayer = false;
            isPlayer = false;

            inputs.CanShoot = false;

            meshAgent.transform.forward = gameObject.transform.forward;

            desiredAimVector = meshAgent.transform.forward;

            base.theHUD = GetComponent<deprecated_BotHudReplacement>();

            gameControl.HiThisBotExists(this);

        }

        
        override protected void Update()
        {

            if (canMove)
            {

                //Moving the meshAgent to be where the characterController is in case they get too far away from each other
                if ((meshAgent.nextPosition - transform.position).magnitude > meshAgent.radius)
                {
                    meshAgent.nextPosition = transform.position;
                }

                debugLine.SetPosition(0, playerView.transform.position);
                debugLine.SetPosition(1, playerView.transform.position + (lookVector * 15));

                RaycastHit hitInfo; //will attempt to detect the player
                if (Physics.Raycast(
                    playerView.transform.position,
                    lookVector,
                    out hitInfo,
                    15f,
                    LayerMask.GetMask("Arena", "PlayerLayer")
                    )
                )
                {
                    //oh hello there player
                    if (hitInfo.collider.gameObject.tag.Equals("Player"))
                    {
                        if (chasingPlayer)
                        {
                            if (inputs.CanShoot)
                            {
                                inputs.SetShoot(true); //if already chasing the player, shoot the player
                            }
                            Debug.Log("bot shooty shoot");
                        }

                        //either way, remember that the player was there
                        SetLastKnownPositionOfPlayer(hitInfo.collider.gameObject.transform.position);
                    }
                    else  //if the player wasn't detected
                    {

                        if (chasingPlayer) //if player is being chased, the player is no longer detected
                        {
                            playerStillSpotted = false;

                        }

                        TryPatrolling();

                    }
                }
                else
                {
                    if (chasingPlayer) //if player is being chased, the player is no longer detected
                    {
                        playerStillSpotted = false;
                    }
                    TryPatrolling();

                }

                base.Update();

            }
            
           
        }

        private void TryPatrolling()
        {
            //if the agent has an unobstructed line of sight to its destination
            if (!(meshAgent.Raycast(meshAgent.destination, out NavMeshHit navHit)))
            {
                //and the remaining distance is below it's stopping distance
                if (meshAgent.remainingDistance <= meshAgent.stoppingDistance)
                {
                    //continue patrolling
                    Patrol();
                }
            }
            
        }

        public override void FixedUpdate()
        {
            //base.FixedUpdate();
            if (canMove)
            {
                float speed = helf.CurrentSpeed;
                //GetMovementInput(out speed);

                //movementLine.SetPosition(0, meshAgent.transform.position);
                //movementLine.SetPosition(1, meshAgent.transform.position + meshAgent.desiredVelocity.normalized * 10);

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

                
                

            }
            
        }
        

        /// <summary>
        /// patrols around the BotPatrolNodes
        /// </summary>
        public void Patrol()
        {
            GameObject[] patrolDests = GameObject.FindGameObjectsWithTag("BotPatrolNode");
            Vector3 thePosition = patrolDests[(int)(Random.value * (patrolDests.Length - 1))].transform.position;
            //desiredAimVector = thePosition;
            meshAgent.SetDestination(
                thePosition
            );
        }

        private void HopefullyLessTerribleBotStuff(out float speed)
        {
 
            speed = helf.CurrentSpeed;
            meshAgent.speed = speed;

            //Vector3 navmeshDesync = (meshAgent.nextPosition - transform.position);

            /*
            //Moving the meshAgent to be where the characterController is in case they get too far away from each other
            if ((meshAgent.nextPosition - transform.position).magnitude > meshAgent.radius)
            {
                meshAgent.nextPosition = transform.position;
            }
            */


            //Vector3 routeToNextDestination = meshAgent.desiredVelocity;

            routeToNext = meshAgent.desiredVelocity;


            //movementLine.SetPosition(0, meshAgent.transform.position);
            //movementLine.SetPosition(1, meshAgent.transform.position + routeToNext);
            //movementLine.SetPosition(1, meshAgent.transform.position + m_CharacterController.velocity.normalized * 10);
            //movementLine.SetPosition(1, meshAgent.steeringTarget);




            //TODO: work out turning and such

            //TODO: maybe give bots dediated turning script, turning based on desired aim vector or something

            inputs.SetMovement(new Vector2(routeToNext.x, routeToNext.z));


            /*
            float angleDiff = -Vector2.SignedAngle(
                        new Vector2(desiredAimVector.x, desiredAimVector.y),
                        new Vector2(playerView.transform.forward.x, playerView.transform.forward.z)
            );

            Quaternion totalTurn = Quaternion.AngleAxis(angleDiff, Vector3.up);

            Quaternion diff = Quaternion.FromToRotation(desiredAimVector, this.playerView.transform.forward);

            //Quaternion partialTurn = Quaternion.FromToRotation(lookVector, desiredAimVector - transform.position);//, 360);
            Quaternion partialTurn = Quaternion.FromToRotation(lookVector, transform.position - this.meshAgent.steeringTarget);//, 360);
            float turnDegrees = -(partialTurn.eulerAngles.y < 180 ? partialTurn.eulerAngles.y % 180 : -partialTurn.eulerAngles.y % 180);

            inputs.SetLook(
                new Vector2(
                    //(diff.eulerAngles.y < 180 ? diff.eulerAngles.y % 180 : -diff.eulerAngles.y % 180);
                    //angleDiff,
                    //(partialTurn.eulerAngles.y < 180 ? partialTurn.eulerAngles.y % 180 : -partialTurn.eulerAngles.y % 180) / 90,
                    turnDegrees/180,
                    //(partialTurn.eulerAngles.y - 180) / 360,
                    0
                )
            );

            */

        }


        
        protected override void RotateView()
        {

            //look at the desiredAimVector if chasing the player. otherwise just look at the steering target.
            //Vector3 lookAt = (((chasingPlayer) ? desiredAimVector : meshAgent.steeringTarget) - this.transform.position);

            //Vector3 lookAt = (meshAgent.steeringTarget - this.transform.position);

            //Vector3 lookAt = meshAgent.desiredVelocity.normalized;

            Vector3 lookAt = ((chasingPlayer) ? (desiredAimVector - this.transform.position) : meshAgent.desiredVelocity).normalized;

            Vector3 currentLook = playerView.transform.forward;

            botLookOptions.LookRotation(
                transform,
                playerView.transform,
                lookAt
            );

            playermodelAnimator.SetFloat(turnHash, Vector3.SignedAngle(currentLook, playerView.transform.forward, Vector3.up));

            playerView.transform.localPosition = new Vector3(0, 1.7f, 0);

            UpdateLookVector();
            //inputs.SetShoot(Random.value < 0.01f);



        }
        

        protected override CollisionFlags ActuallyDoTheMoving(Vector3 moveDirection)
        {

            CollisionFlags flags = base.ActuallyDoTheMoving(moveDirection);


            //https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent-velocity.html
            //controls agent manually
            meshAgent.velocity = m_CharacterController.velocity;


            return flags;
        }


        override protected void UpdateLookVector()
        {
            lookVector = playerView.transform.forward;
        }
        

        override protected void GetMovementInput(out float speed)
        {
            HopefullyLessTerribleBotStuff(out speed);
            m_Input = inputs.MovementInput;
        }

        public override void SetCanMove(bool setTo)
        {
            meshAgent.isStopped = !(setTo);
            meshAgent.updatePosition = !(setTo);
            base.SetCanMove(setTo);
            
        }


        /// <summary>
        /// Call this if the bot 'sees'/hears the player
        /// </summary>
        /// <param name="lastKnownPosition"></param>
        public void SetLastKnownPositionOfPlayer(Vector3 lastKnownPosition)
        {
            if (!playerStillSpotted)
            {
                playerStillSpotted = true;
                chasingPlayer = true;
                StartCoroutine(ChasePlayerCoroutine());
            }
            playerStillSpotted = true;
            
            lastKnownPlayerPosition = lastKnownPosition;
            meshAgent.SetDestination(lastKnownPosition);
            desiredAimVector = lastKnownPosition;
            //meshAgent.CalculatePath(lastKnownPosition, meshAgent.path);
        }

        /// <summary>
        /// coroutine for chasing the player and such.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ChasePlayerCoroutine()
        {
            chasingPlayer = true; //it's now chasing the player
            meshAgent.stoppingDistance = 2;
            meshAgent.autoBraking = true; //will stop at a safe distance from the player
            yield return new WaitForSeconds(5f);
            yield return new WaitWhile(CheckIfPlayerStillSpotted);
            meshAgent.autoBraking = false;

            chasingPlayer = false;
            TryPatrolling();
            meshAgent.stoppingDistance = 1;
            yield break;
        }

        private bool CheckIfChasingPlayer() { return chasingPlayer; }

        private bool CheckIfPlayerStillSpotted() { playerSpotted = false;  bool temp = playerStillSpotted; if (playerStillSpotted) { playerStillSpotted = false;  } return temp; }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            Gizmos.DrawRay(meshAgent.transform.position, meshAgent.velocity.normalized * 10);
        }
    }

}
