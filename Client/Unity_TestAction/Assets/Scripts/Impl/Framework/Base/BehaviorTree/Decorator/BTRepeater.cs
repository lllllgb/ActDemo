using UnityEngine;
using System.Collections;

namespace BT
{

	/// <summary>
	/// BTRepeater is a decorator node that keeps the child tick for a number of times, or forever.
	/// </summary>
	public class BTRepeater : BTDecorator
    {
        /// <summary>
        /// How many times can the child tick.
        /// </summary>
        public int mCount;

        /// <summary>
        /// Tick the child forever.
        /// </summary>
        public bool mRepeatForever;

        /// <summary>
        /// Should end the repetition if the child return failure.
        /// </summary>
        public bool mEndOnFailure;

		private int mCurrentCount;


		private BTRepeater (int count, bool repeatForever, bool endOnFailure, BTNode child) : base (child)
        {
			this.mCount = count;
			this.mRepeatForever = repeatForever;
			this.mEndOnFailure = endOnFailure;
		}

		public BTRepeater (int count, bool endOnFailure, BTNode child) : this (count, false, endOnFailure, child)
        {
        }

		public BTRepeater (bool endOnFailure, BTNode child) : this (0, true, endOnFailure, child)
        {
        }

		public override EBTResult Tick ()
        {
            EBTResult tmpResult = EBTResult.Running;
            IsRunning = true;

            do
            {
                if (!mRepeatForever)
                {
                    ++mCurrentCount;
                }

                if (mCurrentCount > mCount)
                {
                    tmpResult = EBTResult.Success;
                    IsRunning = false;
                    break;
                }
                
                EBTResult tmpChildResult = ChildNode.Tick();

                if (mEndOnFailure && EBTResult.Failed == tmpChildResult)
                {
                    tmpResult = EBTResult.Failed;
                    IsRunning = false;
                    break;
                }

            } while (false);

            return tmpResult;
		}

		public override void Clear ()
        {
			base.Clear ();

			mCurrentCount = 0;
		}
	}

}