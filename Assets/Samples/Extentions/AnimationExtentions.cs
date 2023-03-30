using UnityEngine;
using UniRx;
using System;

namespace UuIiView.Sample
{
    public static class AnimationExtentions
    {
        static IDisposable playDisposable;

        public static Animation PlayClip(this Animation anim, string name)
        {
            if (anim.isPlaying)
            {
                if (playDisposable != null) playDisposable.Dispose();
                anim.Stop();
            }

            anim.clip = anim[name].clip;
            anim.Play(name);
            return anim;
        }

        public static void OnComplete(this Animation anim, Action<string> onCompleted, string name = "")
        {
            if (playDisposable != null) playDisposable.Dispose();
            name = string.IsNullOrEmpty(name) ? anim.clip.name : name;
            playDisposable = Observable.EveryUpdate().TakeWhile(_ => anim.IsPlaying(name)).Subscribe(_ => { }, () => { onCompleted?.Invoke(name); });
        }
    }
}