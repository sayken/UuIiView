using System;
using UniRx;

namespace UuIiView
{
    public abstract class UIPresenter : IPresenter
    {
        Dispatcher dispatcher;
        protected string PanelName;
        protected UIPanel uiPanel;

        protected Model model;

        public UIPresenter(Dispatcher dispatcher, string panelName, Model model)
        {
            this.dispatcher = dispatcher;
            PanelName = panelName;
            this.model = model;
        }

        /// ========================================================================
        /// Open, Close
        /// ========================================================================
        protected virtual UIPanel Open(Action onOpen = null)
        {
            if ( uiPanel == null )
            {
                uiPanel = UILayer.Inst.AddPanel(PanelName);
            }
            uiPanel.OnOpen = onOpen;
            return uiPanel.Open(PassToDispatcher);
        }

        protected virtual void Close(Action onClose = null)
        {
            onClose?.Invoke();
            uiPanel.Close();
        }


        /// ========================================================================
        /// Pass CommandLink to Dispatcher
        /// ========================================================================

        void PassToDispatcher(string path) => PassToDispatcher(new CommandLink(path));

        protected void PassToDispatcher(CommandLink cmd) => dispatcher.Dispatch(cmd);


        /// ========================================================================
        /// Event
        /// ========================================================================
        public virtual void OnEvent(string commandLink)
        {
            OnEvent(new CommandLink(commandLink));
        }
        public virtual void OnEvent(CommandLink commandLink)
        {
            switch( commandLink.ActionType )
            {
                case UuIiView.ActionType.Open:
                    Open();
                    uiPanel.UpdateData(GetInitData(commandLink));
                    break;
                case UuIiView.ActionType.Close:
                    Close();
                    break;
                default:
                    break;
            }
        }

        protected virtual string GetInitData(CommandLink commandLink)
        {
            return "{}";
        }
    }
}
