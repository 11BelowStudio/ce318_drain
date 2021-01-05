using Assets.scripts.game.control;
using System.Collections;
using UnityEngine;

namespace Assets.scripts.game.players.human
{
    public class HumanContestant : Contestant
    {

        [SerializeField] private HumanLook lookOptions;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            isPlayer = true;
            base.inputs = new HumanInputs();
            //base.playerView = Camera.main;
            m_OriginalCameraPosition = playerView.transform.localPosition;
            //PlayerMouseLook pLook = new PlayerMouseLook();
            lookOptions.Init(transform, playerView.transform, ref inputs);
            //lookScript = new PlayerMouseLook(transform,playerView.transform,inputs);
            //lookScript.Init(transform, playerView.transform);
            //lookScript.DeclareInputs(inputs);
            //m_FovKick.Setup(Camera.main);
            base.hud = GetComponent<HumanHUD>();
            //theHUD.PromptStart();

            UpdateLookVector();

            lookOptions.SetCursorLock(true);
        }

        protected override void PauseInput()
        {

            if (inputs.PauseInput)
            {
                Debug.Log("Paused!");
                gameControl.PauseGame();
                Debug.Log("aight should be paused");
            }

        }

        protected override void IndividualFixedUpdate()
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


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                
            }
            else
            {
                m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
            }
            //m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);
            m_CollisionFlags = ActuallyDoTheMoving(m_MoveDir);//* Time.fixedDeltaTime));

            ProgressStepCycle(speed);
            UpdateCameraPosition();
        }


        public override void Pause()
        {
            lookOptions.SetCursorLock(false);
            base.Pause();
        }

        public override void Unpause()
        {
            lookOptions.SetCursorLock(true);
            base.Unpause();
        }

        public override bool AreYouAHuman()
        {
            return true;
        }

        protected override void ActuallyRotateTheView()
        {
            lookOptions.LookRotation(transform, playerView.transform);
        }

        public override void GameOver(bool won = false)
        {
            lookOptions.SetCursorLock(false);
            base.GameOver(won);
        }
    }
}