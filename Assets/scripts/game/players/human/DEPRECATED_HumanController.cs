using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;
using UnityStandardAssets.Characters.FirstPerson;
using Random = UnityEngine.Random;
using Assets.scripts.game.weapons;
using Assets.scripts.game.UserInterface;

namespace Assets.scripts.game.players.human
{

    //okay so basically at first I considered making the player controller and the bot controller basically work in the same way as each other.
    //and yeah I think it went slightly successfully


    public class DEPRECATED_HumanController : deprecated_ModifiedFirstPersonController
    {


        [SerializeField] private HumanLook lookOptions;

        //[SerializeField] private HUDObject hud;


        // Start is called before the first frame update
        override protected void Start()
        {
            base.Start();
            isPlayer = true;
            base.inputs = new HumanInputs();
            //base.playerView = Camera.main;
            m_OriginalCameraPosition = playerView.transform.localPosition;
            //PlayerMouseLook pLook = new PlayerMouseLook();
            lookOptions.Init(transform, playerView.transform, ref inputs);
            lookScript = lookOptions;
            //lookScript = new PlayerMouseLook(transform,playerView.transform,inputs);
            //lookScript.Init(transform, playerView.transform);
            //lookScript.DeclareInputs(inputs);
            //m_FovKick.Setup(Camera.main);

            base.theHUD = GetComponent<HumanHUD>();
            //base.theHUD = hud;

            //theHUD.PromptStart();
            gameControl.HiThisHumanExists(this);

            UpdateLookVector();
        }




        public override void FixedUpdate()
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
            else
            {
                if (inputs.PauseInput)
                {
                    gameControl.StartGame();
                }
            }
        }

    }

}