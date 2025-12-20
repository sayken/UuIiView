using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Linq;

namespace UuIiView
{
    /// <summary>
    /// カスタムボタンコンポーネント
    /// 長押し対応、Animator連携、複数アクションタイプをサポートする拡張ボタン
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class CustomButton : UIEvent, IUICustom, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        /// <summary>長押し判定時間（秒）</summary>
        private const float LongTapDuration = 1.5f;

        /// <summary>ボタン押下時のアクション種類</summary>
        [SerializeField]
        [Tooltip("ボタン押下時に実行するアクションの種類")]
        public ActionType actionType;

        [HideInInspector] public bool interactable;
        [HideInInspector] public string targetPanelName = string.Empty;
        [HideInInspector] public string parentName = string.Empty;
        [HideInInspector] public string closeGroupName = string.Empty;

        /// <summary>親要素の名前（リストアイテム等で使用）</summary>
        public string ParentName
        {
            get => parentName;
            set => parentName = value;
        }

        /// <summary>アクション対象のパネル名</summary>
        public string TargetPanelName
        {
            get => targetPanelName;
            set => targetPanelName = value;
        }

        /// <summary>ボタンが操作可能かどうか</summary>
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

        /// <summary>
        /// Interactable状態を設定する（内部処理）
        /// </summary>
        private void SetInteractable(bool value)
        {
            interactable = value;
            if (tapArea != null) tapArea.raycastTarget = value;
        }

        [HideInInspector] public bool selected;

        /// <summary>ボタンが選択状態かどうか</summary>
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

        /// <summary>ボタンが無効状態かどうか</summary>
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

        /// <summary>
        /// Disabled状態を設定する（内部処理）
        /// </summary>
        private void SetDisabled(bool value)
        {
            disabled = value;
            OnDisabled();
        }

        /// <summary>InteractableとDisabledを連動させるかどうか</summary>
        [SerializeField]
        [Tooltip("Interactable変更時にDisabledも連動して変更する")]
        private bool InteractableWithDisable = true;

        /// <summary>タップ判定エリア</summary>
        [SerializeField]
        [Tooltip("タップ判定に使用するImage")]
        private Image tapArea;

        private readonly WaitForSeconds waitForLongTap = new WaitForSeconds(LongTapDuration);

        /// <summary>クリック時のイベント</summary>
        protected Action onClickEvent;
        /// <summary>長押し時のイベント</summary>
        protected Action onLongTapEvent;

        private Animator anim;

        /// <summary>Animatorコンポーネントへの参照（遅延取得）</summary>
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

        private Coroutine longtap;
        private bool isLongTap = false;

        /// <summary>Animatorに存在するパラメータ名リスト</summary>
        protected List<string> containsParam = new List<string>();
        /// <summary>親のUIViewRoot参照</summary>
        protected UIViewRoot viewRoot;

        /// <summary>
        /// 初期化処理
        /// </summary>
        protected virtual void Awake()
        {
            viewRoot = GetComponent<UIViewRoot>();
            if (viewRoot == null)
            {
                viewRoot = gameObject.GetComponentInParent<UIViewRoot>();
            }

            InitializeAnimParams();
            InitializeClickEvent();
            InitializeLongTapEvent();
        }

        /// <summary>
        /// Animatorパラメータを初期化する
        /// </summary>
        protected void InitializeAnimParams()
        {
            if (Anim != null)
            {
                containsParam = Anim.parameters.Select(_ => _.name).ToList();
            }
        }

        /// <summary>
        /// クリックイベントを初期化する
        /// actionTypeに応じてパネルのOpen/Close/Actionを実行
        /// </summary>
        protected virtual void InitializeClickEvent()
        {
            onClickEvent = () =>
            {
                if (viewRoot == null)
                {
                    Debug.LogWarning($"[CustomButton] {gameObject.name}: viewRootが見つかりません。イベントを発火できません。");
                    return;
                }

                if ((actionType == ActionType.Open || actionType == ActionType.CloseAndOpen || actionType == ActionType.CloseGroupAndOpen) && !string.IsNullOrEmpty(targetPanelName))
                {
                    if (actionType == ActionType.CloseAndOpen)
                    {
                        viewRoot.ReceiveEvent(gameObject.name, EventType.Button, ActionType.Close, parentName);
                    }
                    else if (actionType == ActionType.CloseGroupAndOpen)
                    {
                        viewRoot.ReceiveEvent(closeGroupName, gameObject.name, EventType.Button, ActionType.Close, parentName);
                    }
                    viewRoot.ReceiveEvent(targetPanelName, gameObject.name, EventType.Button, ActionType.Open, parentName);
                }
                else if (actionType == ActionType.ActionToPanel && !string.IsNullOrEmpty(targetPanelName))
                {
                    viewRoot.ReceiveEvent(targetPanelName, gameObject.name, EventType.Button, ActionType.Action, parentName);
                }
                else
                {
                    viewRoot.ReceiveEvent(gameObject.name, EventType.Button, actionType, parentName);
                }
            };
        }

        /// <summary>
        /// 長押しイベントを初期化する
        /// </summary>
        protected virtual void InitializeLongTapEvent()
        {
            onLongTapEvent = () =>
            {
                if (viewRoot != null)
                {
                    viewRoot.ReceiveEvent(gameObject.name, EventType.LongTap, actionType, parentName);
                }
            };
        }

        private void OnEnable()
        {
            OnSelected();
            OnDisabled();
        }

        /// <summary>
        /// ポインタークリック時の処理
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isLongTap && Interactable) onClickEvent?.Invoke();
        }

        /// <summary>
        /// ポインター押下時の処理
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (Interactable == false) return;
            OnPressed();
            longtap = StartCoroutine(LongTap());
        }

        /// <summary>
        /// ポインター離し時の処理
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            if (Interactable == false) return;
            OnReleased();
            if (longtap != null) StopCoroutine(longtap);
        }

        /// <summary>
        /// ポインターがエリア外に出た時の処理
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (longtap != null) StopCoroutine(longtap);
        }

        /// <summary>
        /// 長押し判定用コルーチン
        /// </summary>
        private IEnumerator LongTap()
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

        /// <summary>
        /// オブジェクト破棄時の処理
        /// 実行中のコルーチンを停止する
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (longtap != null)
            {
                StopCoroutine(longtap);
                longtap = null;
            }
        }

        /// <summary>
        /// Animatorのパラメータを設定する
        /// </summary>
        /// <param name="animName">パラメータ名</param>
        /// <param name="flag">設定する値</param>
        private void PlayAnim(string animName, bool flag)
        {
            if (Anim != null && containsParam.Contains(animName))
            {
                Anim.SetBool(animName, flag);
            }
        }
    }
}
