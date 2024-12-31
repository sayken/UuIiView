using System;
using UnityEngine;
using UuIiView;


public class TabSelectPresenter : ReactivePresenter
{
    public TabSelectPresenter(Router router,string panelName, Model model) : base(router, panelName, model)
    {
    }
    
    protected override void GetInitData(CommandLink commandLink, Action<string> onCompleted)
    {
        var ta = Resources.Load<TextAsset>("Json/View/tabselect_a");
        onCompleted.Invoke(ta.text);
    }

    public override void OnEvent(CommandLink command)
    {
        base.OnEvent(command);

        if ( command.EventType == UuIiView.EventType.Toggle )
        {
            if ( command.param.TryGetValue("Toggle", out string isOnStr) && bool.TryParse(isOnStr, out bool isOn) && !isOn )
            {
                return;
            }

            if ( Enum.TryParse<eEventName>(command.EventName, out var eventName) )
            {
                string jsonFile = string.Empty;
                switch ( eventName )
                {
                    case eEventName.TabA:
                        jsonFile = "tabselect_a";
                    break;
                    case eEventName.TabB:
                        jsonFile = "tabselect_b";
                    break;
                    case eEventName.TabC:
                        jsonFile = "tabselect_c";
                    break;
                    case eEventName.TabD:
                        jsonFile = "tabselect_d";
                    break;
                    default:
                        jsonFile = "tabselect_a";
                    break;
                }
                
                if ( !string.IsNullOrEmpty(jsonFile) )
                {
                    var ta = Resources.Load<TextAsset>("Json/View/"+ jsonFile);
                    viewModel.Init(ta.text);
                }
            }
        }
    }
}
