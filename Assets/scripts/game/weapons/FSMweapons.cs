using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.scripts.utilities.fsm;
using Assets.scripts.game.players;
using Assets.scripts.game.control;

namespace Assets.scripts.game.weapons
{
	public enum TheWeaponEnum
	{
		NullState = 0, // Use this transition to represent a non-existing transition in your system
		Pistol = 1,
		Shotgun = 2,
		SBG = 3
	}

	public enum WeaponState
	{
		NullWeapon = 0,
		Ready = 1,
		Cooldown = 2,
		FirstShotIsFree = 3
	}

	public enum WeaponTransition
	{
		NullTransition = 0,
		Shoot = 1,
		Recover = 2,
		Reset = 3
	}

	public class FSMWeapons : FSMSystem<TheWeaponEnum, TheWeaponEnum, IMayOrMayNotBeTryingToShoot, GenericWeaponState>
	{

		
		public Pistol pistol;

		public Shotgun shotgun;

		public SomewhatBigGun sbg;

		IMayOrMayNotBeTryingToShoot cont;

		bool needToSwap;

		TheWeaponEnum needToEquipThis;

		public GenericWeapon CurrentWeapon
        {
            get
            {
                switch (CurrentStateID)
                {
					case TheWeaponEnum.Pistol:
						return pistol;
					case TheWeaponEnum.Shotgun:
						return shotgun;
					case TheWeaponEnum.SBG:
						return sbg;
					default:
						Debug.LogError("current weapon machine broke");
						return pistol;
                }
            }
        }

		public bool HaveIJustShot()
        {
			
			return ((GenericWeaponState)CurrentState).HaveIJustShot();

		}

		public WeaponInfo GetWeaponInfo()
		{
			return ((GenericWeaponState)CurrentState).GetWeaponInfo();
		}

		public bool AmIActuallyAbleToShoot()
		{
			return ((GenericWeaponState)CurrentState).AmIActuallyAbleToShoot();
		}


		public FSMWeapons(Pistol p, Shotgun s, SomewhatBigGun bg, IMayOrMayNotBeTryingToShoot c)
        {
			needToSwap = false;
			pistol = p;
			shotgun = s;
			sbg = bg;
			cont = c;
			MakeFSM();
        }

		private void MakeFSM()
		{

			GenericWeaponState pist = new PistolState(pistol, TheWeaponEnum.Pistol, this);
			GenericWeaponState shot = new ShotgunState(shotgun, TheWeaponEnum.Shotgun, this);
			GenericWeaponState sbgS = new SBGState(sbg, TheWeaponEnum.SBG, this);

			pist.AddTransition(TheWeaponEnum.SBG, TheWeaponEnum.SBG);
			pist.AddTransition(TheWeaponEnum.Shotgun, TheWeaponEnum.Shotgun);

			shot.AddTransition(TheWeaponEnum.Pistol, TheWeaponEnum.Pistol);
			shot.AddTransition(TheWeaponEnum.SBG, TheWeaponEnum.SBG);

			sbgS.AddTransition(TheWeaponEnum.Pistol, TheWeaponEnum.Pistol);
			sbgS.AddTransition(TheWeaponEnum.Shotgun, TheWeaponEnum.Shotgun);

			this.AddState(pist);
			this.AddState(shot);
			this.AddState(sbgS);

		}

		public void EquipTransition(TheWeaponEnum t)
		{
			needToEquipThis = t;
			needToSwap = true;
		}

		public void ActAndThenReason()
		{
			Act();
			Reason();
		}

		public void Act()
		{
			((GenericWeaponState)CurrentState).Act(cont);
		}

		public void Reason()
		{
			if (needToSwap)
            {
				needToSwap = false;
				PerformTransition(needToEquipThis);
            }
            else
            {
				((GenericWeaponState)CurrentState).Reason(cont);
			}
			
		}

	}



    public abstract class GenericWeaponState : FSMState<TheWeaponEnum, TheWeaponEnum, IMayOrMayNotBeTryingToShoot, GenericWeaponState>
    {

		public FSMSystem<WeaponTransition, WeaponState, IMayOrMayNotBeTryingToShoot, GenericWeaponState> innerFSM;

		public IAmAWeaponThatCanBeShot weapon;

		private bool didIJustShoot;

		public GenericWeaponState(IAmAWeaponThatCanBeShot theWeapon, TheWeaponEnum weaponState, ICanTransitionToAnotherState<TheWeaponEnum> parent): base(weaponState, parent)
        {
			didIJustShoot = false;
			weapon = theWeapon;
			MakeFSM();
        }

		public WeaponInfo GetWeaponInfo()
		{
			return weapon.GetWeaponInfo();
		}

		public bool AmIActuallyAbleToShoot()
        {
			return (innerFSM.CurrentStateID != WeaponState.Cooldown);
        }

		public void MakeFSM()
        {
			innerFSM = new FSMSystem<WeaponTransition, WeaponState, IMayOrMayNotBeTryingToShoot, GenericWeaponState>();
			InnerWeaponState free = new FirstShotIsFreeState(weapon, WeaponState.FirstShotIsFree, innerFSM);
			InnerWeaponState ready = new ReadyState(weapon, WeaponState.Ready, innerFSM);
			InnerWeaponState wait = new CooldownState(weapon, WeaponState.Cooldown, innerFSM);

			free.AddTransition(WeaponTransition.Shoot, WeaponState.Cooldown);
			free.AddTransition(WeaponTransition.Recover, WeaponState.Ready);
			free.AddTransition(WeaponTransition.Reset, WeaponState.FirstShotIsFree);

			ready.AddTransition(WeaponTransition.Shoot, WeaponState.Cooldown);
			ready.AddTransition(WeaponTransition.Reset, WeaponState.FirstShotIsFree);

			wait.AddTransition(WeaponTransition.Recover, WeaponState.Ready);
			wait.AddTransition(WeaponTransition.Reset, WeaponState.FirstShotIsFree);

			innerFSM.AddState(free);
			innerFSM.AddState(ready);
			innerFSM.AddState(wait);

			
			
        }

		public bool PerformTransition(WeaponTransition wt)
        {
			bool success = innerFSM.PerformTransition(wt);
			if (success && wt.Equals(WeaponTransition.Shoot))
            {
				didIJustShoot = true;
            }
			return success;
        }
		
		public bool HaveIJustShot()
        {
			return didIJustShoot;
        }

		public override void DoBeforeEntering()
        {
			innerFSM.PerformTransition(WeaponTransition.Reset);
        }

		public void Act(IMayOrMayNotBeTryingToShoot actor)
        {
			innerFSM.CurrentState.Act(actor, this);
        }

		public void Reason(IMayOrMayNotBeTryingToShoot actor)
        {
			didIJustShoot = false;
			innerFSM.CurrentState.Reason(actor, this);
        }

        public override void Act(IMayOrMayNotBeTryingToShoot actor, GenericWeaponState fsm)
        {
			innerFSM.CurrentState.Act(actor, fsm);
        }

        public override void Reason(IMayOrMayNotBeTryingToShoot actor, GenericWeaponState fsm)
        {
			didIJustShoot = false;
			innerFSM.CurrentState.Act(actor, fsm);
        }
    }


	public class PistolState: GenericWeaponState
    {
		public PistolState(Pistol p, TheWeaponEnum weaponState, ICanTransitionToAnotherState<TheWeaponEnum> parent ) : base(p, weaponState, parent)
        {
			//first shot at the start is not free.
			innerFSM.PerformTransition(WeaponTransition.Recover);
			
        }
    }

	public class ShotgunState: GenericWeaponState
    {
		public ShotgunState(Shotgun s, TheWeaponEnum weaponState, ICanTransitionToAnotherState<TheWeaponEnum> parent ) : base(s, weaponState, parent)
		{

        }
    }

	public class SBGState: GenericWeaponState
    {
		public SBGState(SomewhatBigGun s, TheWeaponEnum weaponState, ICanTransitionToAnotherState<TheWeaponEnum> parent ) : base(s, weaponState, parent)
		{

        }
    }



    public abstract class InnerWeaponState : FSMState<WeaponTransition, WeaponState, IMayOrMayNotBeTryingToShoot, GenericWeaponState>
    {

		protected IAmAWeaponThatCanBeShot weapon;


		protected InnerWeaponState(IAmAWeaponThatCanBeShot theWeapon, WeaponState theState, ICanTransitionToAnotherState<WeaponTransition> parent): base(theState, parent)
        {
			weapon = theWeapon;
        }

		


	}


	


	public class ReadyState: InnerWeaponState
    {

		public ReadyState(IAmAWeaponThatCanBeShot theWeapon, WeaponState theState, ICanTransitionToAnotherState<WeaponTransition> parent) : base(theWeapon, theState, parent)
		{

		}


		public override void Act(IMayOrMayNotBeTryingToShoot actor,GenericWeaponState gws)
        {
            
        }

		public override void Reason(IMayOrMayNotBeTryingToShoot actor, GenericWeaponState gws)
        {
		
			if (actor.AmITryingToShoot())
            {
				//TODO: weapon.Shoot should use Contestant instead
				weapon.Shoot(actor, false);
				gws.PerformTransition(WeaponTransition.Shoot);
            }

		}
    }

	/// <summary>
	/// Like ReadyState but the first shot is free, remember!
	/// </summary>
	public class FirstShotIsFreeState : ReadyState
	{

		public FirstShotIsFreeState(IAmAWeaponThatCanBeShot theWeapon, WeaponState theState, ICanTransitionToAnotherState<WeaponTransition> parent) : base(theWeapon, theState, parent) { }

		public override void Reason(IMayOrMayNotBeTryingToShoot actor, GenericWeaponState gws)
		{
			if (actor.AmITryingToShoot())
			{
				//TODO: weapon.Shoot should use Contestant instead
				weapon.Shoot(actor, true);
				gws.PerformTransition(WeaponTransition.Shoot);
			}
		}
	}

	public class CooldownState: InnerWeaponState
    {

		bool doneWithCooldown;

		public CooldownState(IAmAWeaponThatCanBeShot theWeapon, WeaponState theState, ICanTransitionToAnotherState<WeaponTransition> parent) : base(theWeapon, theState, parent)
		{
			doneWithCooldown = false;
		}

		

		public override void DoBeforeEntering()
		{
			doneWithCooldown = false;
			weapon.CooldownStuff(DoneWithCooldown);
		}

		public void DoneWithCooldown()
        {
			doneWithCooldown = true;
        }

		
		public override void Act(IMayOrMayNotBeTryingToShoot actor, GenericWeaponState gws)
        {

        }

		public override void Reason(IMayOrMayNotBeTryingToShoot actor, GenericWeaponState gws)
		{
			if (doneWithCooldown)
            {
				//TODO: go back to idle weapon animation
				theFSM.PerformTransition(WeaponTransition.Recover);
            }
		}
    }
}
