using UnityEngine;
using UuIiView;

namespace UuIiView.Sample
{
    public class OutGameScene : MonoBehaviour, IScene
    {
        void Start()
        {
            // 最初の画面を表示
            UILayer.Inst.Router.Routing(CommandLink.CreateOpen(ePanelName.Home));
        }
        public void OnEvent(CommandLink commandLink)
        {
            
        }
    }

}