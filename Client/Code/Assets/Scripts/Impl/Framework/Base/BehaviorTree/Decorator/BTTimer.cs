using UnityEngine;
using System.Collections;

namespace BT {

    /// <summary>
    /// BTTimer is a child node that ticks the child with interval.
    /// During interval, it returns running.
    /// If ticking the child, it returns what the child returns.
    /// </summary>
    public class BTTimer : BTDecorator
    {
        private float mTime;
        private float mInterval;
        
        public BTTimer(float interval, BTNode child) : base(child)
        {
            this.mInterval = interval;
        }

        public override EBTResult Tick()
        {
            mTime += Time.deltaTime;

            if (mTime >= mInterval)
            {
                mTime = 0;
                EBTResult result = ChildNode.Tick();
                return result;
            }
            else
            {
                return EBTResult.Running;
            }
        }

        public override void Clear()
        {
            base.Clear();

            mTime = 0;
        }
    }

}