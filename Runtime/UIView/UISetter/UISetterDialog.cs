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

            foreach ( var dObj in datas )
            {
                var data = (Dictionary<string,object>)dObj;
                var prefab = (bool)data["IsPositive"] ? positiveButtonPrefab : negativeButtonPrefab;
                var go = Instantiate(prefab, transform);
                go.name = (string)data["EventName"];
                go.GetComponentInChildren<TextMeshProUGUI>().text = (string)data["Name"];

                CustomButton customButton = go.GetComponent<CustomButton>();
                if ( data.ContainsKey("TargetParentName") )
                {
                    customButton.actionType = ActionType.ActionToPanel;
                    customButton.TargetPanelName = (string)data["TargetParentName"];
                }
                else
                {
                    customButton.actionType = ActionType.Action;
                }
            }
        }
    }
}
