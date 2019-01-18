using UnityEngine;
using System;
using System.Collections;

namespace BT
{

	public class BTActionCallback : BTAction
    {

		private Action<BTDatabase> mCallback;
		private EBTExecuteOpt mExecuteOpt;
		
		public BTActionCallback (Action<BTDatabase> callback, EBTExecuteOpt executeOpt)
        {
			mCallback = callback;
			mExecuteOpt = executeOpt;
		}

		protected override EBTResult Execute ()
        {
            if (0 != (EBTExecuteOpt.OnTick & mExecuteOpt))
            {
				mCallback?.Invoke(Database);
			}

			return EBTResult.Success;
		}

		public override void Clear ()
        {
			base.Clear ();

            if (0 != (EBTExecuteOpt.OnClear & mExecuteOpt))
            {
                mCallback?.Invoke(Database);
            }
		}


	}

}