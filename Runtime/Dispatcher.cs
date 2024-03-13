using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

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