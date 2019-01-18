using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BT {

    /// <summary>
    /// BTSelector is a composite that:
    /// 带权重的串行OR
    /// 如果当前子节点返回Running，那么Selector也返回Running。下一帧继续执行当前这个子节点。
    /// 如果当前子节点返回失败，那么Selector节点本身返回Running，下一帧执行下一个子节点；如果所有子节点都失败了，就返回失败。
    /// 如果当前子节点返回成功，那么Selector返回成功。
    /// Default clear option is to clear the current active child.
    /// </summary>
    public class BTWeightRandomSelector : BTComposite
    {
		
		private int mActiveChildIndex = -1;

        private List<int> mChildNodeWeightList = new List<int>();
        private List<int> mFailedChildIdxList = new List<int>();

        public BTWeightRandomSelector()
        {
        }

        public BTWeightRandomSelector(List<int> childNodeWeight)
        {
            mChildNodeWeightList.AddRange(childNodeWeight);
        }
        
        public override EBTResult Tick()
        {
            if (-1 == mActiveChildIndex)
            {
                int tmpTotalWeight = 0;

                for (int i = 0, max = ChildNodes.Count; i < max; ++i)
                {
                    if (mFailedChildIdxList.Contains(i))
                    {
                        continue;
                    }

                    tmpTotalWeight += (i >= mChildNodeWeightList.Count ? 1 : mChildNodeWeightList[i]);
                }

                int tmpRandonValue = Random.Range(0, tmpTotalWeight);

                for (int i = 0, max = ChildNodes.Count; i < max; ++i)
                {
                    if (mFailedChildIdxList.Contains(i))
                    {
                        continue;
                    }

                    tmpRandonValue -= (i >= mChildNodeWeightList.Count ? 1 : mChildNodeWeightList[i]);

                    if (tmpRandonValue < 0)
                    {
                        mActiveChildIndex = i;
                        break;
                    }
                }
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
                    mActiveChildIndex = -1;

                    if (mFailedChildIdxList.Count >= ChildNodes.Count)
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
		
		public override void Clear ()
        {
			base.Clear();

            mFailedChildIdxList.Clear();
            mChildNodeWeightList.Clear();
        }
	}
}