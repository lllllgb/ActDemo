using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Xml;
using System.IO;
using System.Text;

namespace MyEditor
{
    public class GameSetupEditor : EditorWindow
    {
        string setupFile = "./Assets/Resources/setting.xml";
        GameSetup setup;

        [MenuItem("Tools/发布工具/配置修改", false)]
        static void ShowUIEditor()
        {
            GameSetupEditor window = GetWindow<GameSetupEditor>();
            window.Init();
            window.Show(true);
        }

        void Init()
        {
            titleContent = new GUIContent("配置修改");
            position = new Rect(300, 300, 500, 320);
            Load();
        }

        void Load()
        {
            GameSetup.instance.Load();
            setup = GameSetup.instance;
        }

        void OnGUI()
        {
            if (setup != null)
            {
                GUILayout.Space(10);
                EditorGUILayout.BeginVertical();
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("DNS:", GUILayout.Width(110));
                    setup.DNS = EditorGUILayout.TextField("", setup.DNS);
                    GUILayout.EndHorizontal();

                    GUILayout.Space(10);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("是否发布:", GUILayout.Width(110));
                    setup.IsPublish = GUILayout.Toggle(setup.IsPublish, "", GUILayout.Width(30));
                    GUILayout.EndHorizontal();

                    GUILayout.Space(10);
                }
                EditorGUILayout.EndVertical();

            }

            GUILayout.Space(20);
            if (GUILayout.Button("保存"))
            {
                Save();
            }
        }

        void Save()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(setupFile);
            XmlNode rootNode = doc.SelectSingleNode("Setup");
            rootNode.SelectSingleNode("dns").InnerText = setup.DNS;
            rootNode.SelectSingleNode("publish").InnerText = setup.IsPublish ? "1" : "0";
            doc.Save(setupFile);
            ShowNotification(new GUIContent("保存成功！！"));
            AssetDatabase.Refresh();
        }
    }
}

