using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AosHotfixFramework;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AosHotfixRunTime
{
    public class UIDragHelper : Singleton<UIDragHelper>
    {

        public interface IDragItem
        {
            bool isDragging { set; get; }
        }

        GameObject mDragGo;
        RectTransform mDragRectTrans;
        int mBeignPointerID = int.MaxValue;

        public bool BeginDrag(PointerEventData pointerEvent, GameObject go, Transform parent)
        {
            if (null != mDragGo)
            {
                return false;
            }

            mBeignPointerID = pointerEvent.pointerId;
            mDragGo = Hotfix.Instantiate(go);
            mDragGo.transform.SetParent(parent, false);
            mDragRectTrans = mDragGo.GetComponent<RectTransform>();
            mDragGo.transform.localScale = Vector3.one;

            Image tmpImg = mDragGo.GetComponent<Image>();

            if (null != tmpImg)
            {
                tmpImg.raycastTarget = false;
            }

            return true;
        }

        public void Draging(PointerEventData pointerEvent)
        {
            if (null == mDragGo || mBeignPointerID != pointerEvent.pointerId)
            {
                return;
            }

            Vector3 pos;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(mDragRectTrans,
                pointerEvent.position, pointerEvent.pressEventCamera, out pos))
            {
                mDragGo.transform.position = pos;
            }
        }

        public bool EndDrag(PointerEventData pointerEvent)
        {
            if (null == mDragGo || mBeignPointerID != pointerEvent.pointerId)
            {
                return false;
            }

            if (null != mDragGo)
            {
                GameObject.Destroy(mDragGo);
            }

            mBeignPointerID = int.MaxValue;
            return true;
        }

        public bool OnDrop(PointerEventData pointerEvent)
        {
            if (null == mDragGo || mBeignPointerID != pointerEvent.pointerId)
            {
                return false;
            }

            return true;
        }
    }
}
