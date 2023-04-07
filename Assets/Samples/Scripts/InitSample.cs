using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UuIiView.Sample
{
    public class InitSample : MonoBehaviour
    {
        [SerializeField] TextAsset testData;

        void Start()
        {
            var samplePresenter = new SamplePresenter();
            samplePresenter.StartPanel();
            samplePresenter.SetTestData(testData.text);
        }
    }
}
