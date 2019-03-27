using UnityEngine;

namespace AosHotfixFramework
{
	public static class Hotfix
	{
        public static GameObject Instantiate(Object original)
        {
            GameObject tmpGo = Object.Instantiate(original) as GameObject;

#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_STANDALONE_OSX || UNITY_IOS)
            ReBindShader(tmpGo);
#endif

            return tmpGo;
        }

        public static void ReBindShader(GameObject go)
        {
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] mats = renderers[i].sharedMaterials;
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
        }
    }
}