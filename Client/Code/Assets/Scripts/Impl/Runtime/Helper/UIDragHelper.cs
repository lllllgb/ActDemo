using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public class UIDragHelper : Singleton<UIDragHelper>
    {

        public interface IDragItem
        {
            bool isDragging { set; get; }
        }

        public void BeginDrag(IDragItem item, GameObject go)
        {
        }
    }
}
