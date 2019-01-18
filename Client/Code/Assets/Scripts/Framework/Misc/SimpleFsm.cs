using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AosBaseFramework
{
	public class SimpleFsm
	{
		public class State
		{			
			public class StateEventArgs
			{
				public State mkCurrentState;
				
				public StateEventArgs(State kState)
				{
					mkCurrentState = kState;
				}
			}
			
			StateEventArgs mkStateEventArgs;
			
			public delegate void StateFunction(State s);
			
			protected string mName;
			protected State mParent;
			protected State mChild;
			protected State mOwner;
			
			protected uint mStateID;
			protected SimpleFsm  mMachine;
	
			protected Dictionary<uint, State> mTransition;
			
			protected StateFunction mFuncEnter;
			protected StateFunction mFuncUpdate;
			protected StateFunction mFuncExit;
			
			public string Name
			{
				get
				{
					if (mName=="")
						return mStateID.ToString();
					else
						return mName;
				}
				set { mName = value; }
			}
			
			public State(uint id, SimpleFsm machine)
			{
				_init(id, machine, null);
			}
			
			public State(uint id, SimpleFsm machine, State owner)
			{
				_init(id, machine, owner);
			}
				
			protected void _init(uint id, SimpleFsm machine, State owner)
			{
				mName = "";
				mStateID = id;
				mMachine = machine;
				mParent = null;
				mChild = null;
				mOwner = owner;
				mFuncEnter = null;
				mFuncUpdate = null;
				mFuncExit = null;
				
				mTransition = new Dictionary<uint, State>();
				
				mkStateEventArgs = new StateEventArgs(this);
				
				machine.RegisterState(this);
			}
			
			public uint GetID()
			{
				return mStateID;
			}
			
			public State GetOwner()
			{
				return mOwner;
			}
			
			public State GetChild()
			{
				return mChild;
			}
			
			public State GetParent()
			{
				return mParent;
			}
			
			public void SetChild(State s)
			{
				s.mParent = this;
				mChild = s;
			}
			
			public void SetParent(State s)
			{
				s.SetChild(this);
			}
			
			public void AddTransition(uint e, State s)
			{
				if (!mTransition.ContainsKey(e))
				{
					mTransition.Add(e, s);
				}
			}
			
			public State React(uint e)
			{
				State newState;
				if (!mTransition.TryGetValue(e, out newState))
					return null;
				
				return newState;
			}
			
			public void SetEnterFunction(StateFunction func)
			{
				mFuncEnter = func;
			}
			
			public void SetUpdateFunction(StateFunction func)
			{
				mFuncUpdate = func;
			}
			
			public void SetExitFunction(StateFunction func)
			{
				mFuncExit = func;
			}
			
			protected void _enter()
			{
				if (mFuncEnter != null)
					mFuncEnter(this);
				
				if (mChild != null)
					mChild.Enter();
			}
			
			public virtual void Enter()
			{
				_enter();
			}
			
			public virtual void Exit()
			{
				_exit();	
			}
			
			protected void _exit()
			{
				if (mFuncExit != null)
					mFuncExit(this);
				
//				if (mChild != null)
//					mChild.Exit();
			}
			
			public virtual void Update()
			{
				_update();
			}
			
			protected void _update()
			{
				if (mFuncUpdate != null)
					mFuncUpdate(this);
				
				if (mChild != null)
					mChild.Update();
			}
		}
	
		Dictionary<uint, List<State>> mkID2StateMap;
		
		void RegisterState(State kState)
		{
			List<State> kStateList;
			if (!mkID2StateMap.TryGetValue(kState.GetID(), out kStateList))
			{
				kStateList = new List<State>();
				mkID2StateMap.Add(kState.GetID(), kStateList);
			}
			kStateList.Add(kState);
		}
		
		protected State mCurState = null;
		protected State mLastState = null;
		protected bool mDebug = false;
		
		public bool bIsDebug
		{
			get { return mDebug; }
			set { mDebug = value; }
		}
		
		public SimpleFsm()
		{
			mCurState = null;
			mkID2StateMap = new Dictionary<uint, List<State>>();
		}
		
		// 此函数在状态机初始化时调用
		// 一旦设置好初始状态后，以后不能随意设置当前状态
		// 状态的切换只与事件相关
		public void SetCurState(State s)
		{
			if (mCurState != null)
			{
				Debug.LogError("[FSM] Error! 除了设置状态机初始状态外，不允许再强制设置状态。");
				return;
			}
			
			mCurState = s;
			s.Enter();

//			for (; s != null; s = s.GetChild())
//			{
//				s.Enter();
//			}
		}
		
		public State GetCurState()
		{
			return mCurState;
		}
		
		public State GetLastState()
		{
			return mLastState;
		}
		
		public void Update()
		{
			if (mCurState == null)
				return;
			
			mCurState.Update();
//			State cur = null;
//			for (cur = mCurState; cur != null; cur = cur.GetChild())
//			{
//				cur.Update();
//			}
		}
		
		// 响应事件，切换状态
		public bool React(uint fsmEvent)
		{
			if (mCurState == null)
				return false;
			mLastState = mCurState;
			State lastNewState = _React_Inner(mCurState, fsmEvent);
			
			return lastNewState != null;
		}
		
		State _React_Inner(State cur, uint fsmEvent)
		{
			State topNewState = null;
			State lastNewState = null;
		
			for (; cur != null; cur = cur.GetChild())
			{
				State parentState = cur.GetParent();
				State newState = cur.React(fsmEvent);
				if (newState != null)
				{
					if (newState.GetOwner() == cur.GetOwner())
					{
						// 状态转换
						if (parentState != null)
							parentState.SetChild(newState);
						else
							mCurState = newState;
						
						if (topNewState == null)
							topNewState = newState;
						
						if (mDebug)
						{
							string curName = cur.Name;
							string newName = newState.Name;
							Debug.Log("[FSM] "+ curName + "-->" + newName + " Event:"+fsmEvent.ToString());
						}
						
						cur.Exit();
						newState.Enter();
						lastNewState = newState;
					}
				}
			}
			
			// 新的状态应该继续响应事件，以确定他的子状态行为
			if (topNewState != null)
			{
				State st = _React_Inner(topNewState.GetChild(), fsmEvent);
				if(st != null) lastNewState = st;
			}
			return lastNewState;
		}
		
		// 该函数用于判断当前状态机是否能够切换成功
		public bool CanReact(uint fsmEvent)
		{
			if (mCurState == null)
				return false;
			
			State cur = mCurState;
		
			for (; cur != null; cur = cur.GetChild())
			{
				State newState = cur.React(fsmEvent);
				if (newState != null)
				{
					if (newState.GetOwner() == cur.GetOwner())
					{
						return true;
					}
				}
			}
			
			return false;
		}	
	}
}


