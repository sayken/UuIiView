using System.Collections.Generic;
using UnityEngine;
using System;

namespace UuIiView
{
    [RequireComponent(typeof(UILayer))]
    public class Dispatcher : MonoBehaviour
    {
        // 全Presenterのインスタンスを保持
        public Dictionary<string, IPresenter> presenters { get; private set; }= new ();

        /// <summary>
        /// Presenterをセットする
        /// </summary>
        /// <param name="panelName"></param>
        /// <param name="type"></param>
        /// <param name="model"></param>
        public void SetPresenter(string panelName, Type type, IModel model)
        {
            IPresenter obj = (IPresenter)Activator.CreateInstance(type, UILayer.Inst.Dispatcher, panelName, model);
            presenters.Add(panelName, obj);
        }

        /// <summary>
        /// Presenterを取得する
        /// </summary>
        /// <param name="panelName">取得したpresenterの名前</param>
        /// <returns>指定したpresenter（見つからない場合はnull）</returns>
        public IPresenter GetPresenter(string panelName)
        {
            return presenters.ContainsKey(panelName) ? presenters[panelName] : null;
        }

        /// <summary>
        /// 全てのEventを受け取って、処理対象のPresenterに処理を渡す
        /// </summary>
        /// <param name="cmd"></param>
        public void Dispatch(CommandLink cmd)
        {
            Debug.Log(cmd.Log());
            
            presenters[cmd.PanelName].OnEvent(cmd);
        }
    }
}