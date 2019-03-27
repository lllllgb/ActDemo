using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Model;

namespace MyEditor
{
    public class EffectTool : EditorWindow
    {
        const string Effect_Path = "Assets/Bundles/Effect/";

        [MenuItem("Tools/资源相关/特效初始化")]
        static void InitEffect()
        {
            List<string> tmpPaths = EditorResHelper.GetAllResourcePath(Effect_Path, false);

            foreach (string path in tmpPaths)
            {
                string tmpPath = path.Replace('\\', '/');
                GameObject tmpGo = AssetDatabase.LoadAssetAtPath<GameObject>(tmpPath);

                if (null == tmpGo)
                {
                    continue;
                }

                EffectRecord tmpEffectRecord = tmpGo.GetComponent<EffectRecord>();

                if (null == tmpEffectRecord)
                {
                    tmpEffectRecord = tmpGo.AddComponent<EffectRecord>();
                }

                tmpEffectRecord.Init();
                EditorUtility.SetDirty(tmpGo);
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/资源相关/重新指定特效shader")]
        static void InitEffectShader()
        {
            List<string> tmpPaths = EditorResHelper.GetAllResourcePath(Effect_Path, false);

            foreach (string path in tmpPaths)
            {
                string tmpPath = path.Replace('\\', '/');
                GameObject tmpGo = AssetDatabase.LoadAssetAtPath<GameObject>(tmpPath);

                if (null == tmpGo)
                {
                    continue;
                }

                Renderer[] tmpRenderers = tmpGo.GetComponentsInChildren<Renderer>(true);

                for (int i = 0; i < tmpRenderers.Length; i++)
                {
                    Material[] mats = tmpRenderers[i].sharedMaterials;
                    for (int j = 0; j < mats.Length; j++)
                    {
                        Material mat = mats[j];
                        if (mat != null)
                        {
                            Shader shader = Shader.Find(mat.shader.name);

                            if (shader != null)
                                mat.shader = shader;
                        }

                    }
                }

                EditorUtility.SetDirty(tmpGo);
            }

            AssetDatabase.Refresh();
        }
    }
}
