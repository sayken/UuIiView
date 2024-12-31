using System;
using UnityEngine;
using UuIiView;


public class SettingPresenter : ReactivePresenter
{
    public SettingPresenter(Router router,string panelName, Model model) : base(router, panelName, model)
    {
    }
    
    protected override void GetInitData(CommandLink commandLink, Action<string> onCompleted)
    {
        var ta = Resources.Load<TextAsset>("Json/View/setting");
        onCompleted.Invoke(ta.text);
    }

    public override void OnEvent(CommandLink command)
    {
        base.OnEvent(command);

        if ( command.EventType == UuIiView.EventType.Slider )
        {
            if ( float.TryParse( command.param["Slider"], out float fSlider) )
            {
                var slider = ((int)(fSlider*100f)) / 100f;
                viewModel.UpdateData("SliderValue", slider, true);
            }
        }
    }
}
