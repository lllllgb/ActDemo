using UnityEngine;
using System.Collections;

namespace BT
{

	/// <summary>
	/// BTCompareData is a conditional inheriting from BTConditional.
	/// 
	/// It performs comparison between the provided data with what's found in BTDatabase.
	/// It returns true if they are equal, false otherwise.
	/// </summary>
	public class BTCompareData<T> : BTConditional
    {
		private int mDataID;
		private T mRHS;


		public BTCompareData (int dataID, T rhs)
        {
            mDataID = dataID;
			mRHS = rhs;
		}

		public override bool Check ()
        {
			if (mRHS == null)
            {
				return Database.IsExist(mDataID);
			}

			return Database.GetData<T>(mDataID).Equals(mRHS);
		}
	}

}