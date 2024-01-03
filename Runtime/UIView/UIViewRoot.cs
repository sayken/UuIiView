using System.Collections.Generic;
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
        Button,
        Toggle,
        CustomButton,
        CustomLongTap,
        CustomToggle,
        InputFieldValueChanged,
        InputFieldEndEdit,
        Open,
        Close,
        Slider,
    }

    public class UIViewRoot : MonoBehaviour
    {
        private object data;
        private UIPanel rootPanel;
        public Action<string> OnEvent { get; private set; }

        public void ButtonEvent(Button btn) =>           ReceiveEvent(rootPanel.gameObject.name, btn.name, EventType.Button, data, true);
        public void ToggleEvent(Toggle tgl) =>           ReceiveEvent(rootPanel.gameObject.name, tgl.name, EventType.Toggle, data, tgl.isOn);
        public void SliderEvent(Slider slider) =>        ReceiveEvent(rootPanel.gameObject.name, slider.name, EventType.Slider, slider.value, true);

        public void ViewEvent(string targetPanelName, string name, EventType type, bool isOn = true) => ReceiveEvent(targetPanelName, name, type, data, isOn);
        public void ViewEvent(string name, EventType type, bool isOn = true) => ReceiveEvent(rootPanel.gameObject.name, name, type, data, isOn);
        public void InputEvent(string name, EventType type, string data) => ReceiveEvent(rootPanel.gameObject.name, name, type, data, true);

        // 初期化
        public void Init(object d, Action<string> onEvent) => Init(null, d, onEvent);
        public void Init(UIPanel root, object d) => Init(root, d, root.vm.OnEvent);
        void Init(UIPanel root, object d, Action<string> onEvent)
        {
            SetReceiver(onEvent);
            SetData(root, d);
        }

        // イベントを受け取ってCommandLinkに変換
        public void ReceiveEvent(string panelName, string name, EventType type, object data, bool isOn)
        {
            string commandLink = panelName + "/" + name + "/" + type.ToString() + "/" + isOn;
            if (data != null)
            {
                if (type == EventType.InputFieldValueChanged || type == EventType.InputFieldEndEdit )
                {
                    commandLink += "//Input=" + data.ToString();
                }
                else if ( type == EventType.Slider )
                {
                    commandLink += "//Slider=" + data.ToString();
                }
                else
                {
                    var dic = (Dictionary<string, object>)data;
                    if (dic.ContainsKey("Id"))
                    {
                        commandLink += "/" + dic["Id"];
                    }
                    // 名前の最後にIdが付くものは、パラメータとしてCommandLinkに追加
                    foreach (var kv in dic)
                    {
                        if (kv.Key != "Id" && kv.Key.EndsWith("Id"))
                        {
                            commandLink += $"/{kv.Key}={kv.Value}";
                        }
                    }
                }
            }

            OnEvent?.Invoke(commandLink);
        }

        public void SetReceiver(Action<string> onEvent)
        {
            this.OnEvent = onEvent;
        }

        // Json( or protobuf) を渡す
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
                UpdateDataByProto(d);
            }
        }

        // 実処理
        void UpdateDataByProto(object d)
        {
            data = d;
            var trans = gameObject.GetComponentsInChildren<RectTransform>(true);

            var infos = data.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            //Log(infos);
            foreach (Transform t in trans)
            {
                var prop = infos.FirstOrDefault(_ => _.Name == t.gameObject.name);
                if (prop == null) continue;
                SetObj(t, prop.GetValue(data));
            }
        }

        void UpdateDataByDic(Dictionary<string,object> dic)
        {
            data = dic;
            var trans = gameObject.GetComponentsInChildren<RectTransform>(true);
            foreach (Transform t in trans)
            {
                if (!dic.ContainsKey(t.gameObject.name)) continue;
                SetObj(t, dic[t.gameObject.name]);
            }
        }

        void SetObj(Transform t, object obj)
        {
            try
            {
                var uiSetter = t.GetComponent<UISetter>();
                if (uiSetter != null)
                {
                    uiSetter.SetObj(obj);
                }

                if (t == gameObject.transform) return;
                t.GetComponent<UIViewRoot>()?.Init(rootPanel, obj, OnEvent);
            }
            catch(Exception e)
            {
                Debug.LogError("e = " + e.ToString());
                Debug.LogError("obj = " + obj.ToString());
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

    }
}
