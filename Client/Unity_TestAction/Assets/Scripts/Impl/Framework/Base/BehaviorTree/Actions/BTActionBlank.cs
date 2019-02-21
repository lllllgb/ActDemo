using UnityEngine;
using System.Collections;

namespace BT
{

	public class BTActionBlank : BTAction
    {

		protected override EBTResult Execute ()
        {
			return EBTResult.Running;
		}
	}

}