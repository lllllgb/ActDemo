using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRenderers : MonoBehaviour
{
    public Renderer[] Renderers;

    private bool mEnableRim = false;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void EnableRim(bool flag)
    {
        if (mEnableRim == flag)
        {
            return;
        }

        mEnableRim = flag;

        for (int i = 0; i < Renderers.Length; i++)
        {
            Renderer renderer = Renderers[i];

            foreach (Material mat in renderer.materials)
            {
                string tempShaderName = mat.shader.name;

                if (tempShaderName.Equals("Shader/Obj-Soft-Light"))
                {
                    mat.SetFloat("_RimPower", flag ? 1 : 0);
                }
            }
        }
    }
}
