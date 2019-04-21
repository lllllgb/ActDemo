
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

/// <summary>
/// 播放视频
/// </summary>
public class PlayVideo : MonoBehaviour
{

    //定义参数获取VideoPlayer组件和RawImage组件
    private VideoPlayer videoPlayer;

    private RawImage rawImage;

    void Start()
    {

        //获取场景中对应的组件
        videoPlayer = this.GetComponent<VideoPlayer>();
        //视频循环播放
        videoPlayer.isLooping = true;

        rawImage = this.GetComponent<RawImage>();

    }

    void Update()
    {
        //如果videoPlayer没有对应的视频texture，则返回
        if (videoPlayer.texture == null)
        {
            return;
        }

        //把VideoPlayerd的视频渲染到UGUI的RawImage
        rawImage.texture = videoPlayer.texture;

    }

}