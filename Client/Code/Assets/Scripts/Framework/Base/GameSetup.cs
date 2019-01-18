using UnityEngine;
using System.Collections;
using System.Xml;

public class GameSetup
{
    private static GameSetup msInstance = new GameSetup();

    public static GameSetup instance
    {
        get { return GameSetup.msInstance; }
    }

    string mDns = string.Empty;
    bool mIsPublish = true;//是否发布

    bool mIsUsePlatform = false;//是否使用平台
    bool mIsResUpdate = false;//是否更新资源
    bool mIsCodeUpdate = false;//是否更新代码
    string mPlatformIp = "192.168.3.6";//平台IP
    int mPlatformPort = 8183;//平台端口号
    string mUnionID = "1";
    string mUpdateUrl = "http://192.168.3.183/UpdateRes/";//资源更新地址
    string mClientUrl = string.Empty;//客户端地址
    string mCrashUrl = string.Empty;//崩溃信息地址
    string mExceptUrl = string.Empty;//异常信息地址

    #region Property

    public string DNS
    {
        get { return mDns; }
        set { mDns = value; }
    }

    public bool IsPublish
    {
        get { return mIsPublish; }
        set { mIsPublish = value; }
    }

    public bool IsUsePlatform
    {
        get { return mIsUsePlatform; }
        set { mIsUsePlatform = value; }
    }

    public bool IsResUpdate
    {
        get { return mIsResUpdate; }
        set { mIsResUpdate = value; }
    }

    public bool IsCodeUpdate
    {
        get { return mIsCodeUpdate; }
        set { mIsCodeUpdate = value; }
    }

    public string platformIp
    {
        get { return mPlatformIp; }
        set { mPlatformIp = value; }
    }

    public int platformPort
    {
        get { return mPlatformPort; }
        set { mPlatformPort = value; }
    }

    public string unionID
    {
        get { return mUnionID; }
        set { mUnionID = value; }
    }

    public string UpdateUrl
    {
        get { return mUpdateUrl; }
        set { mUpdateUrl = value; }
    }

    public string clientUrl
    {
        get { return mClientUrl; }
        set { mClientUrl = value; }
    }

    public string crashUrl
    {
        get { return mCrashUrl; }
        set { mCrashUrl = value; }
    }

    public string exceptUrl
    {
        get { return mExceptUrl; }
        set { mExceptUrl = value; }
    }

    #endregion

    public bool Load()
    {
        TextAsset textAsset = (TextAsset)Resources.Load("setting", typeof(TextAsset));
        if (textAsset == null)
            return false;

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textAsset.text);
        XmlNode rootNode = doc.SelectSingleNode("Setup");
        mDns = rootNode.SelectSingleNode("dns").InnerText.Trim();
        mIsPublish = rootNode.SelectSingleNode("publish").InnerText == "1";

        return true;
    }
}
