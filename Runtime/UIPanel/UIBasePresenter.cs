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
        protected UIPanel uiPanel;

        public UIBasePresenter(IDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        /// ========================================================================
        /// Open
        /// ========================================================================
        protected UIPanel Open(string name, Action onOpen = null)
        {
            uiPanel = UILayer.Inst.AddPanel(name);
            uiPanel.OnOpen = onOpen;
            return uiPanel.Open(PassToDispatcher);
        }

        protected void Close(string panelName, Action onClose = null)
        {
            uiPanel.OnClose = onClose;
            uiPanel.Close();
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
