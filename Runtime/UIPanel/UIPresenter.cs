using System;

namespace UuIiView
{
    public class UIPresenter : IPresenter
    {
        Dispatcher dispatcher;
        protected string PanelName;
        protected UIPanel uiPanel;

        public UIPresenter(Dispatcher dispatcher, string panelName, IModel model)
        {
            this.dispatcher = dispatcher;
            PanelName = panelName;
        }

        /// ========================================================================
        /// Open, Close
        /// ========================================================================
        protected UIPanel Open(Action onOpen = null)
        {
            uiPanel = UILayer.Inst.AddPanel(PanelName);
            uiPanel.OnOpen = onOpen;
            return uiPanel.Open(PassToDispatcher);
        }

        protected void Close(Action onClose = null)
        {
            uiPanel.OnClose = onClose;
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
        }
    }
}
