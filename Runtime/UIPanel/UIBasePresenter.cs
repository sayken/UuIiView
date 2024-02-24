using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace UuIiView
{
    public class UIBasePresenter : IPresenter
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
}
