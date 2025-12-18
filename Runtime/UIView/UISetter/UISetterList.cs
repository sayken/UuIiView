using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UuIiView
{
    public class UISetterList : UISetter
    {
        private UIViewRoot uiViewRoot;
        [SerializeField] private UIViewRoot cellPrefab;
        [SerializeField] private Transform listRoot;
        [SerializeField] string itemName;

        List<UIViewRoot> itemCells = new List<UIViewRoot>();

        public override void Set(object obj)
        {
            if (obj == null) return;

            IList dataList;
            if (obj is IList list)
            {
                dataList = list;
            }
            else
            {
                Debug.LogWarning($"[UISetterList] {gameObject.name}: objがIListではありません。Type={obj.GetType()}");
                return;
            }

            if (obj.GetType() == typeof(Newtonsoft.Json.Linq.JArray))
            {
                var convertedList = new List<object>();
                foreach (var o in dataList)
                {
                    var d = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(o.ToString());
                    convertedList.Add(d);
                }
                dataList = convertedList;
            }

            uiViewRoot = GetComponentInParent<UIViewRoot>();
            if (uiViewRoot == null)
            {
                Debug.LogError($"[UISetterList] {gameObject.name}: UIViewRootが見つかりません。");
                return;
            }

            for (int i = itemCells.Count; i < dataList.Count; i++)
            {
                var vm = Instantiate(cellPrefab, listRoot);
                if (!string.IsNullOrWhiteSpace(itemName)) vm.gameObject.name = itemName;
                itemCells.Add(vm);
            }
            for( int i=0 ; i<itemCells.Count ; i++ )
            {
                if (dataList.Count > i)
                {
                    itemCells[i].Init(uiViewRoot.RootUIViewRoot, dataList[i]);
                    itemCells[i].gameObject.SetActive(true);
                }
                else
                {
                    itemCells[i].gameObject.SetActive(false);
                }
            }

            // ボタンやトグルといったEvent系の親オブジェクトとしてselfを登録する
            foreach ( var uiCustom in GetComponentsInChildren<IUICustom>(true))
            {
                uiCustom.ParentName = gameObject.name;
            }
        }
    }
}