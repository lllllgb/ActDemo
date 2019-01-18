using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace BT
{
	/// <summary>
	/// BTDatabase is the blackboard in a classic blackboard system. 
	/// (I found the name "blackboard" a bit hard to understand so I call it database ;p)
	/// 
	/// It is the place to store data from local nodes, cross-tree nodes, and even other scripts.

	/// Nodes can read the data inside a database by the use of a string, or an int id of the data.
	/// The latter one is prefered for efficiency's sake.
	/// </summary>
	public class BTDatabase
    {

        private Dictionary<int, object> mID2DataDict = new Dictionary<int, object>();

        public void SetData<T>(int dataID, T data)
        {
            if (mID2DataDict.ContainsKey(dataID))
            {
                mID2DataDict[dataID] = data;
            }
            else
            {
                mID2DataDict.Add(dataID, data);
            }
        }

        public T GetData<T>(int dataID)
        {
            object tmpData = null;

            if (mID2DataDict.TryGetValue(dataID, out tmpData))
            {
                return (T)tmpData;
            }

            return default(T);
        }

        public void RemoveData(int dataID)
        {
            mID2DataDict.Remove(dataID);
        }

        public bool IsExist(int dataID)
        {
            return mID2DataDict.ContainsKey(dataID);
        }

        public void Clear()
        {
            mID2DataDict.Clear();
        }
	}

}