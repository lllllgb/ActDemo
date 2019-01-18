using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BT
{

    /// <summary>
    /// 并行的AND
    /// Parallel 从返回值来看它是 “&&” 逻辑。与Sequence的区别是，在每一桢，它都执行所有子节点一次~~。
    /// 所有子节点都Running，那么Parallel节点也返回Running。
    /// 有任何一个节点返回失败，那么Parallel立刻结束，返回失败。还处于Running的子节点也会终止（从界面上可以看出，正在Running的被假设为失败）。
    /// 有任何一个节点返回成功，那么该子节点下一帧就不会被调用了，但是Parallel本身仍然返回Running，直到所有子节点都返回成功，Parallel才返回成功。
    /// </summary>
    public class BTParallelSequence : BTComposite
    {
		private List<int> mCompleteChildIdxList = new List<int>();
		
		public override EBTResult Tick ()
        {
            if (0 == ChildNodes.Count)
            {
                return EBTResult.Success;
            }

            EBTResult tmpResult = EBTResult.Running;
            IsRunning = true;

            for (int i = 0, max = ChildNodes.Count; i < max; i++)
            {
                if (mCompleteChildIdxList.Contains(i))
                    continue;

                BTNode tmpChildNode = ChildNodes[i];

                switch (tmpChildNode.Tick())
                {
                    case EBTResult.Success:
                        mCompleteChildIdxList.Add(i);

                        if (mCompleteChildIdxList.Count == ChildNodes.Count)
                        {
                            tmpResult = EBTResult.Success;
                        }
                        break;
                    case EBTResult.Failed:
                        tmpResult = EBTResult.Failed;
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
