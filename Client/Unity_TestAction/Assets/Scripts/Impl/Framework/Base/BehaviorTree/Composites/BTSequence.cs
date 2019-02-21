using UnityEngine;
using System.Collections;

namespace BT
{

    /// <summary>
    /// BTSequence is a composite node that ticks children from left to right.
    /// 串行的AND
    /// 如果当前子节点返回Running，那么Sequence也返回Running。下一帧继续执行当前这个子节点。
    /// 如果当前子节点返回失败，那么Sequence节点本身返回失败。
    /// 如果当前子节点返回成功，如果还有下一个子节点，那么Sequence本身返回Running，下一帧会切换到下一个子节点； 如果所有子节点都完毕了，则Sequence节点返回成功，整个节点结束。
    /// 
    /// Default clear option is to clear the current active child.
    /// </summary>
    public class BTSequence : BTComposite
    {
		private int mActiveChildIndex;
		public int ActiveChildIndex {get {return mActiveChildIndex;}}


		public override EBTResult Tick ()
        {
            if (-1 == mActiveChildIndex)
            {
                mActiveChildIndex = 0;
            }

            if (mActiveChildIndex >= ChildNodes.Count)
            {
                return EBTResult.Success;
            }

            BTNode tmpChildNode = ChildNodes[mActiveChildIndex];
            EBTResult tmpResult = EBTResult.Running;
            IsRunning = true;

            switch (tmpChildNode.Tick())
            {
                case EBTResult.Success:
                    mActiveChildIndex++;

                    if (mActiveChildIndex >= ChildNodes.Count)
                    {
                        tmpResult = EBTResult.Success;
                    }
                    break;
                case EBTResult.Failed:
                    IsRunning = false;
                    tmpResult = EBTResult.Failed;
                    break;
            }

            if (EBTResult.Running != tmpResult)
            {
                Clear();
            }

            return tmpResult;
        }

        public override void Clear()
        {
            base.Clear();

            mActiveChildIndex = -1;
        }
    }

}