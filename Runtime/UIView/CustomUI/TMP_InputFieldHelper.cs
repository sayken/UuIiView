using UnityEngine;
using TMPro;

namespace UuIiView
{
    [RequireComponent(typeof(TMP_InputField))]
    public class TMP_InputFieldHelper : MonoBehaviour
    {
        void Awake()
        {
            var viewRoot = GetComponent<UIViewRoot>();
            if (viewRoot == null)
            {
                viewRoot = gameObject.GetComponentInParent<UIViewRoot>();
            }

            var inputField = GetComponent<TMP_InputField>();

            inputField.onValueChanged.AddListener((_) => viewRoot.InputEvent(gameObject.name, EventType.InputFieldValueChanged, _));
            inputField.onEndEdit.AddListener((_) => viewRoot.InputEvent(gameObject.name, EventType.InputFieldEndEdit, _));
        }
    }
}