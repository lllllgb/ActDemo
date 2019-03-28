using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
public class MakeArtFont
{

    [MenuItem("Assets/LoadFont（选中目标文件夹）")]
    static void CarteNewFont()
    {

        string[] strs = Selection.assetGUIDs;
        string path = AssetDatabase.GUIDToAssetPath(strs[0]);

        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        
        CombinImage(path, Path.GetFileName(path));

    }

    /// <summary>
    /// 调用此函数后使此图片合并
    /// </summary>
    /// <param name="sourceImg">粘贴的源图片</param>
    /// <param name="destImg">粘贴的目标图片</param>
    public static void CombinImage(string sourceImg, string fileName)
    {
        string[] pngs = Directory.GetFiles(sourceImg, "*.png");

        string texturePath = sourceImg + "/" + fileName + "_texture.png";

        int maxWidth = 0;
        int maxHeight = 0;

        foreach (string path in pngs)
        {
            string subPngPath = path.Replace(Application.dataPath, "Assets");

            if (subPngPath.IndexOf("texture") == -1)
            {
                Texture tex2D = AssetDatabase.LoadAssetAtPath(subPngPath, typeof(Texture)) as Texture;
                maxWidth += tex2D.width;
                if (maxHeight < tex2D.height)
                {
                    maxHeight = tex2D.height;
                }
            }
        }
        Texture2D fullTexture = new Texture2D(maxWidth, maxHeight, TextureFormat.RGBA32, false);
        Color[] fullTcolors = fullTexture.GetPixels();
        for (int i = 0; i < fullTcolors.Length; i++)
        {
            fullTcolors[i].a = 0;
            fullTcolors[i].r = 0;
            fullTcolors[i].g = 0;
            fullTcolors[i].b = 0;
        }
        fullTexture.SetPixels(fullTcolors);
        int posX = 0;
        List<BmInfo> bmInfoList = new List<BmInfo>();

        foreach (string path in pngs)
        {
            string subPngPath = path.Replace(Application.dataPath, "Assets");

            if (subPngPath.IndexOf("texture") == -1)
            {

                SetTextureImporter(subPngPath);

                Texture2D tex2D = AssetDatabase.LoadAssetAtPath(subPngPath, typeof(Texture2D)) as Texture2D;
                Color[] colors = tex2D.GetPixels();
                fullTexture.SetPixels(posX, 0, tex2D.width, tex2D.height, colors);

                BmInfo bmInfo = new BmInfo();

                bmInfo.index = Uncode(tex2D.name);
                bmInfo.width = tex2D.width;
                bmInfo.height = tex2D.height;
                bmInfo.x = posX;
                bmInfo.y = 0;

                bmInfoList.Add(bmInfo);

                posX += tex2D.width;

            }

        }

        byte[] full = fullTexture.EncodeToPNG();
        File.WriteAllBytes(texturePath, full);

        AssetDatabase.Refresh();

        texturePath = texturePath.Replace(Application.dataPath, "Assets");

        SetTextureImporter(texturePath);

        Texture texture = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture)) as Texture;
        CreateBitmapFont(fileName, texture, bmInfoList, maxWidth, maxHeight);


    }

    public static void SetTextureImporter(string subPngPath)
    {
        TextureImporter ti = AssetImporter.GetAtPath(subPngPath) as TextureImporter;
        ti.textureType = TextureImporterType.Sprite;
        ti.mipmapEnabled = false;
        ti.isReadable = true;
        ti.filterMode = FilterMode.Trilinear;
        ti.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        AssetDatabase.ImportAsset(subPngPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
    }


    /// <summary>
    /// 创建新的美术字体
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="bmInfoList"></param>
    /// <param name="columnNum"></param>
    /// <param name="lineNum"></param>
    public static void CreateBitmapFont(string fileName, Texture texture, List<BmInfo> bmInfoList, int maxWidth, int maxHeight)
    {

        string assetPath = GetAssetPath(AssetDatabase.GetAssetPath(texture));
        string fontPath = assetPath + "/" + fileName + "_1.fontsettings";
        string materialPath = assetPath + "/" + fileName + "_2.mat";


        Material fontMaterial = AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) as Material;//new Material(Shader.Find("UI/Default"));
        if (fontMaterial == null)
        {
            fontMaterial = new Material(Shader.Find("UI/Default"));
            fontMaterial.mainTexture = texture;
            AssetDatabase.CreateAsset(fontMaterial, materialPath);
        }
        else
        {
            fontMaterial.mainTexture = texture;
        }

        Font assetFont = AssetDatabase.LoadAssetAtPath(fontPath, typeof(Font)) as Font;

        string ApplicationDataPath = Application.dataPath.Replace("Assets", "") + fontPath;

        if (assetFont == null)
        {
            assetFont = new Font();
            AssetDatabase.CreateAsset(assetFont, fontPath);
        }

        assetFont.material = fontMaterial;
        CharacterInfo[] characters = new CharacterInfo[bmInfoList.Count];
        for (int i = 0; i < bmInfoList.Count; i++)
        {
            BmInfo bmInfo = bmInfoList[i];
            CharacterInfo info = new CharacterInfo();
            info.index = bmInfo.index;
            info.uv.width = (float)bmInfo.width / (float)maxWidth;
            info.uv.height = (float)bmInfo.height / (float)maxHeight;
            info.uv.x = (float)bmInfo.x / (float)maxWidth;
            info.uv.y = (1 - (float)bmInfo.y / (float)maxHeight) - info.uv.height;
            info.vert.x = 0;
            info.vert.y = 0;
            info.vert.width = (float)bmInfo.width;
            info.vert.height = -(float)bmInfo.height;
            info.width = (float)bmInfo.width;
            characters[i] = info;
        }
        assetFont.characterInfo = characters;
    }
    private static string GetAssetPath(string path)
    {
        return path == "" || path == null ? "Assets/" : path.Substring(0, path.LastIndexOf("/"));
    }
    //将中文字符转为10进制整数
    public static int Uncode(string str)
    {
        int outStr = 0;
        if (!string.IsNullOrEmpty(str))
        {
            if (str == "。")
            {
                outStr = (int)"."[0];
            }
            else
            {
                outStr = ((int)str[0]);
            }
        }
        return outStr;
    }
}
public class BmInfo
{
    public int index = 0;
    public float width = 0;
    public float height = 0;
    public float x = 0;
    public float y = 0;
}