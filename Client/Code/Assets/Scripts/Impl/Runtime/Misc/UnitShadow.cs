using UnityEngine;
using System.Collections;

namespace AosHotfixRunTime
{
    public class UnitShadow
    {
        const float CheckDistance = 20.0f;

        Vector3 mInitOffset = Vector3.zero;
        bool mLastOnGround = false;
        Unit mOwner;
        GameObject mShadowGo;
        int mFloorLayerMask = GameLayer.LayerMask_Scene | GameLayer.LayerMask_Default;

        public void Init(Unit owner, GameObject shadow)
        {
            mOwner = owner;
            mShadowGo = shadow;

            mInitOffset = mShadowGo.transform.localPosition;
        }

        // Update is called once per frame
        public void Update(float deltaTime)
        {
            if (null == mOwner)
                return;

            // we only do raycast shadow when unit on air.
            if (mOwner.OnGround != mLastOnGround || !mOwner.OnGround)
            {
                RaycastHit tmpHitInfo;

                if (Physics.Raycast(mOwner.Position + Vector3.up, Vector3.down, out tmpHitInfo, CheckDistance, mFloorLayerMask))
                {
                    mShadowGo.transform.position = tmpHitInfo.point + mInitOffset;
                }

                mLastOnGround = mOwner.OnGround;
            }
        }
    }
}
