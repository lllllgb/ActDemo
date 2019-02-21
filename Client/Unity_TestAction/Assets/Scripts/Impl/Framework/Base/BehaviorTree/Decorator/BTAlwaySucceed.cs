using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT
{

    /// <summary>
    /// BTInverter is a decorator node that inverts what its child returns.
    /// </summary>
    public class BTAlwaySucceed : BTDecorator
    {

        public BTAlwaySucceed(BTNode child) : base(child)
        {
        }

        public override EBTResult Tick()
        {
            switch (ChildNode.Tick())
            {
                case EBTResult.Success:
                case EBTResult.Failed:
                    return EBTResult.Success;
            }

            return EBTResult.Running;
        }
    }

}
