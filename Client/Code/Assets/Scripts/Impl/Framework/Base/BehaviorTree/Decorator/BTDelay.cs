using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{
    public class BTDelay : BTDecorator
    {
        private float mTime;
        private float mDelay;
        
        public BTDelay(float delay, BTNode child) : base(child)
        {
            mDelay = delay;
        }

        public override EBTResult Tick()
        {
            mTime += Time.deltaTime;
            EBTResult tmpResult = EBTResult.Running;

            if (mTime >= mDelay)
            {
                tmpResult = ChildNode.Tick();
            }

            return tmpResult;
        }

        public override void Clear()
        {
            base.Clear();

            mTime = 0;
        }
    }
}