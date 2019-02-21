using UnityEngine;
using System.Collections;

namespace BT
{

	/// <summary>
	/// BTActionWait is an action node that waits for a specified time.
	/// It returns running during the wait, and returns success after that.
	/// </summary>
	public class BTActionWait : BTAction
    {
		private float mStartTime;

        private float mWaitTime;


		public BTActionWait (float seconds)
        {
			this.mWaitTime = seconds;
		}

		protected override void Enter ()
        {
			base.Enter ();

			mStartTime = Time.time;
		}

		protected override EBTResult Execute ()
        {
			if (Time.time - mStartTime >= mWaitTime)
            {
				return EBTResult.Success;
			}

			return EBTResult.Running;
		}

        protected override void Exit()
        {
            base.Exit();

            mWaitTime = 0;
        }
    }

}