using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Linq;

namespace UuIiView
{
    [RequireComponent(typeof(Animator))]
    public class CustomButton : UIEvent, IUICustom, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] public ActionType actionType;
        [HideInInspector] public bool interactable;
        [HideInInspector] public string targetPanelName = string.Empty;
        [HideInInspector] public string parentName = string.Empty;
        [HideInInspector] public string closeGroupName = string.Empty;

        public string ParentName
        {
            get => parentName;
            set => parentName = value;
        }

        public string TargetPanelName
        {
            get => targetPanelName;
            set => targetPanelName = value;
        }

        public bool Interactable
        {
            get
            {
                return interactable;
            }
            set
            {
                SetInteractable(value);

                if ( InteractableWithDisable )
                {
                    SetDisabled(!value);
                }
            }
        }
        void SetInteractable(bool value)
        {
            interactable = value;
            tapArea.raycastTarget = value;
        }
        [HideInInspector] public bool selected;
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

        [HideInInspector] public bool disabled;
        public bool Disabled
        {
            get
            {
                return disabled;
            }
            set
            {
                SetDisabled(value);

                if ( InteractableWithDisable )
                {
                    SetInteractable(!value);
                }
            }
        }
        void SetDisabled(bool value)
        {
            disabled = value;
            OnDisabled();
        }
        [SerializeField] bool InteractableWithDisable = true;

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
                if ( anim == null || anim.runtimeAnimatorController == null )
                {
                    return null;
                }
                return anim;
            }
        }

        Coroutine longtap;
        bool isLongTap = false;

        protected List<string> containsParam = new List<string>();

        void Awake()
        {
            var viewRoot = GetComponent<UIViewRoot>();
            if (viewRoot == null)
            {
                viewRoot = gameObject.GetComponentInParent<UIViewRoot>();
            }

            onClickEvent = () =>
            {
                if ((actionType == ActionType.Open || actionType == ActionType.CloseAndOpen || actionType == ActionType.CloseGroupAndOpen) && !string.IsNullOrEmpty(targetPanelName))
                {
                    // Debug.Log("CustomButton "+ actionType +" / "+ closeGroupName);
                    if ( actionType == ActionType.CloseAndOpen )
                    {
                        viewRoot.ReceiveEvent(gameObject.name, EventType.Button, ActionType.Close, parentName);
                    }
                    else if ( actionType == ActionType.CloseGroupAndOpen)
                    {
                        viewRoot.ReceiveEvent(closeGroupName, gameObject.name, EventType.Button, ActionType.Close, parentName);
                    }
                    viewRoot.ReceiveEvent(targetPanelName, gameObject.name, EventType.Button, ActionType.Open, parentName);
                }
                else
                {
                    viewRoot.ReceiveEvent(gameObject.name, EventType.Button, actionType, parentName);
                }
            };
            onLongTapEvent = () => viewRoot.ReceiveEvent(gameObject.name, EventType.LongTap, actionType, parentName);

            if (Anim != null )
            {
                containsParam = Anim.parameters.Select(_ => _.name).ToList();
            }
        }
        void OnEnable()
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
            if (Anim != null && containsParam.Contains(animName) )
            {
                Anim.SetBool(animName, flag);
            }
        }
    }
}
