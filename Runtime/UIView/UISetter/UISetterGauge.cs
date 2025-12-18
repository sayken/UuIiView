using UnityEngine;
using UnityEngine.UI;

namespace UuIiView
{
    public class UISetterGauge : UISetter
    {
        Image image;
        Image Image
        {
            get
            {
                image = image ?? GetComponent<Image>();
                return image;
            }
        }

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
