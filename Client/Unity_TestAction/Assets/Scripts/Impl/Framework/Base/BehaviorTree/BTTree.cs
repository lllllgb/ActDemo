using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BT
{
	/// <summary>
	/// BTTree is where the behavior tree should be constructed.
	/// </summary>
	public class BTTree
    {
		public string Note;
		private BTNode mRoot;
        private BTDatabase mDatabase;
        
		public BTTree(BTNode root, BTDatabase data)
        {
            mRoot = root;
			mDatabase = data;

			if (mRoot.Name == null)
            {
				mRoot.Name = "Root";
			}

			mRoot.Activate(mDatabase);
		}
        
		public void SetData<T>(int dataID, T data)
        {
			mDatabase.SetData(dataID, data);
		}

		public EBTResult Update()
        {
            if (null != mRoot)
            {
                return mRoot.Tick();
            }

            return EBTResult.Failed;
		}

        public void Clear()
        {
            if (null != mRoot)
            {
                mRoot.Clear();
            }
        }
	}

}
