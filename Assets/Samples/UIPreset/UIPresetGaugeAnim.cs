using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UuIiView;

namespace UuIiView.Sample
{
    [RequireComponent(typeof(Slider))]
    public class UIPresetGaugeAnim : MonoBehaviour, IUIPreset
    {

        [SerializeField] float duration = 5f;
        [SerializeField] Ease ease;

        float nowValue;
        Slider slider;

        private void Awake()
        {
            slider = GetComponent<Slider>();
            nowValue = slider.value;
        }

        public void Preset(object obj)
        {
            float targetValue;
            if (!float.TryParse(obj.ToString(), out targetValue))
            {
                Debug.LogError($"floatに変換できません [obj={obj}]");
                return;
            }
            if (targetValue < 0f || targetValue > 1f)
            {
                Debug.LogError($"値が 0f〜1f ではありません。 [obj={obj}]");
                return;
            }


            DOTween
                .To(() => nowValue, (n) => nowValue = n, targetValue, duration)
                .SetEase(ease)
                .OnUpdate(() => slider.value = nowValue);
        }

    }
}
