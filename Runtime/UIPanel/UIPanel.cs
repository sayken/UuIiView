using System;
using UnityEngine;

namespace UuIiView
{
    [RequireComponent(typeof(UIViewRoot))]
    public class UIPanel : MonoBehaviour
    {
        UIViewRoot _vm;
        public UIViewRoot vm => _vm ??= GetComponent<UIViewRoot>();
        ITransition transition;

        public Action OnOpen;
        public Action OnClose;

        public void SetTransition(ITransition transition) => this.transition = transition;

        public bool isOpened { get; private set; } = false;

        public UIPanel Open(object d)
        {
            Open(d, null);
            return this;
        }
        public UIPanel Open(Action<string> onEvent)
        {
            Open(null, onEvent);
            return this;
        }
        public UIPanel Open(object d, Action<string> onEvent)
        {
            isOpened = false;

            UILayer.Inst.TapLock(true);

            vm.SetData(d);
            if (onEvent != null) vm.SetReceiver(onEvent);

            UILayer.Inst.SortPanel();
            transition = GetComponent<ITransition>();
            if (transition != null)
            {
                transition.TransitionIn(() => OpenCompleted(this.OnOpen));
            }
            else
            {
                OpenCompleted(OnOpen);
            }
            return this;
        }

        void OpenCompleted(Action onOpen)
        {
            onOpen?.Invoke();
            UILayer.Inst.TapLock(false);
            isOpened = true;
        }

        public void UpdateData(object o) => vm.SetData(o);

        public void Close(bool forceDestroy = false)
        {
            UILayer.Inst.TapLock(true);

            //var transition = GetComponent<Transition>();
            if (transition != null)
            {
                transition.TransitionOut( () => CloseCompleted(forceDestroy) );
            }
            else
            {
                CloseCompleted(forceDestroy);
            }
        }

        void CloseCompleted(bool forceDestroy)
        {
            OnClose?.Invoke();
            UILayer.Inst.TapLock(false);

            bool needDestroy = UILayer.Inst.Close(gameObject.name, forceDestroy);
            if (needDestroy)
            {
                Destroy(gameObject);
            }
            else
            {
                UILayer.Inst.SortPanel();
            }
        }

        void OnDestroy()
        {
            UILayer.Inst.SortPanel();
        }

        public void Back()
        {

        }

        Action onTapBlind;

        public void OnTapBlind()
        {
            onTapBlind?.Invoke();
            Close();
        }
    }
}
