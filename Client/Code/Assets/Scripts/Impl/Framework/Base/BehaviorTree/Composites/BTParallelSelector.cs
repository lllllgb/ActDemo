using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BT
{
	/// <summary>
	/// BTParallelSelector is a composite that:
    /// Parallel Selector 从返回值来看是 “||” 逻辑。它是并行的，每一桢执行所有子节点一次~~。
    /// 所有子节点都Running，那么Parallel Selector节点也返回Running。
    /// 有任何一个节点返回失败，那么Parallel Selector 本身返回Running，直到所有子节点都失败了，它才返回失败。
    /// 有任何一个节点返回成功，Parallel Selector 直接返回成功。
	/// </summary>
	public class BTParallelSelector : BTComposite
    {
		private List<int> mCompleteChildIdxList = new List<int>();

		public override EBTResult Tick ()
        {
            EBTResult tmpResult = EBTResult.Running;
            IsRunning = true;

            for (int i = 0; i < ChildNodes.Count; i++)
            {
                if (mCompleteChildIdxList.Contains(i))
                    continue;

                BTNode tmpChildNode = ChildNodes[i];

                switch (tmpChildNode.Tick())
                {
                    case EBTResult.Success:
                        tmpResult = EBTResult.Success;
                        break;
                    case EBTResult.Failed:
                        mCompleteChildIdxList.Add(i);

                        if (mCompleteChildIdxList.Count == ChildNodes.Count)
                        {
                            IsRunning = false;
                            tmpResult = EBTResult.Failed;
                        }
                        break;
                }

                if (EBTResult.Running != tmpResult)
                {
                    IsRunning = false;
                    Clear();
                    break;
                }
            }
            
			return tmpResult;
		}
		
		public override void Clear ()
        {
			base.Clear();

			mCompleteChildIdxList.Clear();
		}
	}
}
