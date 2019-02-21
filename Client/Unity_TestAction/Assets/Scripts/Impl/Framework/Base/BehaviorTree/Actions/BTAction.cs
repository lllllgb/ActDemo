using UnityEngine;
using System.Collections;

namespace BT
{

	/// <summary>
	/// BTAction is the base class for action nodes.
	/// It is where the actual gameplay logic happens.
	/// </summary>
	public class BTAction : BTLeaf
    {
        private enum EBTActionStatus
        {
            Ready,
            Running,
        }

        private EBTActionStatus mStatus = EBTActionStatus.Ready;
        
		sealed public override EBTResult Tick ()
        {
			EBTResult tmpTickResult = EBTResult.Success;

			if (mStatus == EBTActionStatus.Ready)
            {
				Enter();
				mStatus = EBTActionStatus.Running;
			}
			if (mStatus == EBTActionStatus.Running)
            {
				tmpTickResult = Execute();
                IsRunning = true;

                if (tmpTickResult != EBTResult.Running)
                {
					Exit();
					mStatus = EBTActionStatus.Ready;
					IsRunning = false;
				}
			}

			return tmpTickResult;
		}

		public override void Clear ()
        {
			base.Clear();

			if (mStatus != EBTActionStatus.Ready)// not cleared yet
            {	
				Exit();
				mStatus = EBTActionStatus.Ready;
			}
		}

		/// <summary>
		/// Called when the action node is about to execute.
		/// </summary>
		protected virtual void Enter () {}

        /// <summary>
        /// Called every frame if the action node is active.
        /// </summary>
        protected virtual EBTResult Execute() { return EBTResult.Success; }

		/// <summary>
		/// Called when the action node finishes.
		/// </summary>
		protected virtual void Exit () {}


	}
}