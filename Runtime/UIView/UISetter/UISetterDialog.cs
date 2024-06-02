using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using TMPro;

namespace UuIiView
{
    public class UISetterDialog : UISetter
    {
        [SerializeField] GameObject positiveButtonPrefab;
        [SerializeField] GameObject negativeButtonPrefab;

        public override void Set(object obj)
        {
            var datas = (IList)obj;

            foreach ( var data in datas )
            {
                var dic = JsonConvert.DeserializeObject<Dictionary<string,object>>(data.ToString());
                var prefab = (bool)dic["IsPositive"] ? positiveButtonPrefab : negativeButtonPrefab;
                var go = Instantiate(prefab, transform);
                go.name = (string)dic["EventName"];
                go.GetComponentInChildren<TextMeshProUGUI>().text = (string)dic["Name"];

                CustomButton customButton = go.GetComponent<CustomButton>();
                if ( dic.ContainsKey("TargetParentName") )
                {
                    customButton.actionType = ActionType.ActionToPanel;
                    customButton.TargetPanelName = (string)dic["TargetParentName"];
                }
                else
                {
                    customButton.actionType = ActionType.Action;
                }
            }
        }
    }
}
