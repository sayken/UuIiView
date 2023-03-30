using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UuIiView.Sample
{
    public class InitSample : MonoBehaviour
    {
        [SerializeField] UIPanelData uiPanelData;
        [SerializeField] UIPanelGroup uiPanelGroup;

        private void Awake()
        {
            UILayer.Inst.Initialize(uiPanelData, uiPanelGroup);
        }

        void Start()
        {
            var samplePresenter = new SamplePresenter();
            samplePresenter.StartPanel();
        }
    }
}