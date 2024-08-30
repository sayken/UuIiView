using System;
using UnityEngine;

namespace UuIiView
{
    [CreateAssetMenu(menuName ="UuIiView/Create SpriteHolder")]
    public class SpriteHolder : ScriptableObject
    {
        public SpriteInfo[] Sprites;
    }

    [Serializable]
    public class SpriteInfo
    {
        public string Id;
        public Sprite Sprite;
    }
}