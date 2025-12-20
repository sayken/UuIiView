using UnityEngine;

namespace UuIiView
{
    /// <summary>
    /// カスタムトグルコンポーネント
    /// CustomButtonを継承し、ON/OFF状態を持つトグルボタンを実装
    /// CustomToggleGroupと連携して排他選択やマルチ選択に対応
    /// </summary>
    public class CustomToggle : CustomButton
    {
        /// <summary>所属するトグルグループ（親から自動取得）</summary>
        public CustomToggleGroup toggleGroup;

        /// <summary>トグルのON/OFF状態（内部保持用）</summary>
        [HideInInspector] public bool isOn;

        /// <summary>
        /// トグルのON/OFF状態
        /// 設定時にSelected状態とアニメーションも更新される
        /// </summary>
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

        /// <summary>
        /// 初期化処理
        /// 親階層からCustomToggleGroupを取得し、グループに自身を登録する
        /// </summary>
        protected override void Awake()
        {
            toggleGroup = gameObject.GetComponentInParent<CustomToggleGroup>();
            if (toggleGroup != null)
            {
                toggleGroup.customToggles.Add(this);
            }

            base.Awake();
        }

        /// <summary>
        /// クリックイベントを初期化する
        /// トグルグループがある場合はグループ経由でON/OFF制御
        /// グループがない場合は単独でトグル動作
        /// </summary>
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
                    if (viewRoot == null)
                    {
                        Debug.LogWarning($"[CustomToggle] {gameObject.name}: viewRootが見つかりません。イベントを発火できません。");
                        return;
                    }
                    IsOn = !IsOn;
                    viewRoot.ReceiveEvent(gameObject.name, EventType.Toggle, actionType, parentName, IsOn);
                };
            }
        }

        /// <summary>
        /// 長押しイベントを初期化する
        /// トグルでは長押しイベントは使用しない（空実装）
        /// </summary>
        protected override void InitializeLongTapEvent()
        {
            onLongTapEvent = () => { };
        }

        /// <summary>
        /// トグルイベントを発火する
        /// CustomToggleGroupから状態変更時に呼び出される
        /// </summary>
        public void TriggerEvent()
        {
            if (viewRoot == null)
            {
                Debug.LogWarning($"[CustomToggle] {gameObject.name}: viewRootが見つかりません。イベントを発火できません。");
                return;
            }
            viewRoot.ReceiveEvent(gameObject.name, EventType.Toggle, actionType, parentName, IsOn);
        }
    }
}
