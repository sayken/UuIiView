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
        void Awake()
        {
            var viewRoot = GetComponent<UIViewRoot>();
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
                    viewRoot.ViewEvent(gameObject.name, EventType.CustomToggle, IsOn);
                };
            }
            else
            {
                onClickEvent = () =>
                {
                    IsOn = !IsOn;
                    viewRoot.ViewEvent(gameObject.name, EventType.CustomToggle, IsOn);
                };
            }
            onLongTapEvent = () => { };

            if (Anim != null)
            {
                containsParam = Anim.parameters.Select(_ => _.name).ToList();
            }
        }
    }
}
