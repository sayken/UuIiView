using UnityEngine;

namespace UuIiView
{
    /// <summary>
    /// UI要素にデータを設定するための基底クラス
    /// IUIPresetが実装されている場合はそちらを優先して使用する
    /// </summary>
    public class UISetter : MonoBehaviour
    {
        private IUIPreset uiPreset;

        /// <summary>
        /// IUIPresetコンポーネントへの参照（遅延取得）
        /// </summary>
        private IUIPreset UIPreset => uiPreset ??= GetComponent<IUIPreset>();

        /// <summary>
        /// オブジェクトをUIに設定する
        /// IUIPresetが存在する場合はPresetメソッドを使用し、
        /// 存在しない場合はSetメソッドを使用する
        /// </summary>
        /// <param name="obj">設定するデータ</param>
        public void SetObj(object obj)
        {
            if (UIPreset != null)
            {
                UIPreset.Preset(obj);
            }
            else
            {
                Set(obj);
            }
        }

        /// <summary>
        /// データをUIに反映する（サブクラスでオーバーライド）
        /// </summary>
        /// <param name="obj">設定するデータ</param>
        public virtual void Set(object obj)
        {
            if (obj != null)
            {
                Debug.Log($"[UISetter] {gameObject.name}: {obj}");
            }
        }
    }
}