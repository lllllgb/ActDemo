using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class LoopScrollPrefabSource 
    {
        public string prefabName;
        public int poolSize = 5;

        private bool inited = false;

        public GameObject prefab;
        public virtual GameObject GetObject()
        {
            if(!inited)
            {
                SG.ResourceManager.Instance.InitPool(prefabName, poolSize, prefab);
                inited = true;
            }
            return SG.ResourceManager.Instance.GetObjectFromPool(prefabName, prefab);
        }

        public virtual void ReturnObject(Transform go)
        {
            go.SendMessage("ScrollCellReturn", SendMessageOptions.DontRequireReceiver);
            go.SendMessage("Dispose");
            SG.ResourceManager.Instance.ReturnObjectToPool(go.gameObject);          
        }

        public GameObject AddComponent()
        {
            if (prefab != null)
            {
                return prefab;
            }
            return null;
        }
    }
}
