using System;
using System.Collections.Generic;
using System.Diagnostics;

public sealed class Logger
{
    static string ServerLogFormatStr = "<color=yellow>{0}</color>";
    /// <summary>
    /// 是否使用log
    /// </summary>
    public static bool useLog = SetDefaultLogState();
    static List<IBaseLogger> msLoggerList = new List<IBaseLogger>();

    public static void ServerLog(string message)
    {
        if (useLog)
        {
            message = string.Format(ServerLogFormatStr, message);
            LoggerBehavior.Log(message, LoggerBehavior.LoggerType.Log);
            UnityEngine.Debug.Log(message);
        }
    }

    public static void AddLogger(IBaseLogger logger)
    {
        if (logger != null && !msLoggerList.Contains(logger))
            msLoggerList.Add(logger);
    }

#if UNITY_IPHONE || UNITY_ANDROID

    public static bool SetDefaultLogState() 
    {
        return true;
    }

    public static void LogError(object value)
    {
        if (value == null)
        {
            UnityEngine.Debug.LogError("LogError value is null");
            return;
        }

        if (useLog)
            UnityEngine.Debug.LogError(value.ToString());

        LoggerBehavior.Log(value.ToString(), LoggerBehavior.LoggerType.Error);

        for (int i = 0; i < msLoggerList.Count; i++)
        {
            IBaseLogger logger = msLoggerList[i];
            if (logger != null)
                logger.LogError(value);
        }
    }

    public static void LogErrorFormat(string format, params object[] args)
    {
        if (format == null)
        {
            UnityEngine.Debug.LogError("LogErrorFormat format is null");
            return;
        }

        if (args == null)
        {
            UnityEngine.Debug.LogError("LogErrorFormat args is null");
            return;
        }

        if (useLog)
            UnityEngine.Debug.LogErrorFormat(format, args);

        for (int i = 0; i < msLoggerList.Count; i++)
        {
            IBaseLogger logger = msLoggerList[i];
            if (logger != null)
                logger.LogErrorFormat(format, args);
        }
    }

    public static void LogWarning(object value)
    {
        if (value == null)
        {
            UnityEngine.Debug.LogError("LogWarning value is null");
            return;
        }

        if (useLog)
            UnityEngine.Debug.LogWarning(value.ToString());

        for (int i = 0; i < msLoggerList.Count; i++)
        {
            IBaseLogger logger = msLoggerList[i];
            if (logger != null)
                logger.LogWarning(value);
        }
    }

    public static void LogWarningFormat(string format, params object[] args)
    {
        if (format == null)
        {
            UnityEngine.Debug.LogError("LogWarningFormat format is null");
            return;
        }

        if (args == null)
        {
            UnityEngine.Debug.LogError("LogWarningFormat args is null");
            return;
        }

        if (useLog)
            UnityEngine.Debug.LogWarningFormat(format, args);

        for (int i = 0; i < msLoggerList.Count; i++)
        {
            IBaseLogger logger = msLoggerList[i];
            if (logger != null)
                logger.LogWarningFormat(format, args);
        }
    }

    public static void Log(object value)
    {
        if (value == null)
        {
            UnityEngine.Debug.LogError("Log value is null");
            return;
        }

        if (useLog)
        {
            UnityEngine.Debug.Log(value.ToString());
        }

        for (int i = 0; i < msLoggerList.Count; i++)
        {
            IBaseLogger logger = msLoggerList[i];
            if (logger != null)
                logger.Log(value);
        }
    }

    public static void LogFormat(string format, params object[] args)
    {
        if (format == null)
        {
            UnityEngine.Debug.LogError("LogFormat format is null");
            return;
        }

        if (args == null)
        {
            UnityEngine.Debug.LogError("LogFormat args is null");
            return;
        }

        if (useLog)
            UnityEngine.Debug.LogFormat(format, args);

        for (int i = 0; i < msLoggerList.Count; i++)
        {
            IBaseLogger logger = msLoggerList[i];
            if (logger != null)
                logger.LogFormat(format, args);
        }
    }

#else

    public static bool SetDefaultLogState()
    {
        return true;
    }
    public delegate void LogErrorFormatAction(string format, params object[] args);
    public static LogErrorFormatAction LogErrorFormat = UnityEngine.Debug.LogErrorFormat;
    public static Action<object> LogError = UnityEngine.Debug.LogError;

    public delegate void LogWarningFormatAction(string format, params object[] args);
    public static LogWarningFormatAction LogWarningFormat = UnityEngine.Debug.LogWarningFormat;
    public static Action<object> LogWarning = UnityEngine.Debug.LogWarning;

    public delegate void LogFormatAction(string format, params object[] args);
    public static LogFormatAction LogFormat = UnityEngine.Debug.LogFormat;
    public static Action<object> Log = UnityEngine.Debug.Log;

#endif

    public static string GetStack()
    {
        StackTrace trace = new StackTrace(true);
        return trace.ToString();
    }
}