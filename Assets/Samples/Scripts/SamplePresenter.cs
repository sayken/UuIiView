using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UuIiView.Sample
{
    public class SamplePresenter : UuIiView.UIBasePresenter
    {
        public void StartPanel()
        {
            var uiPanels = OpenGroup("Main");
            Open("QuestList");
            Open("QuestDetail");

        }

    }
}