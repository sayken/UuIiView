using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace UuIiView
{
    public class UIBasePresenter
    {
        IDispatcher dispatcher;

        protected Dictionary<string, UIPanel> uiPanelDic = new Dictionary<string, UIPanel>();

        public UIBasePresenter(IDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

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
        protected UIPanel Open(string name, Action onOpen = null)
        {
            uiPanelDic[name] = UILayer.Inst.AddPanel(name);
            uiPanelDic[name].OnOpen = onOpen;
            return uiPanelDic[name].Open(PassToDispatcher);
        }

        protected void Close(string name, Action onClose = null)
        {
            var uiGroup = UILayer.Inst.GetPanelGroup(name);
            if (uiGroup != null)
            {
                //uiGroup.panelNames.ForEach(_ => ClosePanel(_, null));
                bool first = true;
                foreach ( var _ in uiGroup.panelNames )
                {
                    ClosePanel(_, (first?onClose:null));
                    first = false;
                }
            }
            else
            {
                ClosePanel(name, onClose);
            }
        }
        void ClosePanel(string panelName, Action onClose)
        {
            if (uiPanelDic.ContainsKey(panelName))
            {
                uiPanelDic[panelName].OnClose = onClose;
                uiPanelDic[panelName].Close();
                uiPanelDic.Remove(panelName);
            }
        }

        void PassToDispatcher(string path) => PassToDispatcher(new CommandLink(path));

        protected void PassToDispatcher(CommandLink cmd) => dispatcher.Dispatch(cmd);

        /// ========================================================================
        /// Event
        /// ========================================================================
        public virtual void OnEvent(string commandLink)
        {
            OnEvent(new CommandLink(commandLink));
        }

        public virtual void OnEvent(CommandLink command)
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

            return ($"<color=cyan>[UuIiView] CommandLink (Id = {Id} : PanelName={PanelName} : EventName={EventName} : EventType={EventType} : IsOn={IsOn} : param=({paramStr})</color>");
        }
    }
}
