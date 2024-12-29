using System;
using UnityEngine;
using UuIiView;


public class HomePresenter : ReactivePresenter
{
    public HomePresenter(Router router,string panelName, Model model) : base(router, panelName, model)
    {
    }
    
    protected override void GetInitData(CommandLink commandLink, Action<string> onCompleted)
    {
        var ta = Resources.Load<TextAsset>("Json/View/home");
        onCompleted.Invoke(ta.text);
    }

    public override void OnEvent(CommandLink command)
    {
        base.OnEvent(command);
    }
}
