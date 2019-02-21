using UnityEngine;

namespace AosBaseFramework
{
    /// <summary>
    /// 实用函数集。
    /// </summary>
    public static partial class Utility
    {
        //比较浮点数
        public static bool CompareFloatValue(float a, float b, int accuracy = 10000)
        {
            return Mathf.RoundToInt(a * accuracy) == Mathf.RoundToInt(b * accuracy);
        }

        //
        public static string TimeStrBySecond(int second, bool isSameRAM = false)
        {
            return isSameRAM ? FormatTimeStr(second / 3600, second % 3600 / 60, second % 60) :
                FormatTimeStr(second / 3600, second % 3600 / 60, second % 60);
        }

        public static string FormatTimeStr(int hour, int min, int sec)
        {
            string tempStr = string.Empty;

            if (hour < 10)
            {
                tempStr += "0";
                tempStr += hour;
            }
            else
                tempStr += hour;

            tempStr += ":";

            if (min < 10)
            {
                tempStr += "0";
                tempStr += min;
            }
            else
                tempStr += min;

            tempStr += ":";

            if (sec < 10)
            {
                tempStr += "0";
                tempStr += sec;
            }
            else
                tempStr += sec;

            return tempStr;
        }
    }
}
