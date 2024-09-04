using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UuIiView
{
    public class CustomToggleGroup : MonoBehaviour
    {
        public List<CustomToggle> customToggles = new List<CustomToggle>();

        [SerializeField] bool allowSwitchOff = false;
        [SerializeField] bool allowMultiSelect = true;
        [SerializeField] int allowMaxSelect = 1;

        private void Start()
        {
            if (!customToggles.Any(_ => _.isOn) && customToggles.Count > 0 && !allowSwitchOff)
            {
                customToggles[0].IsOn = true;
            }
        }

        public void On(CustomToggle toggle, bool isOn)
        {
            int onCount = customToggles.Count(_ => _.isOn);

            if (toggle.IsOn == isOn) return;

            if (isOn == true)
            {
                // 現在選択されていないトグルを選択
                if (!allowMultiSelect)
                {
                    foreach ( var tgl in customToggles)
                    {
                        tgl.IsOn = (tgl==toggle);
                        tgl.TriggerEvent();
                    }

                }
                else if (allowMaxSelect > onCount)
                {
                    toggle.IsOn = true;
                }
            }
            else if (isOn == false && (onCount > 1 || allowSwitchOff))
            {
                toggle.IsOn = false;
            }
        }

        public void SelectToggle(int idx = 0)
        {
            if ( idx < 0 || customToggles.Count <= idx )
            {
                idx = 0;
            }
            On(customToggles[idx], true);
        }
    }
}
