using UnityEngine;
using UniRx;
using System;
using System.Collections.Generic;

namespace UuIiView.Sample
{
    public static class AnimationExtentions
    {
        static Dictionary<string, IDisposable> playDisposables = new Dictionary<string, IDisposable>();

        public static Animation PlayClip(this Animation anim, string name)
        {
            if (anim.isPlaying)
            {
                if (playDisposables.ContainsKey(name)) Dispose(name);
                anim.Stop();
            }

            anim.clip = anim[name].clip;
            anim.Play(name);
            return anim;
        }

        public static void OnComplete(this Animation anim, Action<string> onCompleted, string name = "")
        {
            if (playDisposables.ContainsKey(name)) Dispose(name);

            name = string.IsNullOrEmpty(name) ? anim.clip.name : name;
            playDisposables[name] = Observable.EveryUpdate().TakeWhile(_ => anim.IsPlaying(name)).Subscribe(_ => { }, () => { onCompleted?.Invoke(name); Dispose(name); });
        }

        public static void Dispose(string name)
        {
            playDisposables[name].Dispose();
            playDisposables.Remove(name);
        }
    }
}