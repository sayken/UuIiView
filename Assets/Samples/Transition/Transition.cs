using UnityEngine;
using System;
using DG.Tweening;
using UuIiView;

namespace UuIiView.Sample
{
    public class Transition : MonoBehaviour, ITransition
    {
        public enum TransitionType
        {
            None,
            Tween,
            LegacyAnimation
        }

        public enum TweenType
        {
            Top,
            Bottom,
            Left,
            Right,
            Scale,
            Fade
        }

        RectTransform _rectTransform;
        RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null) _rectTransform = (RectTransform)transform;
                return _rectTransform;
            }
        }

        public TransitionType inType = TransitionType.None;
        public TransitionType outType = TransitionType.None;

        [HideInInspector] public Animation anim;
        [HideInInspector] public AnimationClip inAnimClip;
        [HideInInspector] public AnimationClip outAnimClip;

        [HideInInspector] public CanvasGroup canvasGroup;
        [HideInInspector] public TweenType inTweenType;
        [HideInInspector] public float inDuration;
        [HideInInspector] public Ease inEaseType;
        [HideInInspector] public TweenType outTweenType;
        [HideInInspector] public float outDuration;
        [HideInInspector] public Ease outEaseType;
        [HideInInspector] public float scaleRate;

        private Vector2 screenInPos;
        private Vector3 defaultScale;

        public bool isTransitionIn { get; private set; } = false;

        private void Awake()
        {
            screenInPos = rectTransform.anchoredPosition;
            defaultScale = rectTransform.localScale;
        }

        public void TransitionIn(Action onComplete)
        {
            switch (inType)
            {
                case TransitionType.None:
                    OnCompleted(onComplete, true);
                    break;
                case TransitionType.Tween:
                    Tween(true, inTweenType, inDuration, inEaseType)?.OnComplete(() => OnCompleted(onComplete, true));
                    break;
                case TransitionType.LegacyAnimation:
                    if (anim[inAnimClip.name] == null)
                    {
                        anim.AddClip(inAnimClip, inAnimClip.name);
                    }
                    anim.PlayClip(inAnimClip.name).OnComplete((_) => OnCompleted(onComplete, true));
                    break;
            }
        }

        public void TransitionOut(Action onComplete)
        {
            switch (outType)
            {
                case TransitionType.None:
                    OnCompleted(onComplete, false);
                    break;
                case TransitionType.Tween:
                    Tween(false, outTweenType, outDuration, outEaseType)?.OnComplete(() => OnCompleted(onComplete, false));
                    break;
                case TransitionType.LegacyAnimation:
                    if (anim[outAnimClip.name] == null)
                    {
                        anim.AddClip(outAnimClip, outAnimClip.name);
                    }
                    anim.PlayClip(outAnimClip.name).OnComplete((_) => OnCompleted(onComplete, false));
                    break;
            }
        }

        void OnCompleted(Action onComplete, bool isTransitionIn)
        {
            onComplete?.Invoke();
            this.isTransitionIn = isTransitionIn;
        }


        Tweener Tween(bool isIn, TweenType tweenType, float duration, Ease ease)
        {
            Tweener tweener = null;

            rectTransform.DOKill();
            canvasGroup.DOKill();

            var screenOutPos = Vector2.zero;
            bool movePos = true;

            var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(rectTransform);
            bounds.size = new Vector3(Mathf.Min(bounds.size.x, Screen.width), Mathf.Min(bounds.size.y, Screen.height), bounds.size.z);

            switch (tweenType)
            {
                case TweenType.Scale:
                    rectTransform.localScale = defaultScale * (isIn ? scaleRate : 1f);
                    canvasGroup.alpha = Convert.ToInt16(!isIn);
                    tweener = rectTransform.DOScale(defaultScale * (isIn ? 1f : scaleRate), duration).SetEase(ease);
                    canvasGroup.DOFade(Convert.ToInt16(isIn), duration).SetEase(ease);
                    movePos = false;
                    break;
                case TweenType.Fade:
                    canvasGroup.alpha = Convert.ToInt16(!isIn);
                    tweener = canvasGroup.DOFade(Convert.ToInt16(isIn), duration).SetEase(ease);
                    movePos = false;
                    break;
                case TweenType.Top:
                    screenOutPos = new Vector2(screenInPos.x, bounds.size.y);
                    break;
                case TweenType.Bottom:
                    screenOutPos = new Vector2(screenInPos.x, -bounds.size.y);
                    break;
                case TweenType.Left:
                    screenOutPos = new Vector2(-bounds.size.x, screenInPos.y);
                    break;
                case TweenType.Right:
                    screenOutPos = new Vector2(bounds.size.x, screenInPos.y);
                    break;
            }

            if (movePos)
            {
                if (isIn) rectTransform.anchoredPosition = screenOutPos;
                tweener = rectTransform.DOAnchorPos((isIn ? screenInPos : screenOutPos), duration).SetEase(ease);
            }
            return tweener;
        }

#if UNITY_EDITOR
        public void Reset()
        {
            rectTransform.anchoredPosition = screenInPos;
            rectTransform.localScale = defaultScale;
        }
#endif

    }
}