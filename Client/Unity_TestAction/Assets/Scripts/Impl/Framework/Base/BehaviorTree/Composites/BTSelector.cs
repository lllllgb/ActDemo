using UnityEngine;
using System.Collections;

namespace BT
{

    /// <summary>
    /// 串行的OR
    /// 如果当前子节点返回Running，那么Selector也返回Running。下一帧继续执行当前这个子节点。
    /// 如果当前子节点返回失败，那么Selector节点本身返回Running，下一帧执行下一个子节点；如果所有子节点都失败了，就返回失败。
    /// 如果当前子节点返回成功，那么Selector返回成功。
    /// 
    /// Default clear option is to clear the current active child.
    /// </summary>
    public class BTSelector : BTComposite
    {

		private int mActiveChildIndex = -1;
        public int ActiveChildIndex { get { return mActiveChildIndex; } }
        
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
                    tmpResult = EBTResult.Success;
                    break;
                case EBTResult.Failed:
                    mActiveChildIndex++;

                    if (mActiveChildIndex >= ChildNodes.Count)
                    {
                        tmpResult = EBTResult.Failed;
                    }
                    break;
            }

            if (EBTResult.Running != tmpResult)
            {
                IsRunning = false;
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