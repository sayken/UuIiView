using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UuIiView
{
    [RequireComponent(typeof(ScrollRect))]
    public class UISetterList : UISetter
    {
        [SerializeField] private UIPanel uiPanelRoot;
        [SerializeField] private UIViewRoot cellPrefab;
        [SerializeField] string itemName;

        List<UIViewRoot> itemCells = new List<UIViewRoot>();

        public override void Set(object obj)
        {
            var data = ConvertData<UIListData>(obj);
            SetList(data.Datas);
        }

        /// <summary>
        /// リストデータを生成して初期化
        /// </summary>
        /// <param name="obj">各要素のJsonのList</param>
        public virtual void SetList(IList obj)
        {
            var listRoot = GetComponent<ScrollRect>().content;

            for (int i = 0; i < obj.Count; i++)
            {
                var data = ConvertData<Dictionary<string, object>>(obj[i]);

                if (itemCells.Count > i)
                {
                    // 既にitemCellsにデータがあるので流用
                    itemCells[i].Init(uiPanelRoot, data);
                }
                else
                {
                    // itemCellsにデータが無い（or 足りない）ので追加
                    var vm = Instantiate(cellPrefab, listRoot);
                    if (!string.IsNullOrWhiteSpace(itemName)) vm.gameObject.name = itemName;
                    vm.Init(uiPanelRoot, data);
                    itemCells.Add(vm);
                }
            }

            // 必要なitemCellだけ表示状態にする
            for (int i = 0; i < itemCells.Count; i++)
            {
                itemCells[i].gameObject.SetActive(obj.Count > i);
            }
        }

        T ConvertData<T>(object obj)
        {
            if ( obj.GetType() == typeof(string) )
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(obj.ToString());
            }
            return (T)obj;
        }
    }

    public class UIListData
    {
        // List表示に必要なパラメータなど
        public Dictionary<string,object> Info;
        // Listの実データ
        public IList Datas;
    }
}