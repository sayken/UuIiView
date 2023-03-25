using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UuIiView
{
    [CreateAssetMenu(menuName = "UuIiView/ViewTest")]
    public class ViewTest : ScriptableObject
    {
        public List<ViewTestData> testDatas;
    }

    [Serializable]
    public class ViewTestData
    {
        public GameObject uiPanel;
        public List<TextAsset> uiViewTestJsons;
    }
}