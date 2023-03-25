using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace UuIiView
{
    public class UIBasePresenter
    {
        protected Dictionary<string, UIPanel> uiPanelDic = new Dictionary<string, UIPanel>();

        /// ========================================================================
        /// Open
        /// ========================================================================
        protected List<UIPanel> OpenGroup(string name)
        {
            var ret = new List<UIPanel>();
            var uiGroup = UILayer.Inst.GetPanelGroup(name);
            if (uiGroup != null)
            {
                uiGroup.panelNames.ForEach(_ => ret.Add(Open(_)));
            }
            return ret;
        }
        protected UIPanel Open(string name)
        {
            uiPanelDic[name] = UILayer.Inst.AddPanel(name);
            return uiPanelDic[name].Open(OnEvent);
        }

        protected void Close(string name)
        {
            var uiGroup = UILayer.Inst.GetPanelGroup(name);
            if (uiGroup != null)
            {
                uiGroup.panelNames.ForEach(_ => ClosePanel(_));
            }
            else
            {
                ClosePanel(name);
            }
        }
        void ClosePanel(string panelName)
        {
            if (uiPanelDic.ContainsKey(panelName))
            {
                uiPanelDic[panelName].Close();
                uiPanelDic.Remove(panelName);
            }
        }

        /// ========================================================================
        /// Event
        /// ========================================================================
        protected virtual void OnEvent(string commandLink)
        {
            var command = new CommandLink(commandLink);
            Debug.Log($"<color=cyan>[UuIiView] CommandLink {commandLink}\n{command.Log()}</color>");
            OnEvent(command);
        }

        protected virtual void OnEvent(CommandLink command)
        {

        }
    }

    public class CommandLink
    {
        public string Id;
        public string EventName;
        public UuIiView.EventType EventType;
        public bool IsOn;
        public string PanelName;
        public Dictionary<string, string> param;

        public CommandLink(string commandLink)
        {
            var arr = commandLink.Split("/");
            PanelName = arr[0];
            EventName = arr[1];
            EventType = (UuIiView.EventType)Enum.Parse(typeof(UuIiView.EventType), arr[2]);
            IsOn = bool.Parse(arr[3]);
            Id = (arr.Length >= 5) ? arr[4] : "";
            param = new Dictionary<string, string>();
            for (int i = 5; i < arr.Length; i++)
            {
                var sep = arr[i].Split("=");
                param[sep[0]] = sep[1];
            }
        }

        public string Log()
        {
            var paramStr = "";
            foreach (var kv in param)
            {
                paramStr += $",{kv.Key}={kv.Value}";
            }
            if (paramStr.Length > 0)
                paramStr = paramStr.Substring(1);

            return ($"Id={Id} : PanelName={PanelName} : EventName={EventName} : EventType={EventType} : IsOn={IsOn} : param=({paramStr})");
        }
    }
}
