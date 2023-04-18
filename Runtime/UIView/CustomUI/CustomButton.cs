using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace UuIiView
{
    [RequireComponent(typeof(Animator))]
    public class CustomButton : UIEvent, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [HideInInspector] public bool interactable;
        public bool Interactable
        {
            get
            {
                return interactable;
            }
            set
            {
                interactable = value;
                tapArea.raycastTarget = value;
            }
        }
        public bool selected;
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                OnSelected();
            }
        }

        public bool disabled;
        public bool Disabled
        {
            get
            {
                return disabled;
            }
            set
            {
                disabled = value;
                OnDisabled();
            }
        }

        public bool InteractableWith
        {
            set
            {
                Interactable = value;
                Disabled = value;
            }
        }

        [SerializeField] Image tapArea;

        WaitForSeconds waitForLongTap = new WaitForSeconds(1.5f);

        protected Action onClickEvent;
        protected Action onLongTapEvent;

        Animator anim;
        public Animator Anim
        {
            get
            {
                anim ??= GetComponent<Animator>();
                return anim;
            }
        }

        Coroutine longtap;
        bool isLongTap = false;

        void Awake()
        {
            var viewRoot = GetComponent<UIViewRoot>();
            if (viewRoot == null)
            {
                viewRoot = gameObject.GetComponentInParent<UIViewRoot>();
            }

            onClickEvent = () => viewRoot.ViewEvent(gameObject.name, EventType.CustomButton);
            onLongTapEvent = () => viewRoot.ViewEvent(gameObject.name, EventType.CustomLongTap);
        }
        void Start()
        {
            OnSelected();
            OnDisabled();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isLongTap && Interactable) onClickEvent?.Invoke();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (Interactable == false) return;
            OnPressed();
            longtap = StartCoroutine(LongTap());
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (Interactable == false) return;
            OnReleased();
            StopCoroutine(longtap);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (longtap != null) StopCoroutine(longtap);
        }

        IEnumerator LongTap()
        {
            isLongTap = false;
            yield return waitForLongTap;
            if (onLongTapEvent != null)
            {
                isLongTap = true;
                onLongTapEvent.Invoke();
            }
        }

        protected virtual void OnPressed()
        {
            PlayAnim("Pressed", true);
        }
        protected virtual void OnReleased()
        {
            PlayAnim("Pressed", false);
        }
        protected virtual void OnSelected()
        {
            PlayAnim("Selected", selected);
        }
        protected virtual void OnDisabled()
        {
            PlayAnim("Disabled", disabled);
        }

        void PlayAnim(string animName, bool flag)
        {
            if (Anim != null)
            {
                Anim.SetBool(animName, flag);
            }
        }
    }
}
