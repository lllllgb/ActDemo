using System;
using System.Collections.Generic;
using UnityEngine;

namespace AosHotfixRunTime
{
    public static class GameLayer
    {
        public static string LayerName_Default = "Default";
        public static string LayerName_UI = "UI";
        public static string LayerName_Scene = "Scene";
        public static string LayerName_Unit = "Unit";
        public static string LayerName_HudInfo = "HudInfo";
        public static string LayerName_HudPopup = "HudPopup";

        public static int Layer_Default = LayerMask.NameToLayer(LayerName_Default);
        public static int Layer_UI = LayerMask.NameToLayer(LayerName_UI);
        public static int Layer_Scene = LayerMask.NameToLayer(LayerName_Scene);
        public static int Layer_Unit = LayerMask.NameToLayer(LayerName_Unit);
        public static int Layer_HudInfo = LayerMask.NameToLayer(LayerName_HudInfo);
        public static int Layer_HudPopup = LayerMask.NameToLayer(LayerName_HudPopup);

        public static int LayerMask_Nothing = 0;
        public static int LayerMask_Default = GetMask(Layer_Default);
        public static int LayerMask_UI = GetMask(Layer_UI);
        public static int LayerMask_Scene = GetMask(Layer_Scene);
        public static int LayerMask_Unit = GetMask(Layer_Unit);
        public static int LayerMask_HudInfo = GetMask(Layer_HudInfo);
        public static int LayerMask_HudPopup = GetMask(Layer_HudPopup);

        public static int GetMask(int layer)
        {
            return 1 << layer;
        }

        public static int GetCombineMask(string[] layers)
        {
            string text = string.Empty;
            int num = 0;
            for (int i = 0; i < layers.Length; i++)
            {
                text = layers[i];
                int num2 = LayerMask.NameToLayer(text);
                num |= 1 << num2;
            }
            return num;
        }
    }
}

