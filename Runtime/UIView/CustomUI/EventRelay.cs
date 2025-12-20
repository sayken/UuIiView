using UnityEngine;
using UnityEngine.UI;

namespace UuIiView
{
    /// <summary>
    /// Unity標準UIコンポーネントのイベントをUIViewRootに中継するコンポーネント
    /// Button/Toggle/Slider/TMP_InputFieldに対応
    /// </summary>
    public class EventRelay : MonoBehaviour
    {
        /// <summary>ボタン押下時のアクション種類</summary>
        [SerializeField]
        [Tooltip("イベント発生時に実行するアクションの種類")]
        public ActionType actionType;

        /// <summary>アクション対象のパネル名</summary>
        [HideInInspector] public string targetPanelName = string.Empty;

        /// <summary>親要素の名前（リストアイテム等で使用）</summary>
        [HideInInspector] public string parentName = string.Empty;

        /// <summary>親要素の名前</summary>
        public string ParentName
        {
            get => parentName;
            set => parentName = value;
        }

        /// <summary>親のUIViewRoot参照</summary>
        private UIViewRoot viewRoot;

        /// <summary>
        /// 初期化処理
        /// UIViewRootを取得し、UIコンポーネントの種類に応じてイベントリスナーを設定
        /// </summary>
        private void Awake()
        {
            viewRoot = GetComponent<UIViewRoot>();
            if (viewRoot == null)
            {
                viewRoot = gameObject.GetComponentInParent<UIViewRoot>();
            }

            if (viewRoot == null)
            {
                Debug.LogError($"[EventRelay] {gameObject.name}: UIViewRootが見つかりません。");
                return;
            }

            var component = GetComponent<Selectable>();
            if (component == null)
            {
                Debug.LogError($"[EventRelay] {gameObject.name}: Selectableコンポーネントが見つかりません。");
                return;
            }

            SetupEventListeners(component);
        }

        /// <summary>
        /// UIコンポーネントの種類に応じてイベントリスナーを設定する
        /// </summary>
        /// <param name="component">対象のSelectableコンポーネント</param>
        private void SetupEventListeners(Selectable component)
        {
            var componentType = component.GetType();

            if (componentType == typeof(TMPro.TMP_InputField))
            {
                SetupInputFieldListeners((TMPro.TMP_InputField)component);
            }
            else if (componentType == typeof(Button))
            {
                SetupButtonListeners((Button)component);
            }
            else if (componentType == typeof(Toggle))
            {
                SetupToggleListeners((Toggle)component);
            }
            else if (componentType == typeof(Slider))
            {
                SetupSliderListeners((Slider)component);
            }
        }

        /// <summary>
        /// TMP_InputFieldのイベントリスナーを設定する
        /// </summary>
        /// <param name="inputField">対象のInputField</param>
        private void SetupInputFieldListeners(TMPro.TMP_InputField inputField)
        {
            inputField.onValueChanged.AddListener((input)
                => viewRoot.ReceiveEvent(viewRoot.name, gameObject.name, EventType.Input, ActionType.DataSync, parentName, input, true));
            inputField.onEndEdit.AddListener((input)
                => viewRoot.ReceiveEvent(viewRoot.name, gameObject.name, EventType.Input, ActionType.DataSync, parentName, input, true));
        }

        /// <summary>
        /// Buttonのイベントリスナーを設定する
        /// </summary>
        /// <param name="button">対象のButton</param>
        private void SetupButtonListeners(Button button)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => ViewEvent(EventType.Button));
        }

        /// <summary>
        /// Toggleのイベントリスナーを設定する
        /// </summary>
        /// <param name="toggle">対象のToggle</param>
        private void SetupToggleListeners(Toggle toggle)
        {
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener((isOn) => ViewEvent(EventType.Toggle, isOn));
        }

        /// <summary>
        /// Sliderのイベントリスナーを設定する
        /// </summary>
        /// <param name="slider">対象のSlider</param>
        private void SetupSliderListeners(Slider slider)
        {
            slider.onValueChanged.AddListener(val
                => viewRoot.ReceiveEvent(viewRoot.name, gameObject.name, EventType.Slider, ActionType.DataSync, parentName, val, true));
        }

        /// <summary>
        /// UIViewRootにイベントを通知する
        /// actionTypeに応じてパネルのOpen/Closeを制御
        /// </summary>
        /// <param name="eventType">イベントの種類</param>
        /// <param name="isOn">トグル状態（デフォルトtrue）</param>
        private void ViewEvent(EventType eventType, bool isOn = true)
        {
            if ((actionType == ActionType.Open || actionType == ActionType.CloseAndOpen) && !string.IsNullOrEmpty(targetPanelName))
            {
                if (actionType == ActionType.CloseAndOpen)
                {
                    viewRoot.ReceiveEvent(gameObject.name, eventType, ActionType.Close, parentName, isOn);
                }
                viewRoot.ReceiveEvent(targetPanelName, gameObject.name, eventType, ActionType.Open, parentName, isOn);
            }
            else
            {
                viewRoot.ReceiveEvent(gameObject.name, eventType, actionType, parentName, isOn);
            }
        }
    }
}
