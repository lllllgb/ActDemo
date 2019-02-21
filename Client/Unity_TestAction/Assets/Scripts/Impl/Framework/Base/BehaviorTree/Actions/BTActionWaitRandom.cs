using UnityEngine;
using System.Collections;

namespace BT
{

	/// <summary>
	/// BTActionWaitRandom is almost the same as BTActionWait, 
	/// except every time it is about to execute, the waiting time is randomly generated in a specified range.
	/// </summary>
	public class BTActionWaitRandom : BTAction
    {
		private float mStartTime;
		private float mWaitTime;

        private float mMin;
        private float mMax;
        
		public BTActionWaitRandom (float min, float max)
        {
			this.mMin = min;
			this.mMax = max;
		}

		protected override void Enter ()
        {
			base.Enter ();

			mStartTime = Time.time;
			mWaitTime = Random.Range(mMin, mMax);
		}

		protected override EBTResult Execute ()
        {
			if (Time.time - mStartTime >= mWaitTime)
            {
				return EBTResult.Success;
			}

			return EBTResult.Running;
		}
	}

}