using UnityEngine;
using UnityEngine.UI;

namespace UuIiView
{
    /// <summary>
    /// ゲージ表示を行うUISetter
    /// ImageのfillAmountを制御してプログレスバーやHPゲージ等を表現
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class UISetterGauge : UISetter
    {
        /// <summary>Imageコンポーネント参照（遅延取得）</summary>
        private Image image;

        /// <summary>Imageコンポーネントへの参照</summary>
        private Image Image
        {
            get
            {
                image ??= GetComponent<Image>();
                return image;
            }
        }

        /// <summary>
        /// ゲージの値を設定する
        /// 0.0～1.0の範囲でfillAmountを設定
        /// </summary>
        /// <param name="obj">double/float型または数値に変換可能な文字列</param>
        public override void Set(object obj)
        {
            if (obj == null)
            {
                return;
            }

            if (obj is double value)
            {
                Image.fillAmount = (float)value;
            }
            else if (obj is float floatValue)
            {
                Image.fillAmount = floatValue;
            }
            else if (double.TryParse(obj.ToString(), out double parsedValue))
            {
                Image.fillAmount = (float)parsedValue;
            }
            else
            {
                Debug.LogWarning($"[UISetterGauge] {gameObject.name}: objをdoubleに変換できません。Type={obj.GetType()}");
            }
        }
    }
}
