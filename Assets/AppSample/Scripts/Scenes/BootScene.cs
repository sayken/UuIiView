using UnityEngine;
using UuIiView;
using System;
using UnityEngine.SceneManagement;

namespace UuIiView.Sample
{
    public class BootScene : MonoBehaviour
    {
        [SerializeField] UIPanelData uiPanelData;
        void Start()
        {
            // === UILayer生成
            UILayer.Inst.Initialize(uiPanelData);

            // === マスターテーブルの作成
            var master = new MasterModel();
            master.Init();

            // === Modelクラス群の準備
            Model model = new Model();
            model.Add(master);
            // model.Add(new UserModel(master));
            // model.Add(new CoordinateModel());
            // model.Add(new HistoryModel());
            // model.Add(new MissionModel());
            // model.Add(new PostModel());
            // model.Add(new UserInfoModel());
            // model.Add(new ShopModel());
            // model.Add(new PurchaseModel(model.Get<UserModel>(), master));
            // model.Add(new StageListModel());


            // === Presenterを生成してRouterに登録する
            foreach ( string panelName in UILayer.Inst.GetPanelNames())
            {
                // 専用PresenterのTypeを取得（ない場合はDefaultPresenterを使用）
                var type = Type.GetType(panelName + "Presenter") ?? Type.GetType("DefaultPresenter");

                // Presenter登録
                UILayer.Inst.Router.SetPresenter(panelName, type, model);
            }

            // === その他、起動時にやりたい処理
            {
                Application.targetFrameRate = 30;

            }

            // === 準備完了したので、ゲーム画面開始
            SceneManager.LoadScene("OutGameScene");
        }
    }
}