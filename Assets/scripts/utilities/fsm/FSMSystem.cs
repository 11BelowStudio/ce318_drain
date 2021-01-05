using Assets.scripts.game.control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.utilities.fsm
{
    public class FSMSystem<TransEnum,StateEnum,Actor, WorldInfo> : ICanTransitionToAnotherState<TransEnum>
		where TransEnum : System.Enum
		where StateEnum : System.Enum
		//where Actor: Component where WorldInfo: Component

	{
		private IDictionary<StateEnum, FSMState<TransEnum, StateEnum, Actor, WorldInfo>> stateMap;


        // The only way one can change the state of the FSM is by performing a transition
        // Don't change the CurrentState directly
        private StateEnum currentStateID;
        public StateEnum CurrentStateID { get { return currentStateID; } }
        private FSMState<TransEnum,StateEnum,Actor, WorldInfo> currentState;
        public FSMState<TransEnum,StateEnum,Actor, WorldInfo> CurrentState { get { return currentState; } }


		public void ActAndThenReason(Actor actor, WorldInfo controller)
        {
			CurrentState.Act(actor, controller);
			CurrentState.Reason(actor, controller);
        }

		public void Act(Actor actor, WorldInfo controller)
		{
			CurrentState.Act(actor, controller);
		}

		public void Reason(Actor actor, WorldInfo controller)
        {
			CurrentState.Reason(actor, controller);
        }



		public FSMSystem()
        {
			stateMap = new Dictionary<StateEnum, FSMState<TransEnum, StateEnum, Actor, WorldInfo>>();
        }


		/// <summary>
		/// This method places new states inside the FSM,
		/// or prints an ERROR message if the state was already inside the List.
		/// First state added is also the initial state.
		/// </summary>
		public bool AddState(FSMState<TransEnum,StateEnum,Actor, WorldInfo> s)
		{
			// Check for Null reference before deleting
			if (s == null || s.ID.Equals(0))
			{
				Debug.LogError("FSM ERROR: Null reference is not allowed");
				return false;
			}

			// First State inserted is also the Initial state,
			//   the state the machine is in when the simulation begins
			if (stateMap.Count == 0)
            {
				stateMap.Add(s.ID, s);
				currentState = s;
				currentStateID = s.ID;
				return true;
            }

			
			// Add the state to the map if it's not inside it already
			if (stateMap.ContainsKey(s.ID))
            {
				Debug.LogError("FSM ERROR: Impossible to add state " + s.ID.ToString() +
								   " because state has already been added");
				return false;
            }
            else
            {
				stateMap.Add(s.ID, s);
				return true;
            }

		}

		/// <summary>
		/// This method delete a state from the FSM List if it exists, 
		///   or prints an ERROR message if the state was not on the List.
		/// </summary>
		public bool DeleteState(StateEnum id)
		{
			// Check for NullState before deleting
			
			if (id.Equals(default(StateEnum)))
			{
				Debug.LogError("FSM ERROR: NullStateID is not allowed for a real state");
				return false;
			}

			//delete that existing state if it exists
			if (stateMap.Remove(id))
            {
				return true;
            }
            else
            {
				Debug.LogError("FSM ERROR: Impossible to delete state " + id.ToString() +
						   ". It was not on the list of states");
				return false;
			}


		}

		/// <summary>
		/// This method tries to change the state the FSM is in based on
		/// the current state and the transition passed. If current state
		///  doesn't have a target state for the transition passed, 
		/// an ERROR message is printed.
		/// </summary>
		public bool PerformTransition(TransEnum trans)
		{
			
			// Check for NullTransition before changing the current state
			if (trans.Equals(default(TransEnum)))
			{
				Debug.LogError("FSM ERROR: NullTransition is not allowed for a real transition");
				return false;
			}
			

			// Check if the currentState has the transition passed as argument
			StateEnum id = currentState.GetOutputState(trans);
			
			if (id.Equals(default(TransEnum)) || !stateMap.ContainsKey(id)) //and make sure it's a valid state
			{
				Debug.LogError("FSM ERROR: State " + currentStateID.ToString() + " does not have a target state " +
							   " for transition " + trans.ToString());
				return false;
			}


			currentStateID = id;
			// Do the post processing of the state before setting the new one
			currentState.DoBeforeLeaving();

			currentState = stateMap[id];
			// Do the post processing of the state before setting the new one
			currentState.DoBeforeEntering();

			return true;


			
		} // PerformTransition()

	}

	public interface ICanTransitionToAnotherState<TransEnum> where TransEnum: System.Enum
    {
		bool PerformTransition(TransEnum t);
    }
}