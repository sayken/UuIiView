using System;

namespace UuIiView
{
    public abstract class UIPresenter : IPresenter
    {
        Dispatcher dispatcher;
        protected string PanelName;
        protected UIPanel uiPanel;

        protected Model model;

        // protected Action onPanelOpen;
        // protected Action onPanelClose;
        protected Action onOpen;
        protected Action onClose;

        public UIPresenter(Dispatcher dispatcher, string panelName, Model model)
        {
            this.dispatcher = dispatcher;
            PanelName = panelName;
            this.model = model;
        }

        /// ========================================================================
        /// Open, Close
        /// ========================================================================
        protected virtual UIPanel Open(Action onPanelOpen = null, Action onPanelClose = null)
        {
            if ( uiPanel == null )
            {
                uiPanel = UILayer.Inst.AddPanel(PanelName);
            }
            uiPanel.OnOpen = onPanelOpen;
            uiPanel.OnClose = onPanelClose;

            onOpen?.Invoke();
            return uiPanel.Open(PassToDispatcher);
        }

        protected virtual void Close()
        {
            onClose?.Invoke();
            if ( uiPanel != null)
            {
                uiPanel.Close();
            }
        }


        /// ========================================================================
        /// Pass CommandLink to Dispatcher
        /// ========================================================================

        void PassToDispatcher(string path) => PassToDispatcher(new CommandLink(path));

        protected void PassToDispatcher(CommandLink cmd) => dispatcher.Dispatch(cmd);

        protected IPresenter GetPresenter(string name) => dispatcher.GetPresenter(name);

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
                    GetInitData(commandLink, (json)=>{
                        uiPanel.UpdateData(json);
                    });
                    break;
                case UuIiView.ActionType.Close:
                    Close();
                    break;
                default:
                    break;
            }
        }

        protected virtual void GetInitData(CommandLink commandLink, Action<string> onCompleted)
        {
            onCompleted.Invoke("{}");
        }
    }
}
