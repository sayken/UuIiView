﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Reflection;
using System.Text;

namespace UuIiView
{
    public enum EventType
    {
        None,
        Log,
        Button = 10,
        Toggle,
        LongTap,
        Slider,
        Input,
    }

    public enum ActionType
    {
        None,
        Open,
        Close,
        CloseAndOpen,
        Action,
        DataSync,
    }

    /// <summary>
    /// UuIiView のコア処理。
    /// ・Json(or Dictioanry<string, object> or class)を受け取って、Childrensにアタッチされている UISetter に object を渡す
    /// ・Childrens から上がってきた 各種Event を Reciever に渡す
    /// </summary>
    public class UIViewRoot : MonoBehaviour
    {
        private object data;
        private List<UISetter> uiSetters;


        private UIPanel rootPanel;
        public Action<string> OnEvent { get; private set; }


        // ====================================================================================================
        // Init
        // ====================================================================================================
        public void Init(object d, Action<string> onEvent) => Init(null, d, onEvent);
        public void Init(UIPanel root, object d) => Init(root, d, root.vm.OnEvent);
        void Init(UIPanel root, object d, Action<string> onEvent)
        {
            SetReceiver(onEvent);
            SetData(root, d);
        }

        // ====================================================================================================
        // Event Receiver
        // ====================================================================================================
        public void ViewEvent(string targetPanelName, string name, EventType type, ActionType actType, string parentName, object data, bool isOn = true)
            => ReceiveEvent(targetPanelName, name, type, actType, parentName, data, isOn);
        public void ViewEvent(string targetPanelName, string name, EventType type, ActionType actType, string parentName, bool isOn = true)
            => ReceiveEvent(targetPanelName, name, type, actType, parentName, data, isOn);
        public void ViewEvent(string name, EventType type, ActionType actType, string parentName, bool isOn = true)
            => ReceiveEvent(rootPanel.gameObject.name, name, type, actType, parentName, data, isOn);

        // イベントを受け取ってCommandLinkに変換
        void ReceiveEvent(string panelName, string name, EventType eventType, ActionType actionType, string parentName, object data, bool isOn)
        {
            StringBuilder commandLink = new StringBuilder();
            commandLink.Append($"{panelName}/{eventType}/{actionType}/{name}/{parentName}/");
            if (data != null)
            {
                // IdというKeyが含まれていたら、後ろにつける
                Dictionary<string, object> dic = new();
                if ( data.GetType() == typeof(Dictionary<string,object>) )
                {
                    dic = (Dictionary<string, object>)data;
                    if (dic.ContainsKey("Id"))
                    {
                        commandLink.Append(dic["Id"]);
                    }
                }

                commandLink.Append( eventType switch
                {
                    EventType.Input => "/Input=" + data.ToString(),
                    EventType.Slider => "/Slider=" + data.ToString(),
                    EventType.Toggle => "/Toggle=" + isOn,
                    _ => ""
                });

                // 名前の最後にIdが付くものは、パラメータとしてCommandLinkに追加
                dic.Where(_=>_.Key!="Id" && _.Key.EndsWith("Id")).ToList().ForEach(kv=>commandLink.Append($"/{kv.Key}={kv.Value}"));
            }

            OnEvent?.Invoke(commandLink.ToString());
        }

        public void SetReceiver(Action<string> onEvent)
        {
            this.OnEvent = onEvent;
        }

        // ====================================================================================================
        // SetData (Json or Dictionary<string, object> or class)を渡してUIを更新する
        // ====================================================================================================
        public void SetData(object d) => SetData(null, d);
        public void SetData(UIPanel root, object d)
        {
            rootPanel = root == null ? GetComponent<UIPanel>() : root;

            if ( d == null )
            {
                return;
            }
            if (d.GetType() == typeof(string))
            {
                //Log(d.ToString());
                var dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(d.ToString());
                UpdateDataByDic(dic);
            }
            else if ( d.GetType() == typeof(Dictionary<string,object>))
            {
                //Log((Dictionary<string, object>)d);
                UpdateDataByDic((Dictionary<string, object>)d);
            }
            else
            {
                UpdateDataByClass(d);
            }
        }

        // 実処理
        void UpdateDataByClass(object d)
        {
            data = d;
            uiSetters ??= gameObject.GetComponentsInChildren<UISetter>(true).ToList();

            var infos = data.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            //Log(infos);
            foreach (UISetter u in uiSetters)
            {
                var prop = infos.FirstOrDefault(_ => _.Name == u.gameObject.name);
                if (prop == null) continue;
                SetObj(u, prop.GetValue(data));
            }
        }

        void UpdateDataByDic(Dictionary<string,object> dic)
        {
            data = dic;
            uiSetters ??= gameObject.GetComponentsInChildren<UISetter>(true).ToList();
            
            foreach (UISetter u in uiSetters)
            {
                if (!dic.ContainsKey(u.gameObject.name)) continue;
                SetObj(u, dic[u.gameObject.name]);
            }
        }

        void SetObj(UISetter uiSetter, object obj)
        {
            try
            {
                if (uiSetter != null)
                {
                    uiSetter.SetObj(obj);
                }

                if (uiSetter.transform == gameObject.transform) return;
                uiSetter.GetComponent<UIViewRoot>()?.Init(rootPanel, obj, OnEvent);
            }
            catch(Exception e)
            {
                Debug.LogError("e = " + e.ToString() +"\nobj = "+ obj.ToString());
            }
        }

        // =====================================================================================================
        // ログ出力
        // =====================================================================================================
        void Log(string d)
        {
            Debug.Log($"<color=yellow>[UuIiView] SetData (string) {gameObject.name}</color>\n{d}");
        }
        void Log(Dictionary<string, object> dic)
        {
            StringBuilder sb = new StringBuilder();
            foreach( var kv in dic)
            {
                sb.Append(kv.Key).Append(" : ").AppendLine(kv.Value.ToString());
            }
            Debug.Log($"<color=yellow>[UuIiView] SetData (Dictionary) {gameObject.name}</color>\n{sb.ToString()}");
        }
        void Log(PropertyInfo[] infos)
        {
            StringBuilder sb = new StringBuilder();
            foreach ( var pi in infos)
            {
                sb.Append(pi.Name).Append(" : ").AppendLine(pi.GetValue(data).ToString());
            }
            Debug.Log($"<color=yellow>[UuIiView] SetData (Proto) {gameObject.name}</color>\n{sb.ToString()}");
        }

        [ContextMenu("Show Repository Log")]
        public void RepositoryLog()
        {
            string cmd = name +"/Log/None/ShowLog/ParentName/Id";
            OnEvent?.Invoke(cmd);
        }
    }
}
