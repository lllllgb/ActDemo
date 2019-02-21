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

        public ActionUnitManager()
        {
        }

        public void Add(IActUnit actUnit)
        {
            mUnits.Add(actUnit);
        }

        public void Remove(IActUnit actUnit)
        {
            mUnits.Remove(actUnit);
        }

        public void ClearAll()
        {
            mUnits.Clear();
        }
    }
}
