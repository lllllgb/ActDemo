using UnityEngine;
using System.Collections;

namespace BT
{

	/// <summary>
	/// BTInverter is a decorator node that inverts what its child returns.
	/// </summary>
	public class BTInverter : BTDecorator
    {

		public BTInverter (BTNode child) : base (child)
        {
        }

		public override EBTResult Tick ()
        {
            switch (ChildNode.Tick())
            {
                case EBTResult.Success:
                    return EBTResult.Failed;
                case EBTResult.Failed:
                    return EBTResult.Success;
            }

			return EBTResult.Running;
		}
	}

}