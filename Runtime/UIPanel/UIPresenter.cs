using System;
using UniRx;

namespace UuIiView
{
    public abstract class UIPresenter : IPresenter
    {
        Dispatcher dispatcher;
        protected string PanelName;
        protected UIPanel uiPanel;

        protected Repository repo;
        public Repository Repo => repo;
        CompositeDisposable disposable = new ();

        public UIPresenter(Dispatcher dispatcher, string panelName, IModel model)
        {
            this.dispatcher = dispatcher;
            PanelName = panelName;

            repo = new Repository();
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
            switch( commandLink.ActionType )
            {
                case UuIiView.ActionType.Open:
                    Open();
                    Bind();
                    repo.Init(GetInitData(commandLink));
                    break;
                case UuIiView.ActionType.Close:
                    Close();
                    ClearBind();
                    break;
                case UuIiView.ActionType.DataSync:
                    DataSync(commandLink);
                    break;
                default:
                    break;
            }
        }

        void DataSync(CommandLink commandLink)
        {
            if ( commandLink.EventType == EventType.Slider )
            {
                if ( float.TryParse(commandLink.param["Slider"], out float val) )
                {
                    Sync(commandLink, val);
                }
            }
            else if ( commandLink.EventType == EventType.Toggle )
            {
                if ( bool.TryParse(commandLink.param["Toggle"], out bool val) )
                {
                    Sync(commandLink, val);
                }
            }
            else if ( commandLink.EventType == EventType.Input )
            {
                Sync(commandLink, commandLink.param["Input"]);
            }
        }
        void Sync(CommandLink commandLink, object val)
        {
            if ( !string.IsNullOrEmpty(commandLink.ParentName) )
            {
                repo.SyncListItem(commandLink.ParentName, commandLink.Id, commandLink.EventName, val);
            }
            else if ( string.IsNullOrEmpty(commandLink.Id) )
            {
                repo.Sync(commandLink.EventName, val);
            }
        }

        protected virtual string GetInitData(CommandLink commandLink)
        {
            return "{}";
        }
        
        protected void Bind()
        {
            repo.Data.Subscribe( data => { if ( data!=null ) uiPanel.UpdateData(data); } ).AddTo(disposable);
        }
        protected void ClearBind()
        {
            disposable.Clear();
        }
        }
}
