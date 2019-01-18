using System;
using System.Collections.Generic;
using UnityEngine;

public class LoggerBehavior : MonoBehaviour
{
    public static LoggerBehavior instance;

    void Avake()
    {
        instance = this;
    }

    public enum LoggerType
    {
        Log,
        Warning,
        Error,
    }

    public struct Element
    {
        public string message;
        public LoggerType type;
    }

    private List<Element> mList = new List<Element>();
    public int LogMaxNum = 30;
    public bool showLog = true;
    public bool showWarning = true;
    public bool showError = true;
    public bool showPanel = true;

    public int panelWidth = 350;
    private Vector2 scrollPosition;

    void OnGUI()
    {
        if (showPanel)
        {
            UpdateLog();
            UpdateButton();
        }
    }

    void UpdateLog()
    {
        scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, new GUILayoutOption[]
		{
			GUILayout.Width(panelWidth),
			GUILayout.Height(Screen.height - 50)
		});

        GUILayout.BeginVertical(new GUILayoutOption[0]);
        for (int i = 0; i < mList.Count; i++)
        {
            Element logger = mList[i];
            if (showLog && logger.type == LoggerType.Log)
            {
                GUI.color = Color.white;
                GUILayout.Label(logger.message, new GUILayoutOption[0]);
            }
            if (logger.type == LoggerType.Warning && showWarning)
            {
                GUI.color = Color.yellow;
                GUILayout.Label(logger.message, new GUILayoutOption[0]);
            }
            if (logger.type == LoggerType.Error && showError)
            {
                GUI.color = Color.red;
                GUILayout.Label(logger.message, new GUILayoutOption[0]);
            }
        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }
    void UpdateButton()
    {
        GUI.color = Color.black;
        GUILayout.BeginArea(new Rect(0f, (float)(Screen.height - 50), (float)this.panelWidth, 50f));
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        this.showLog = GUILayout.Toggle(this.showLog, "Log", new GUILayoutOption[0]);
        this.showWarning = GUILayout.Toggle(this.showWarning, "Warning", new GUILayoutOption[0]);
        this.showError = GUILayout.Toggle(this.showError, "Error", new GUILayoutOption[0]);
        if (GUILayout.Button("Clear", new GUILayoutOption[0]))
        {
            mList.Clear();
        }
        if (GUILayout.Button("Close", new GUILayoutOption[0]))
        {
            showPanel = false;
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    public void Add(string message, LoggerType type)
    {
        bool flag = false;
        if (type == LoggerType.Log && showLog)
            flag = true;
        else if (type == LoggerType.Log && showWarning)
            flag = true;
        else if (type == LoggerType.Log && showError)
            flag = true;

        if (flag)
        {
            Element element = new Element();
            element.message = message;
            element.type = type;

            if (mList.Count > LogMaxNum)
                mList.RemoveAt(0);

            mList.Add(element);
        }
    }

    public static void Log(string message, LoggerType type)
    {
        if (instance != null)
            instance.Add(message, type);
    }
}
