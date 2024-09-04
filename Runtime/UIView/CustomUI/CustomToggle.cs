using UnityEngine;
using System.Linq;

namespace UuIiView
{
    public class CustomToggle : CustomButton
    {
        public CustomToggleGroup toggleGroup;
        [HideInInspector] public bool isOn;

        UIViewRoot viewRoot;
        public bool IsOn
        {
            get
            {
                return isOn;
            }
            set
            {
                isOn = value;
                Selected = isOn;
                OnSelected();
            }
        }
        void Awake()
        {
            viewRoot = GetComponent<UIViewRoot>();
            if (viewRoot == null)
            {
                viewRoot = gameObject.GetComponentInParent<UIViewRoot>();
            }

            toggleGroup = gameObject.GetComponentInParent<CustomToggleGroup>();

            if (toggleGroup != null)
            {
                toggleGroup.customToggles.Add(this);
                onClickEvent = () =>
                {
                    toggleGroup.On(this, !IsOn);
                    // toggleGroupがある時は、toggleGroup.On経由で TriggerEventが呼ばれるので、ここでは何もしない
                };
            }
            else
            {
                onClickEvent = () =>
                {
                    IsOn = !IsOn;
                    viewRoot.ReceiveEvent(viewRoot.gameObject.name, gameObject.name, EventType.Toggle, actionType, parentName, IsOn);
                };
            }
            onLongTapEvent = () => { };

            if (Anim != null)
            {
                containsParam = Anim.parameters.Select(_ => _.name).ToList();
            }
        }

        public void TriggerEvent()
        {
            viewRoot.ReceiveEvent(viewRoot.gameObject.name, gameObject.name, EventType.Toggle, actionType, parentName, IsOn);
        }
    }
}
