using UnityEngine;
using System.Collections;

namespace BT {

	/// <summary>
	/// BTActionResetData is an action node that reset a data in BTDatabase.
	/// It provides three options:
	/// 	- OnTick: reset the data on tick;
	/// 	- OnClear: reset the data on clear;
	/// 	- Both: reset the data on tick and on clear.
	/// It is useful with the use of different decorators.
	/// </summary>
	public class BTActionResetData<T> : BTAction
    {
		private int mSetDataId;
		private T mRhsData;
		private EBTExecuteOpt mResetOpt;
        
		public BTActionResetData (int setDataID, T rhs, EBTExecuteOpt resetOpt)
        {
            mSetDataId = setDataID;
			mRhsData = rhs;
			mResetOpt = resetOpt;
		}

		protected override EBTResult Execute ()
        {
			if (0 != (EBTExecuteOpt.OnTick & mResetOpt))
            {
				Database.SetData<T>(mSetDataId, mRhsData);
			}

			return EBTResult.Success;
		}

		public override void Clear ()
        {
			base.Clear ();

            if (0 != (EBTExecuteOpt.OnClear & mResetOpt))
            {
				Database.SetData<T>(mSetDataId, mRhsData);
			}
		}
	}

}