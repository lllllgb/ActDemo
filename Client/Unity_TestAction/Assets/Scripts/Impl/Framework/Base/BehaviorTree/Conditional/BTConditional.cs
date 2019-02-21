using UnityEngine;
using System.Collections;

namespace BT
{

	/// <summary>
	/// BTConditional is the base class for conditional nodes.
	/// It is usually used to check conditions.
	/// 
	/// Concrete conditional classes inheriting from this class should override the Check method.
	/// </summary>
	public abstract class BTConditional : BTLeaf
    {

		sealed public override EBTResult Tick ()
        {
			if (Check())
            {
				return EBTResult.Success;
			}
			else
            {
				return EBTResult.Failed;
			}
		}

		/// <summary>
		/// This is where the condition check happens.
		/// </summary>
		public virtual bool Check ()
        {
			return false;
		}
	}

}