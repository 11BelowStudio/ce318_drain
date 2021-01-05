using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.scripts.game.control;

namespace Assets.scripts.utilities.fsm
{
	public abstract class FSMState<TransEnum, StateEnum, Actor, WorldInfo>
		where TransEnum : System.Enum
		where StateEnum : System.Enum //where Actor : Component where WorldInfo : Component
	{
		protected Dictionary<TransEnum, StateEnum> map = new Dictionary<TransEnum, StateEnum>();
		protected StateEnum stateID;
		public StateEnum ID { get { return stateID; } }

		protected ICanTransitionToAnotherState<TransEnum> theFSM;

		public FSMState(StateEnum thisStateID, ICanTransitionToAnotherState<TransEnum> parentFSM)
		{
			stateID = thisStateID;
			theFSM = parentFSM;
		}

		public void AddTransition(TransEnum trans, StateEnum id)
		{
			
			
			// Check if anyone of the args is invalid
			if (trans.Equals(default(TransEnum)))
			{
				Debug.LogError("FSMState ERROR: NullTransition is not allowed for a real transition");
				return;
			}

			if (id.Equals(default(TransEnum)))
			{
				Debug.LogError("FSMState ERROR: NullStateID is not allowed for a real ID");
				return;
			}
			

			// Since this is a Deterministic FSM,
			//   check if the current transition was already inside the map
			if (map.ContainsKey(trans))
			{
				Debug.LogError("FSMState ERROR: State " + stateID.ToString() + " already has transition " + trans.ToString() +
							   "Impossible to assign to another state");
				return;
			}

			map.Add(trans, id);
		}

		/// <summary>
		/// This method deletes a pair transition-state from this state's map.
		/// If the transition was not inside the state's map, an ERROR message is printed.
		/// </summary>
		public void DeleteTransition(TransEnum trans)
		{
			
			// Check for NullTransition
			if (trans.Equals(default(TransEnum)))
			{
				Debug.LogError("FSMState ERROR: NullTransition is not allowed");
				return;
			}
			

			// Check if the pair is inside the map before deleting
			if (map.ContainsKey(trans))
			{
				map.Remove(trans);
				return;
			}
			Debug.LogError("FSMState ERROR: Transition " + trans.ToString() + " passed to " + stateID.ToString() +
						   " was not on the state's transition list");
		}

		/// <summary>
		/// This method returns the new state the FSM should be if
		///    this state receives a transition and 
		/// </summary>
		public StateEnum GetOutputState(TransEnum trans)
		{
			// Check if the map has this transition
			if (map.ContainsKey(trans))
			{
				return map[trans];
			}
			return default(StateEnum);
		}

		/// <summary>
		/// This method is used to set up the State condition before entering it.
		/// It is called automatically by the FSMSystem class before assigning it
		/// to the current state.
		/// </summary>
		public virtual void DoBeforeEntering() { }

		/// <summary>
		/// This method is used to make anything necessary, as reseting variables
		/// before the FSMSystem changes to another one. It is called automatically
		/// by the FSMSystem before changing to a new state.
		/// </summary>
		public virtual void DoBeforeLeaving() { }

		/// <summary>
		/// This method decides if the state should transition to another on its list
		/// Actor refers to the object this class controls. Controller refers to the game controller.
		/// </summary>
		public abstract void Reason(Actor actor, WorldInfo controller);

		/// <summary>
		/// This method controls the behavior of the NPC in the game World.
		/// Every action, movement or communication the NPC does should be placed here
		/// Actor refers to the object this class controls. Controller refers to the game controller.
		/// </summary>
		public abstract void Act(Actor actor, WorldInfo controller);

	} // class FSMState
}