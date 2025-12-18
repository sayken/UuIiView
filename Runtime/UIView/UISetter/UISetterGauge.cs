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
            if ( obj.GetType() != typeof(double) )
            {
                Debug.Log("obj type is not double : "+ obj.GetType());
                return;
            }

            double value = (double)obj;
            Image.fillAmount = (float)value;
        }
    }
}
