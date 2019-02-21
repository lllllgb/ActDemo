using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AosBaseFramework
{
    public class GameObjectPool
    {
        private GameObject mRootGo;
        private Dictionary<string, Queue<GameObject>> mName2GoDict = new Dictionary<string, Queue<GameObject>>();

        public GameObjectPool(GameObject rootGo)
        {
            mRootGo = rootGo;
        }

        public GameObject Spawn(string goName)
        {
            GameObject tmpGo = null;

            do
            {
                if (string.IsNullOrEmpty(goName))
                {
                    break;
                }
                
                Queue<GameObject> tmpGoQueue = null;

                if (!mName2GoDict.TryGetValue(goName, out tmpGoQueue))
                {
                    break;
                }

                if (0 == tmpGoQueue.Count)
                {
                    break;
                }

                tmpGo = tmpGoQueue.Dequeue();
                tmpGo.transform.SetParent(null, false);

            } while (false);

            return tmpGo;
        }

        public void Despawn(string goName, GameObject go)
        {
            if (string.IsNullOrEmpty(goName) || null == go)
            {
                return;
            }

            Queue<GameObject> tmpGoQueue = null;

            if (!mName2GoDict.TryGetValue(goName, out tmpGoQueue))
            {
                tmpGoQueue = new Queue<GameObject>();
                mName2GoDict.Add(goName, tmpGoQueue);
            }

            tmpGoQueue.Enqueue(go);
            go.transform.SetParent(mRootGo.transform, false);
        }

        public void Clear(System.Action<string, int> destroyGoHandle)
        {
            foreach (var elem in mName2GoDict)
            {
                foreach (var go in elem.Value)
                {
                    GameObject.Destroy(go);
                }

                destroyGoHandle?.Invoke(elem.Key, elem.Value.Count);
                elem.Value.Clear();
            }

            mName2GoDict.Clear();
            GameObject.Destroy(mRootGo);
        }
    }
}
