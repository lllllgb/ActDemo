using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ACT
{
    public class ActionUnitManager
    {

        List<IActUnit> mUnits = new List<IActUnit>();
        public List<IActUnit> Units { get { return mUnits; } }

        public IActUnit LocalPlayer { get; set; }

        private List<IActUnit> mRemoveUnits = new List<IActUnit>();

        public ActionUnitManager()
        {
        }

        public void Add(IActUnit actUnit)
        {
            mUnits.Add(actUnit);
        }

        public void Update(float deltaTime)
        {
            for (int i = 0, max = mRemoveUnits.Count; i < max; ++i)
            {
                mUnits.Remove(mRemoveUnits[i]);
            }

            for (int i = mUnits.Count - 1; i >= 0; --i)
            {
                mUnits[i].Update(deltaTime);
            }
        }

        public void Remove(IActUnit actUnit)
        {
            mRemoveUnits.Add(actUnit);
        }

        public void ClearAll()
        {
            mUnits.Clear();
        }
    }
}
