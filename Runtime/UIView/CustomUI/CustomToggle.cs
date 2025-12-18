using UnityEngine;
using System.Linq;

namespace UuIiView
{
    public class CustomToggle : CustomButton
    {
        public CustomToggleGroup toggleGroup;
        [HideInInspector] public bool isOn;

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

        protected override void Awake()
        {
            toggleGroup = gameObject.GetComponentInParent<CustomToggleGroup>();
            if (toggleGroup != null)
            {
                toggleGroup.customToggles.Add(this);
            }

            base.Awake();
        }

        protected override void InitializeClickEvent()
        {
            if (toggleGroup != null)
            {
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
                    viewRoot.ReceiveEvent(gameObject.name, EventType.Toggle, actionType, parentName, IsOn);
                };
            }
        }

        protected override void InitializeLongTapEvent()
        {
            onLongTapEvent = () => { };
        }

        public void TriggerEvent()
        {
            viewRoot.ReceiveEvent(gameObject.name, EventType.Toggle, actionType, parentName, IsOn);
        }
    }
}
