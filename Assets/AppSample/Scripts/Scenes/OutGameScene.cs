using System.Net.Security;
using UnityEngine;

namespace UuIiView.Sample
{
    public class OutGameScene : MonoBehaviour
    {
        void Start()
        {
            // 最初の画面を表示
            UILayer.Inst.Router.Routing(CommandLink.CreateOpen(ePanelName.Home));
        }
    }
}