using System;
using System.Collections.Generic;
using UnityEngine;

namespace AosHotfixRunTime
{
    public static class GameLayer
    {
        public static string LayerName_Default = "Default";
        public static string LayerName_UI = "UI";
        public static string LayerName_Touch = "Touch";
        public static string LayerName_Map = "Map";
        public static string LayerName_Building = "Building";
        public static string LayerName_Unit = "Unit";
        public static string LayerName_Door = "Door";
        public static string LayerName_FightAction = "FightAction";
        public static string LayerName_HUD = "HUD";
        public static string LayerName_Highlight = "Highlight";
        public static string LayerName_HudPopup = "HudPopup";
        public static string LayerName_SceneObj = "SceneObj";

        public static int Layer_Default = LayerMask.NameToLayer(LayerName_Default);
        public static int Layer_UI = LayerMask.NameToLayer(LayerName_UI);
        public static int Layer_Touch = LayerMask.NameToLayer(LayerName_Touch);
        public static int Layer_Map = LayerMask.NameToLayer(LayerName_Map);
        public static int Layer_Building = LayerMask.NameToLayer(LayerName_Building);
        public static int Layer_Unit = LayerMask.NameToLayer(LayerName_Unit);
        public static int Layer_Door = LayerMask.NameToLayer(LayerName_Door);
        public static int Layer_FightAction = LayerMask.NameToLayer(LayerName_FightAction);
        public static int Layer_HUD = LayerMask.NameToLayer(LayerName_HUD);
        public static int Layer_Highlight = LayerMask.NameToLayer(LayerName_Highlight);
        public static int Layer_HudPopup = LayerMask.NameToLayer(LayerName_HudPopup);
        public static int Layer_SceneObj = LayerMask.NameToLayer(LayerName_SceneObj);

        public static int LayerMask_Nothing = 0;
        public static int LayerMask_Default = GetMask(Layer_Default);
        public static int LayerMask_UI = GetMask(Layer_UI);
        public static int LayerMask_Touch = GetMask(Layer_Touch);
        public static int LayerMask_Map = GetMask(Layer_Map);
        public static int LayerMask_Building = GetMask(Layer_Building);
        public static int LayerMask_Unit = GetMask(Layer_Unit);
        public static int LayerMask_Door = GetMask(Layer_Door);
        public static int LayerMask_FightAction = GetMask(Layer_FightAction);
        public static int LayerMask_HUD = GetMask(Layer_HUD);
        public static int LayerMask_Highlight = GetMask(Layer_Highlight);
        public static int LayerMask_HudPopup = GetMask(Layer_HudPopup);
        public static int LayerMask_SceneObj = GetMask(Layer_SceneObj);

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

    public static class SortLayer
    {
        public static string LayerName_Default = "Default";
        public static string LayerName_HouseBg = "House_Bg";
        public static string LayerName_HouseBuilding = "House_Building";
        public static string LayerName_MazeBg1 = "Maze_Bg1";
        public static string LayerName_MazeBg2 = "Maze_Bg2";
        public static string LayerName_MazeDoor = "Maze_Door";
        public static string LayerName_MazeWall = "Maze_Wall";
        public static string LayerName_MazeRoom = "Maze_Room";
        public static string LayerName_Unit = "Unit";
        public static string LayerName_MazeFg = "Maze_Fg";

        public static int LayerID_Default = SortingLayer.NameToID(LayerName_Default);
        public static int LayerID_HouseBg = SortingLayer.NameToID(LayerName_HouseBg);
        public static int LayerID_HouseBuilding = SortingLayer.NameToID(LayerName_HouseBuilding);
        public static int LayerID_MazeBg1 = SortingLayer.NameToID(LayerName_MazeBg1);
        public static int LayerID_MazeBg2 = SortingLayer.NameToID(LayerName_MazeBg2);
        public static int LayerID_MazeDoor = SortingLayer.NameToID(LayerName_MazeDoor);
        public static int LayerID_MazeWall = SortingLayer.NameToID(LayerName_MazeWall);
        public static int LayerID_MazeRoom = SortingLayer.NameToID(LayerName_MazeRoom);
        public static int LayerID_Unit = SortingLayer.NameToID(LayerName_Unit);
        public static int LayerID_MazeFg = SortingLayer.NameToID(LayerName_MazeFg);
    }

    public static class SortOrder
    {
        public static int SortOrder_Unit = 100;

        public static int SortOrder_Unit_UI = 1000;
    }
}

