using UnityEngine;
using UnityEngine.UI;

public class LoopItemIndex : MonoBehaviour {

    public Image image;
    public Text text;
    public int index;

    bool isFind = true;
    protected virtual void ScrollCellIndex(int idx)
    {
        index = idx;
        if (isFind)
        {
            isFind = false;
            Find();
        }
    }
    //Item查找
    protected virtual void Find() { AddListen(); }
    //事件监听
    protected virtual void AddListen() { }
    //数据刷新
    protected virtual void RefreshData() { }
    //资源释放
    protected virtual void Dispose() { isFind = true; }
}
