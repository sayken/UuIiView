using System;
using UuIiView;


public class DefaultPresenter : ReactivePresenter
{
    public DefaultPresenter(Router router,string panelName, Model model) : base(router, panelName, model)
    {
    }
    
    protected override void GetInitData(CommandLink commandLink, Action<string> onCompleted)
    {
        onCompleted.Invoke("{}");
        // UnityEngine.Debug.Log($"[{uiPanel.name}] DefaultPresenter");
    }

    public override void OnEvent(CommandLink command)
    {
        base.OnEvent(command);
    }
}
