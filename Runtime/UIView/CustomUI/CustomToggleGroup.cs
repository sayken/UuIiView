using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UuIiView
{
    /// <summary>
    /// カスタムトグルグループコンポーネント
    /// 複数のCustomToggleを管理し、排他選択やマルチ選択の制御を行う
    /// </summary>
    public class CustomToggleGroup : MonoBehaviour
    {
        /// <summary>グループに所属するトグルのリスト</summary>
        public List<CustomToggle> customToggles = new List<CustomToggle>();

        /// <summary>すべてのトグルをOFFにすることを許可するか</summary>
        [SerializeField]
        [Tooltip("すべてのトグルをOFFにすることを許可する")]
        private bool allowSwitchOff = false;

        /// <summary>複数選択を許可するか</summary>
        [SerializeField]
        [Tooltip("複数のトグルを同時にONにすることを許可する")]
        private bool allowMultiSelect = true;

        /// <summary>同時にONにできるトグルの最大数</summary>
        [SerializeField]
        [Tooltip("複数選択時に同時にONにできる最大数")]
        private int allowMaxSelect = 1;

        /// <summary>
        /// 初期化処理
        /// 初期状態でONのトグルがない場合、必要に応じて最初のトグルをONにする
        /// </summary>
        private void Start()
        {
            if (!customToggles.Any(_ => _.isOn) && customToggles.Count > 0 && !allowSwitchOff)
            {
                customToggles[0].IsOn = true;
            }
        }

        /// <summary>
        /// トグルのON/OFF状態を変更する
        /// 排他選択・マルチ選択・最大選択数の制約を適用
        /// </summary>
        /// <param name="toggle">状態を変更するトグル</param>
        /// <param name="isOn">新しい状態（true=ON, false=OFF）</param>
        public void On(CustomToggle toggle, bool isOn)
        {
            int onCount = customToggles.Count(_ => _.isOn);

            if (toggle.IsOn == isOn) return;

            if (isOn)
            {
                // 排他選択モード：他のトグルをすべてOFFにする
                if (!allowMultiSelect)
                {
                    foreach (var tgl in customToggles)
                    {
                        bool newState = (tgl == toggle);
                        // 状態が変わるトグルのみイベントを発火
                        if (tgl.IsOn != newState)
                        {
                            tgl.IsOn = newState;
                            tgl.TriggerEvent();
                        }
                    }
                }
                // マルチ選択モード：最大選択数以内ならONにする
                else if (allowMaxSelect > onCount)
                {
                    toggle.IsOn = true;
                    toggle.TriggerEvent();
                }
            }
            else if (!isOn && (onCount > 1 || allowSwitchOff))
            {
                // OFFにする（最低1つは残すか、allowSwitchOffがtrueの場合）
                toggle.IsOn = false;
                toggle.TriggerEvent();
            }
        }

        /// <summary>
        /// 指定インデックスのトグルを選択状態にする
        /// </summary>
        /// <param name="idx">選択するトグルのインデックス（無効な場合は0番目を選択）</param>
        public void SelectToggle(int idx = 0)
        {
            if (customToggles.Count == 0)
            {
                Debug.LogWarning($"[CustomToggleGroup] {gameObject.name}: customTogglesが空です。");
                return;
            }

            if (idx < 0 || customToggles.Count <= idx)
            {
                idx = 0;
            }
            On(customToggles[idx], true);
        }
    }
}
