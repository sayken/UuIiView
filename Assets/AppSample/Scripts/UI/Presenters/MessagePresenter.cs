using System;
using UnityEngine;
using UuIiView;


public class MessagePresenter : ReactivePresenter
{
    public MessagePresenter(Router router,string panelName, Model model) : base(router, panelName, model)
    {
    }
    
    protected override void GetInitData(CommandLink commandLink, Action<string> onCompleted)
    {
        var ta = Resources.Load<TextAsset>("Json/View/message");
        onCompleted.Invoke(ta.text);
    }

    public override void OnEvent(CommandLink command)
    {
        base.OnEvent(command);
    }
}
