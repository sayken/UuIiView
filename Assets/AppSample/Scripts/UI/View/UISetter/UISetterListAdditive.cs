using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UuIiView;
using System;
using System.Linq;

[RequireComponent(typeof(ScrollRect))]
public class UISetterListAdditive : UISetter
{
    [SerializeField] private UIPanel uiPanelRoot;
    [SerializeField] private LayoutGroup layoutGroup; // GridLayoutGroup or VerticalLayoutGroup に対応（HorizontalLayoutGroupは非対応）
    [SerializeField] UIViewRoot itemCellForCalc;
    [SerializeField] List<UIViewRoot> itemCells = new List<UIViewRoot>();

    private ScrollRect scrollRect;
    private ScrollRect ScrollRect
    {
        get
        {
            scrollRect = scrollRect ?? GetComponent<ScrollRect>();
            return scrollRect;
        }
    }

    // 表示用リストのRawデータ
    public List<ListData> rawListData = new List<ListData>();

    // リストの上に追加しているタイプ（lineやslackなど）の時はtrue, 下に追加していくタイプはfalse
    public bool reverse = false;

    void Awake()
    {
        var csf = ScrollRect.content.GetComponent<ContentSizeFitter>();
        if ( csf != null ) csf.enabled = false;
        var lg = ScrollRect.content.GetComponent<LayoutGroup>();
        if ( lg != null ) lg.enabled = false;

        var uiCustoms = GetComponentsInChildren<IUICustom>(true);
        foreach ( var uiCustom in uiCustoms)
        {
            uiCustom.ParentName = gameObject.name;
        }
    }

    public override void Set(object obj)
    {

        // データを追加する
        Setup(obj);
    }

    [SerializeField] float scrollY;
    [SerializeField] float scrollHeight;

    /// <summary>
	/// ScrollRectのOnValueChangeのたびに呼ばれる
	/// </summary>
	/// <param name="pos"></param>
    public void OnUpdate(Vector2 pos)
    {
        float y = -ScrollRect.content.anchoredPosition.y;
        float h = ScrollRect.GetComponent<RectTransform>().sizeDelta.y;
        scrollY = y;
        scrollHeight = h;
        for (int i = 0; i < rawListData.Count; i++)
        {
            if ( y >= rawListData[i].Rect.y-rawListData[i].Rect.height && y-h <= rawListData[i].Rect.y )
            {
                // 表示領域内のセル
                if ( rawListData[i].cellIndex == -1 )
                {
                    // 新たに表示領域に入ってきたCell
                    var cellIdx = itemCells.FindIndex(_ => _.gameObject.activeSelf == false);
                    if ( cellIdx == -1 )
                    {
                        Debug.LogError("セルが足りない");
                        return;
                    }
                    rawListData[i].cellIndex = cellIdx;
                    itemCells[cellIdx].Init(uiPanelRoot.ViewRoot, rawListData[i].Data);
                    var rt = itemCells[cellIdx].GetComponent<RectTransform>();
                    rt.anchoredPosition = rawListData[i].Rect.position;
                    rt.sizeDelta = rawListData[i].Rect.size;
                    itemCells[cellIdx].gameObject.SetActive(true);
                }
            }
            else if ( rawListData[i].cellIndex >= 0 )
            {
                // 表示領域外なのに表示フラグがOn => 今回の更新で表示領域外に出たので非表示にする
                itemCells[rawListData[i].cellIndex].gameObject.SetActive(false);
                rawListData[i].cellIndex = -1;
            }
        }
    }

    /// <summary>
	/// 一覧をデータに追加する。reverse が on の時はリストの前に挿入、offの時はリストの後ろに追加。
	/// </summary>
	/// <param name="obj"></param>
	/// <returns></returns>
    void Setup(object obj)
    {
        ScrollRect.onValueChanged.RemoveAllListeners();

        var dataList = (IList)obj;
        if (obj.GetType() == typeof(Newtonsoft.Json.Linq.JArray))
        {
            dataList = new List<object>();
            foreach (var o in (IList)obj)
            {
                var d = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(o.ToString());
                dataList.Add(d);
            }
        }

        // 追加分をlistDataに追加
        List<ListData> addList = new();
        List<string> duplicatedIds = new();
        foreach (Dictionary<string, object> d in dataList)
        {
            if (d.ContainsKey("Id"))
            {
                var dat = new ListData() { Id = d["Id"].ToString(), Data = d, Rect = Rect.zero, cellIndex = -1 };

                itemCellForCalc.Init(uiPanelRoot.ViewRoot, d);

                // contents size fitter を反映させる
                Canvas.ForceUpdateCanvases();

                var rectSize = itemCellForCalc.GetComponent<RectTransform>().sizeDelta;
                //Debug.Log($"Id={dat.Id} / size={rectSize} / rect={itemCellForCalc.GetComponent<RectTransform>().rect}");
                dat.Rect.size = rectSize;

                // 既に同じIdが存在している場合は上書き
                int idx = rawListData.FindIndex(x=>x.Id==dat.Id);
                if ( idx == -1 )
                {
                    addList.Add(dat);
                }
                else
                {
                    rawListData[idx] = dat;
                }
            }
        }

        var prevPos = ScrollRect.content.anchoredPosition;
        itemCells.ForEach(_ => _.gameObject.SetActive(false));
        if ( reverse )
        {
            rawListData.InsertRange(0, addList);
        }
        else
        {
            rawListData.AddRange(addList);
        }

        // 全体の位置とサイズを計測
        RectOffset padding = layoutGroup.padding;
        Vector2 spacing = new Vector2(0, 0);
        bool isGridLayout = false;
        if ( layoutGroup.GetType() == typeof(VerticalLayoutGroup) )
        {
            spacing = new Vector2(0, ((VerticalLayoutGroup)layoutGroup).spacing);
        }
        else if ( layoutGroup.GetType() == typeof(GridLayoutGroup) )
        {
            spacing = ((GridLayoutGroup)layoutGroup).spacing;
            isGridLayout = true;
        }

        float x = padding.left;
        float y = -padding.top;
        for ( int i=0; i< rawListData.Count; i++ )
        {
            rawListData[i].Rect.position = new Vector2(x, y);

            if ( isGridLayout )
            {
                x += rawListData[i].Rect.width;
                x += spacing.x;
                if ( x > ScrollRect.content.sizeDelta.x - padding.right )
                {
                    x = padding.left;
                    y -= rawListData[i].Rect.height;
                    y -= spacing.y;
                }
            }
            else
            {
                y -= rawListData[i].Rect.height;
                y -= spacing.y;
            }
            
            rawListData[i].cellIndex = -1;
        }

        var size = ScrollRect.content.sizeDelta;
        size.y = padding.bottom - rawListData.Last().Rect.y + rawListData.Last().Rect.height; // Contentの高さ。rawListDataのyはpadding.Top込みの座標だからここでは含めない
        ScrollRect.content.sizeDelta = size;

        if (reverse)
        {
            if (addList.Count == rawListData.Count)
            {
                ScrollRect.verticalNormalizedPosition = 0f;
            }
            else
            {
                var pos = ScrollRect.content.anchoredPosition;
                pos.y = -rawListData[addList.Count].Rect.y - padding.top + prevPos.y;
                ScrollRect.content.anchoredPosition = pos;
            }
        }

        OnUpdate(Vector2.zero);

        ScrollRect.onValueChanged.AddListener(OnUpdate);
    }
}

[Serializable]
public class ListData
{
    public string Id;
    public Rect Rect;
    public Dictionary<string, object> Data;
    public int cellIndex; // 表示するデータのindex。-1の時は非表示状態。
}

