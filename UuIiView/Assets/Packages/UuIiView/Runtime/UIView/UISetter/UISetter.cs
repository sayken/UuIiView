using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UuIiView
{
    public class UISetter : MonoBehaviour
    {
        IUIPreset uiPreset;
        IUIPreset UIPreset
        {
            get
            {
                uiPreset = uiPreset ?? GetComponent<IUIPreset>();
                return uiPreset;
            }
        }


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

        public virtual void Set(object obj)
        {
            Debug.Log(obj.ToString());
        }
        
    }
}