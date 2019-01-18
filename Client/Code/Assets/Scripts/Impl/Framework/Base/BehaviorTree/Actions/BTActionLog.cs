using UnityEngine;
using System.Collections;

namespace BT
{

	/// <summary>
	/// BTActionLog is an action node that performs a Unity log.
	/// It returns success after logging once.
	/// </summary>
	public class BTActionLog : BTAction
    {

		private string mLogContent;
		private bool mIsError;

		public BTActionLog (string content, bool isError = false)
        {
			mLogContent = content;
			mIsError = isError;
		}

		protected override EBTResult Execute ()
        {
			if (mIsError)
            {
				Debug.LogError(mLogContent);
			}
			else
            {
				Debug.Log(mLogContent);
			}

			return EBTResult.Success;
		}
	}

}