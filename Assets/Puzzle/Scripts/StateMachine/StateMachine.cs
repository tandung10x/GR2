using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T, U>
	where T : IComparable
	where U : IComparable
{

	Dictionary<T, StateConfig> statesConfig = new Dictionary<T, StateConfig>();

	T currentState;

	public T CurrentState
	{
		get { return currentState; }
	}

	public StateMachine(T state)
	{
		currentState = state;
	}

	public StateConfig Configure(T state)
	{
		if (!statesConfig.ContainsKey(state))
			statesConfig.Add(state, new StateConfig());

		return statesConfig[state];
	}

	public void SetTrigger(U trigger)
	{
		T nextState = statesConfig[currentState].SetTrigger(trigger);

		if (nextState.Equals(currentState) || !statesConfig.ContainsKey(currentState)) return;
		statesConfig[currentState].Exit();
		currentState = nextState;
		statesConfig[nextState].Enter();
	}

	public class StateConfig
	{

		public event Action onEnter;
		public event Action onExit;

		List<Transition<U, T>> transitions = new List<Transition<U, T>>();

		public StateConfig AddTransition(U trigger, T state)
		{
			transitions.Add(new Transition<U, T>(trigger, state));
			return this;
		}

		public T SetTrigger(U trigger)
		{
			foreach (Transition<U, T> tran in transitions)
			{
				if (tran.trigger.Equals(trigger))
					return tran.state;
			}
			throw new Exception("No " + trigger + " trigger!");
		}

		public void Enter()
		{
			if (onEnter != null)
				onEnter.Invoke();
		}

		public void Exit()
		{
			if (onExit != null)
				onExit.Invoke();
		}
	}
}

class Transition<T, U>
	where T : IComparable
	where U : IComparable
{

	public T trigger;
	public U state;

	public Transition(T trigger, U state)
	{
		this.trigger = trigger;
		this.state = state;
	}
}
