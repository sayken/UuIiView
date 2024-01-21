using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UuIiView;

public class UISetterList : UISetter
{
    private UIPanel uiPanelRoot;
    [SerializeField] private UIViewRoot cellPrefab;
    [SerializeField] private Transform listRoot;
    [SerializeField] string itemName;

    List<UIViewRoot> itemCells = new List<UIViewRoot>();

    public override void Set(object obj)
    {
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

        uiPanelRoot = GetComponentInParent<UIPanel>();

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
                itemCells[i].Init(uiPanelRoot, dataList[i]);
                itemCells[i].gameObject.SetActive(true);
            }
            else
            {
                itemCells[i].gameObject.SetActive(false);
            }
        }
    }
}
