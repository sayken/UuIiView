using System;
using UniRx;

namespace UuIiView
{
    public abstract class ReactivePresenter : UIPresenter
    {
        protected ViewModel viewModel;
        public ViewModel ViewModel => viewModel;
        CompositeDisposable disposable = new ();

        public ReactivePresenter(Dispatcher dispatcher, string panelName, Model model) : base(dispatcher, panelName, model)
        {
            viewModel = new ViewModel();
        }

        protected override UIPanel Open(Action onOpen = null)
        {
            base.Open(onOpen);
            uiPanel.OnClose = ()=>{ClearBind();};
            return uiPanel;
        }

        protected override void Close(Action onClose = null)
        {
            base.Close(onClose);
            ClearBind();
        }

        public override void OnEvent(CommandLink commandLink)
        {
            switch( commandLink.ActionType )
            {
                case UuIiView.ActionType.Open:
                    Open();
                    Bind();
                    viewModel.Init(GetInitData(commandLink));
                    break;
                case UuIiView.ActionType.Close:
                    Close();
                    
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
                viewModel.SyncListItem(commandLink.ParentName, commandLink.Id, commandLink.EventName, val);
            }
            else if ( string.IsNullOrEmpty(commandLink.Id) )
            {
                viewModel.Sync(commandLink.EventName, val);
            }
        }
        
        protected void Bind()
        {
            viewModel.Data.Subscribe( data => { if ( data!=null ) uiPanel.UpdateData(data); } ).AddTo(disposable);
        }
        protected void ClearBind()
        {
            disposable.Dispose();
            disposable.Clear();
        }
    }
}
