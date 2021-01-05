using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.scripts.utilities.fsm;
using Assets.scripts.game.control;
using System.Linq;
using UnityEngine.AI;
using System;
using Random = UnityEngine.Random;
using Assets.scripts.game.weapons;

namespace Assets.scripts.game.players.bot
{
    public class FSMBotContestant : BotContestant, IAmAFSMBot
    {

        BotFSM botFSM;

        


        // Use this for initialization
        override protected void Start()
        {
            base.Start();
            botFSM = new BotFSM(this, gameControl);
        }


        override protected void IndividualUpdate()
        {
            botFSM.ActAndThenReason();

            base.IndividualUpdate();


        }

        /*
        protected override void ActuallyRotateTheView()
        {
            Vector3 targetThis = (overrideDefaultLook) ? lookAtThisInstead : targetPos;

            Vector3 lookTarget = (NavMesh.Raycast(transform.position, targetThis, out NavMeshHit hit, meshAgent.areaMask) ? meshAgent.steeringTarget : targetThis);

            Vector3 newLook = lookTarget - transform.position;


            botLook.LookRotation(
                transform,
                playerView.transform,
                newLook.normalized
            );

        }
        */


        public Transform GetTargetTransform()
        {
            return transformTarget;
        }
    }

    public interface IAmAFSMBot: IAmARobot, ICanSeeFromHere, ICanBeLocated, IAmArmedButIMayNotBeDangerous, IAmARobotWithADestinationToGetTo, IAmARobotWhoCanRememberHearingThings,
        ICanBeGivenATransformToTarget, IHaveADifficulty, IMayOrMayNotHaveJustShot, IAlsoHaveAVelocity
    {
        


        Transform GetTargetTransform();


    }

    public class BotFSM: FSMSystem<BotTransitions, BotStates, IAmAFSMBot, IListenToTheBots>
    {
        readonly IAmAFSMBot bot;

        readonly IListenToTheBots gc;

        public BotFSM(IAmAFSMBot theBot, IListenToTheBots gameControl)
        {
            bot = theBot;
            gc = gameControl;
            MakeTheFSM();
        }

        private void MakeTheFSM()
        {
            BotFSMState patrol = new PatrolState(this);
            BotFSMState investigate = new InvestigateState(this);
            BotFSMState chase = new ChaseState(this);
            BotFSMState flee = new FleeState(this);


            patrol.AddTransition(BotTransitions.startInvestigating, BotStates.investigateState);
            patrol.AddTransition(BotTransitions.startChasing, BotStates.chaseState);

            investigate.AddTransition(BotTransitions.startChasing, BotStates.chaseState);
            investigate.AddTransition(BotTransitions.startPatrolling, BotStates.patrolState);

            chase.AddTransition(BotTransitions.startFleeing, BotStates.fleeState);
            chase.AddTransition(BotTransitions.startInvestigating, BotStates.investigateState);

            flee.AddTransition(BotTransitions.startPatrolling, BotStates.patrolState);


            this.AddState(patrol);
            this.AddState(investigate);
            this.AddState(chase);
            this.AddState(flee);
        }

        public void ActAndThenReason()
        {
            base.ActAndThenReason(bot, gc);
        }
     
    }

    public enum BotStates
    {
        nullState = 0,
        patrolState = 1,
        investigateState = 2,
        chaseState = 3,
        fleeState = 4
    }

    public enum BotTransitions
    {
        nullTransition = 0,
        startPatrolling = 1,
        startInvestigating = 2,
        startChasing = 3,
        startFleeing = 4
    }



    public abstract class BotFSMState : FSMState<BotTransitions, BotStates, IAmAFSMBot, IListenToTheBots>
    {


        protected BotFSMState(BotStates thisState, ICanTransitionToAnotherState<BotTransitions> parent) : base(thisState, parent)
        {

        }

        protected bool HeardSomething(IAmAFSMBot actor)
        {
            if (actor.GetHearInfo(out HearInfo heardThis))
            {


                Vector3 noiseLocation = heardThis.NoiseLocation;
                Debug.Log($"Noise at {noiseLocation}");

                Vector3 actorPos = actor.IAmHere();
                DifficultyStruct actorDifficulty = actor.GetDifficulty();
                double dist = Vector3.Distance(actorPos, noiseLocation);
                float hd = actorDifficulty.hearDistance;
                Debug.Log($"{dist}, {hd}");
                if (dist < hd)
                {
                    double angleScale = Vector3.Angle(noiseLocation - actorPos, noiseLocation) / 180f;
                    double distScale = dist / hd;
                    Debug.Log($"{distScale}");
                    if (NavMesh.Raycast(actorPos, noiseLocation, out NavMeshHit hit, actor.GetAgent().areaMask))
                    {
                        //double hitDistScale = hit.distance / dist;

                        if (Random.value < actorDifficulty.alertness) //&& (distScale < hitDistScale)
                        {
                            StartInvestigations(actor, noiseLocation);
                            return true;
                        }
                    }
                    else
                    {
                        if (distScale < actorDifficulty.alertness || Random.value < actorDifficulty.alertness)
                        {
                            StartInvestigations(actor, noiseLocation);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        protected virtual bool DidISeeSomething(IAmAFSMBot actor, IListenToTheBots controller)
        {
            Vector3 actorPos = actor.IAmHere();
            DifficultyStruct actorDifficulty = actor.GetDifficulty();
            Vector3 actorLook = actor.GetLookVector();
            try
            {
                NavMeshAgent actorAgent = actor.GetAgent();
                Transform target =
                    controller.GetAllOtherContestants(actor)
                    .Select(
                        c => c.HeresMyTransform()
                    ).Where(
                        c1 =>
                        {
                            Vector3 c1Pos = c1.transform.position;
                            return (
                                (!NavMesh.Raycast(actorPos, c1Pos, out NavMeshHit hit, actorAgent.areaMask))
                                &&
                                Vector3.Distance(c1Pos, actorPos) < actorDifficulty.viewDistance
                                &&
                                Vector3.Angle(c1Pos - actorPos, actorLook) < actorDifficulty.botFOV
                            );
                        }
                    ).OrderBy(
                        c2 =>
                        {
                            Vector3 c2Pos = c2.transform.position;
                            return Vector3.Angle(c2Pos - actorPos, actorLook);
                        }
                    ).First();

                Vector3 targetPos = target.position;
                Debug.Log($"Targeting {targetPos}, angle diff is {Vector3.Angle(targetPos - actorPos, actorLook)}");
                if (Vector3.Angle(targetPos - actorPos, actorLook) < actorDifficulty.focusedFOV)
                {
                    Debug.Log("RIGHT THATS IT");
                    StartChase(actor, target);
                }
                else
                {
                    StartInvestigations(actor, target);
                }
                return true;
#pragma warning disable CS0168 // Variable is declared but never used
            }
            catch (Exception e) { }
#pragma warning restore CS0168 // Variable is declared but never used
            return false;

        }

        protected virtual void StartPatrol()
        {
            theFSM.PerformTransition(BotTransitions.startPatrolling);
        }

        protected virtual void StartInvestigations(IAmAFSMBot actor, Vector3 dest)
        {
            actor.SetStationaryTarget(dest);
            theFSM.PerformTransition(BotTransitions.startInvestigating);
        }

        protected virtual void StartInvestigations(IAmAFSMBot actor, Transform target)
        {
            StartInvestigations(actor, target.position);
        }

        protected virtual void StartChase(ICanBeGivenATransformToTarget actor, Transform chaseThis)
        {
            actor.SetTarget(chaseThis, true);
            theFSM.PerformTransition(BotTransitions.startChasing);
        }

        protected virtual void StartFleeing()
        {
            theFSM.PerformTransition(BotTransitions.startFleeing);
        }

        
    }


    public class PatrolState : BotFSMState
    {
        ISet<Destination> patrolDests;
        Destination currentDest;


        public PatrolState(ICanTransitionToAnotherState<BotTransitions> parent) : base(BotStates.patrolState, parent)
        {
            patrolDests = new HashSet<Destination>(GameObject.FindGameObjectsWithTag("BotPatrolNode").Select(g => new Destination(g.transform)).ToArray());

            currentDest = patrolDests.ToList()[Random.Range(0,patrolDests.Count)];
        }


        private Destination GetNewDestination(IAmAFSMBot actor)
        {
            Vector3 currentPos = actor.IAmHere();
            Vector3 currentLook = actor.GetLookVector();
            NavMeshAgent agent = actor.GetAgent();
            ISet<Destination> candidates = new HashSet<Destination>(patrolDests);
            candidates.Remove(currentDest);
            currentDest = candidates.OrderBy(d =>
               {
                   Vector3 pos = d.dest.position;
                   float angle = Vector3.Angle(pos - currentPos, actor.GetLookVector());
                   float dist = Mathf.Clamp01(Vector3.Distance(pos, currentPos) / 20);
                   float obstruction = (NavMesh.Raycast(currentPos, pos, out NavMeshHit hit, agent.areaMask)) ? (hit.distance / dist) : 1;

                   return d.previousVisits * (angle / dist / obstruction / Random.value);

               }
            ).First();

            currentDest.previousVisits++;

            return currentDest;

        }

        class Destination
        {
            public Transform dest;
            public int previousVisits;
            public Destination(Transform d)
            {
                dest = d;
                previousVisits = 1;
            }

            public void Reset()
            {
                previousVisits = 1;
            }

        }

        public override void DoBeforeEntering()
        {
            base.DoBeforeEntering();
            currentDest = patrolDests.ToList()[Random.Range(0, patrolDests.Count)];
        }

        public override void DoBeforeLeaving()
        {
            base.DoBeforeLeaving();
            foreach (Destination d in patrolDests){ d.Reset(); }
        }

        public override void Act(IAmAFSMBot actor, IListenToTheBots controller)
        {
            if (actor.HaveIReachedMyDestination()) 
            {
                actor.SetTarget(GetNewDestination(actor).dest, false);
            }

            //NavMeshAgent actorAgent = actor.GetAgent();
            

            /*
            NavMeshAgent actorAgent = actor.GetAgent();
            if (NavMesh.Raycast(actor.IAmHere(), actorAgent.destination, out NavMeshHit hit, NavMesh.AllAreas)){

                actor.SetLookTarget(actorAgent.steeringTarget);
            }
            else
            {
                actor.SetLookTarget(actorAgent.destination);
            }
            */
        }

        public override void Reason(IAmAFSMBot actor, IListenToTheBots controller)
        {
            
            

            if (!HeardSomething(actor))
            {
                DidISeeSomething(actor, controller);
            }
            
            
        }


    }

    public class InvestigateState : BotFSMState
    {
        bool reset = true;

        private const float MAX_ATTENTION_SPAN = 5f;

        float attentionSpan;

        public InvestigateState(ICanTransitionToAnotherState<BotTransitions> parent) : base(BotStates.investigateState, parent)
        {

        }

        public override void DoBeforeEntering()
        {
            reset = true;
 
        }

        public override void Act(IAmAFSMBot actor, IListenToTheBots controller)
        {
            if (reset)
            {
                attentionSpan = Mathf.Max(actor.GetDifficulty().alertness, Random.value) * MAX_ATTENTION_SPAN;
                reset = false;
            }
            
        }

        public override void Reason(IAmAFSMBot actor, IListenToTheBots controller)
        {
            attentionSpan -= Time.deltaTime;

            if (DidISeeSomething(actor, controller))
            {
            }
            else if (HeardSomething(actor))
            {

            }
            else if (attentionSpan < 0)
            {
                //give up if the bot's lost interest
                StartPatrol();
            }
            else if (actor.HaveIReachedMyDestination() || actor.GetVelocity().magnitude < 0.1f)
            {
                Vector3 randomThing = actor.IAmHere();
                randomThing += (Random.insideUnitSphere * 7.5f);
                if (NavMesh.SamplePosition(randomThing, out NavMeshHit hitInfo, 7.5f, actor.GetAgent().areaMask))
                {
                    randomThing = hitInfo.position;
                }
                actor.SetStationaryTarget(randomThing);
            }
            


        }

        protected override void StartInvestigations(IAmAFSMBot actor, Vector3 dest)
        {
            actor.SetStationaryTarget(dest);
            reset = true;
        }

        protected override void StartInvestigations(IAmAFSMBot actor, Transform target)
        {
            base.StartChase(actor, target);
        }


    }

    public class ChaseState : BotFSMState
    {
        bool reset = true;

        Transform target;

        bool fired;

        bool lostTarget;


        public ChaseState(ICanTransitionToAnotherState<BotTransitions> parent) : base(BotStates.chaseState, parent)
        {

        }


        public override void DoBeforeEntering()
        {
            Debug.Log("どこまでも Chase You");
            reset = true;
            fired = false;
            lostTarget = false;

        }

        public override void DoBeforeLeaving()
        {
            Debug.Log("what happen");
        }

        public override void Act(IAmAFSMBot actor, IListenToTheBots controller)
        {
            
            if (reset)
            {

                target = actor.GetTargetTransform();
                actor.SetTarget(target, true);
                
            }

            Vector3 targetPos = target.position; // target.GetComponentInChildren<Contestant>().GetClosestPointTo(actor.GetViewPos());
            WeaponInfo currentWeapon = actor.WhatDoIHaveEquipped();
            Vector3 actorPos = actor.IAmHere();




            if (NavMesh.Raycast(actor.GetViewPos(), target.position, out NavMeshHit nmhi, actor.GetAgent().areaMask))
            {
                Debug.Log("where is???");
                lostTarget = true;
            }
            else
            {
                Vector3 closest = target.GetComponentInChildren<Contestant>().GetClosestPointTo(actor.GetViewPos());
                Vector3 actorLook = actor.GetLookVector();
                Vector3 lookPos = actor.GetViewPos();

                Vector2 flatLook = new Vector2(actorLook.x, actorLook.z);

                Vector2 flatLookPos = new Vector2(lookPos.x, lookPos.z);

                int thisLayer = actor.GetLayer();
                int arenaLayer = LayerMask.NameToLayer("Arena");
                //RaycastHit hitInfo;
                //Physics.SphereCast(new Ray(lookPos, actorLook), 0.125f, out hitInfo, currentWeapon.distance, target.gameObject.layer);

                //float angleBetween = Vector3.Angle(targetPos - lookPos, lookPos - actorLook);
                //float angleBetween = Vector2.Angle(new Vector2(closest.x, closest.z) - flatLookPos, flatLook);
                float angleBetween = Vector2.Angle(new Vector2(targetPos.x, targetPos.z) - flatLookPos, flatLook);
                Debug.Log($"angle: {angleBetween}");
                //Debug.Log($"angle: {angleBetween}");
                if (angleBetween < 2f)
                {
                    //Physics.SphereCast(new Ray(lookPos, actorLook), 0.25f, out hitInfo, currentWeapon.distance, target.gameObject.layer);
                    //Debug.Log(hitInfo.point);
                    //Vector3 closest = target.GetComponentInChildren<Contestant>().GetClosestPointTo(actor.GetViewPos());
                    //Vector3 projectedLook = lookPos + actorLook;
                    //closest = hitInfo.point;
                    float distBetween = Vector3.Distance(lookPos, closest); //targetPos);
                    //actor.SetLookTarget(closest);
                    //float distBetween = Vector2.Distance(flatLook, new Vector2(targetPos.x, targetPos.y));
                    Debug.Log($"dist: {distBetween}");
                    if ((currentWeapon.dropoffStart / distBetween > 1) ||(1 - (distBetween / currentWeapon.distance) < actor.GetDifficulty().aggression))
                    {
                        Debug.Log("IMMA FIRIN MAH GUN");
                        Shoot(actor);
                    } 
                }
                else if (angleBetween > 90f)
                {
                    Debug.Log("ur gone!!!");
                    lostTarget = true;
                }
                else
                {
                    Debug.Log("turn time");
                }
            }


        }

        private void Shoot(IAmARobot bot)
        {
            bot.Shoot();
            fired = true;
        }

        public override void Reason(IAmAFSMBot actor, IListenToTheBots controller)
        {
            if (fired)
            {
                Debug.Log("run away!!!");
                StartFleeing();
            }
            else if (lostTarget)
            {
                Debug.Log("hmm where did you go???");
                StartInvestigations(actor, target.position);
            }

            
            
        }
    }

    public class FleeState : BotFSMState
    {
        bool justStartedFleeing;

        bool sodThisImShooting;

        Vector3 fleeTarget;

        ISet<Vector3> fleeTargets;

        public FleeState(ICanTransitionToAnotherState<BotTransitions> parent) : base(BotStates.fleeState, parent)
        {
            fleeTargets = new HashSet<Vector3>(GameObject.FindGameObjectsWithTag("BotPatrolNode").Select(g => g.transform.position));
        }
        public override void Act(IAmAFSMBot actor, IListenToTheBots controller)
        {

            if (justStartedFleeing)
            {
                fleeTarget = actor.IAmHere() + (actor.HeresMyTransform().forward * -1.5f);
            }
            if (actor.CanIShoot())
            {
                sodThisImShooting = true;
                return;
            }
            NavMeshAgent actorAgent = actor.GetAgent();
            Vector3 actorPos = actor.IAmHere();
            /*
            DifficultyStruct actorDiff = actor.GetDifficulty();
            try
            {
                Vector3 fleeDisplacement = fleeTarget;
                Vector3 actorNextPos = actorPos + actor.GetVelocity();
                IList<IAlsoHaveAVelocity> potentialThreats = (IList<IAlsoHaveAVelocity>)controller.GetAllOtherContestants(actor)
   
                    .Where(
                        c =>
                        {
                            Vector3 cPos = c.IAmHere();
                            return (
                                (Vector3.Distance(actorPos, cPos) < actorDiff.hearDistance)
                                ||
                                (NavMesh.Raycast(actorPos, cPos, out NavMeshHit hit, actorAgent.areaMask))
                                ||
                                (NavMesh.Raycast(actorNextPos, cPos + c.GetVelocity(), out hit, actorAgent.areaMask))
                            );
                        }
                    ).ToList().OrderBy(
                        c2 => {
                            //orders them by the distance between them and the actor's current pos/the actor's next position/their next pos and the flee target
                            Vector3 c2Pos = c2.IAmHere();
                            Vector3 c2NextPos = c2Pos + c2.GetVelocity();
                            return (Vector3.Distance(actorPos, c2Pos) + Vector3.Distance(actorNextPos, c2NextPos) + Vector3.Distance( fleeTarget,c2NextPos)) / 3;
                            }
                    );


                for(int i = 0; i < potentialThreats.Count; i++)
                {
                    IAlsoHaveAVelocity threat = potentialThreats[i];
                    Vector3 threatPos = threat.IAmHere();
                    Vector3 threatNext = threatPos + threat.GetVelocity();
                    float fleeDist = Vector3.Distance(fleeDisplacement, threatNext);
                    float nextDist = Vector3.Distance(actorNextPos, threatNext);
                    bool fleeTargetFurther = (fleeDist > nextDist);
                    float relativeDanger = (1 - Mathf.Clamp01((fleeTargetFurther ? fleeDist : nextDist) / actorDiff.viewDistance));

                    //gets a vector that's further away from the relative danger
                    Vector3 tempFlee = Vector3.LerpUnclamped((fleeTargetFurther ? fleeDisplacement : actorNextPos), threatNext, 1 + relativeDanger);

                    //works out the difference between tempFlee and the fleeDisplacement
                    tempFlee -= fleeDisplacement;

                    //adds tempFlee to fleeDisplacement, albeit now scaled by the relative danger of this threat compared to the others
                    fleeDisplacement += (tempFlee / (i + 1));
                }
                fleeTarget = fleeDisplacement;

#pragma warning disable CS0168 // Variable is declared but never used
            } catch (Exception e) {
#pragma warning restore CS0168 // Variable is declared but never used
                Vector2 randomVector = Random.insideUnitCircle * 5f;
                fleeTarget += (actor.HeresMyTransform().forward + new Vector3(randomVector.x, 0, randomVector.y));
            }
            NavMeshPath path = new NavMeshPath();
            actorAgent.CalculatePath(fleeTarget, path);
            switch (path.status)
            {
                case NavMeshPathStatus.PathComplete:
                case NavMeshPathStatus.PathPartial:
                    actorAgent.SetPath(path);
                    break;
                case NavMeshPathStatus.PathInvalid:
                    NavMeshHit hit;
                    NavMesh.SamplePosition(actorPos += Random.insideUnitSphere, out hit, 5f, actorAgent.areaMask);
                    fleeTarget = hit.position;
                    break;
            }
            */

            ISet<Vector3> threats = new HashSet<Vector3>(controller.GetAllOtherContestants(actor).Select(a => a.IAmHere()));

            IList<Vector3> candidates = fleeTargets.OrderBy(
                v =>
                {
                    NavMeshPath p = new NavMeshPath();
                    NavMesh.CalculatePath(actorPos, v, actorAgent.areaMask, p);
                    return p.corners.Length;
                }
            ).Take(6).ToList();

            fleeTarget = candidates.OrderByDescending(
                v =>
                {
                    return threats.Sum(
                        t =>
                        {
                            return Vector3.Distance(t, v);
                        }
                        );
                }
                ).First();

            
            actor.SetStationaryTarget(fleeTarget);
        }

        public override void Reason(IAmAFSMBot actor, IListenToTheBots controller)
        {
            if (sodThisImShooting)
            {
                StartPatrol();
            }
        }

        public override void DoBeforeEntering()
        {
            justStartedFleeing = false;
            sodThisImShooting = false;
        }
    }

}