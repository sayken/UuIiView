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

        public void SetTransition(ITransition transition) => this.transition = transition;

        public bool isOpened { get; private set; } = false;

        public UIPanel Open(object d)
        {
            Open(d, null, null);
            return this;
        }
        public UIPanel Open(Action<string> onEvent)
        {
            Open(null, onEvent, null);
            return this;
        }
        public UIPanel Open(object d, Action<string> onEvent)
        {
            Open(d, onEvent, null);
            return this;
        }

        public void Open(object d, Action<string> onEvent, Action onOpen)
        {
            isOpened = false;

            UILayer.Inst.TapLock(true);

            vm.SetData(d);
            if (onEvent != null) vm.SetReceiver(onEvent);

            UILayer.Inst.SortPanel();
            transition = GetComponent<ITransition>();
            if (transition != null)
            {
                transition.TransitionIn(() => OpenCompleted(onOpen));
            }
            else
            {
                OpenCompleted(onOpen);
            }
        }

        void OpenCompleted(Action onOpen)
        {
            onOpen?.Invoke();
            UILayer.Inst.TapLock(false);
            isOpened = true;
        }

        public void UpdateData(object o) => vm.SetData(o);

        public void Close(bool forceDestroy = false) => Close(null, forceDestroy);

        public void Close(Action onClose, bool forceDestroy)
        {
            UILayer.Inst.TapLock(true);

            //var transition = GetComponent<Transition>();
            if (transition != null)
            {
                transition.TransitionOut( () => CloseCompleted(onClose, forceDestroy) );
            }
            else
            {
                CloseCompleted(onClose, forceDestroy);
            }
        }

        void CloseCompleted(Action onClose, bool forceDestroy)
        {
            onClose?.Invoke();
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