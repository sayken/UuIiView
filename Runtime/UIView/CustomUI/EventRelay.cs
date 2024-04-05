using UnityEngine;
using UnityEngine.UI;

namespace UuIiView
{
    public class EventRelay : MonoBehaviour
    {
        [SerializeField] public ActionType actionType;
        [HideInInspector] public string targetPanelName = string.Empty;
        [HideInInspector] public string parentName = string.Empty;

        public string ParentName
        {
            get => parentName;
            set => parentName = value;
        }

        UIViewRoot viewRoot;
        void Awake()
        {
            viewRoot = GetComponent<UIViewRoot>();
            if (viewRoot == null)
            {
                viewRoot = gameObject.GetComponentInParent<UIViewRoot>();
            }

            var component = GetComponent<Selectable>();
            if ( component.GetType() == typeof(TMPro.TMP_InputField) )
            {
                var inputField = (TMPro.TMP_InputField)component;

                inputField.onValueChanged.AddListener((input)
                    => viewRoot.ViewEvent(viewRoot.name, gameObject.name, EventType.Input, ActionType.DataSync, parentName, input, true));
                inputField.onEndEdit.AddListener((input)
                    => viewRoot.ViewEvent(viewRoot.name, gameObject.name, EventType.Input, ActionType.DataSync, parentName, input, true));
            }
            else if( component.GetType() == typeof(Button) )
            {
                var button = (Button)component;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(()=>ViewEvent(EventType.Button));
            }
            else if( component.GetType() == typeof(Toggle) )
            {
                var button = (Toggle)component;
                button.onValueChanged.RemoveAllListeners();
                button.onValueChanged.AddListener((isOn)=>ViewEvent(EventType.Toggle, isOn));
            }
            else if( component.GetType() == typeof(Slider) )
            {
                var slider = (Slider)component;
                slider.onValueChanged.AddListener(val
                    => viewRoot.ViewEvent(viewRoot.name, gameObject.name, EventType.Slider, ActionType.DataSync, parentName, val, true));
            }
        }

        void ViewEvent(EventType eventType, bool isOn = true)
        {
            if ((actionType == ActionType.Open || actionType == ActionType.CloseAndOpen) && !string.IsNullOrEmpty(targetPanelName))
            {
                if ( actionType == ActionType.CloseAndOpen )
                {
                    viewRoot.ViewEvent(gameObject.name, eventType, ActionType.Close, parentName, isOn);
                }
                viewRoot.ViewEvent(targetPanelName, gameObject.name, eventType, ActionType.Open, parentName, isOn);
            }
            else
            {
                viewRoot.ViewEvent(gameObject.name, eventType, actionType, parentName, isOn);
            }
        }
    }
}