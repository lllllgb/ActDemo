using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AosHotfixFramework
{
    public static class UIHelper
    {
        static bool sIsInitMat = false;
        static Material sUIDefaultMat;
        static Material sUIGreyMat;

        public static void GreyImage(Image image, bool greyFlag)
        {
            if (!sIsInitMat)
            {
                sIsInitMat = true;
                sUIDefaultMat = Resources.Load("UIDefaultMat") as Material;
                sUIGreyMat = Resources.Load("UIGreyMat") as Material;
            }
            
            image.material = (greyFlag ? sUIGreyMat : sUIDefaultMat);
        }
    }
}