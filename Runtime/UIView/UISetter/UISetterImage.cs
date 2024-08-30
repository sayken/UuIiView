using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UuIiView;
using System.Linq;
using UnityEngine.U2D;

namespace UuIiView
{
    [RequireComponent(typeof(Image))]
    public class UISetterImage : UISetter
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

        public enum LoadFrom
        {
            Resource,
            SpriteHolder,
            SpriteAtlas
        }

        public LoadFrom loadFrom;

        [HideInInspector] public SpriteHolder spriteHolder;

        [HideInInspector] public SpriteAtlas atlas;

        public override void Set(object obj)
        {
            string path = string.Empty;

            if (obj.ToString().StartsWith("{"))
            {
                // 2つ以上のパラメータがある
                var dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(obj.ToString());

                if (ColorUtility.TryParseHtmlString(dict["color"], out var color))
                {
                    Image.color = color;
                }
                if (dict.ContainsKey("path"))
                {
                    path = dict["path"];
                }
            }
            else
            {
                // テキスト
                path = obj.ToString();
            }

            if( string.IsNullOrEmpty(path) )
            {
                return;
            }

            Sprite sp = null;
            switch (loadFrom)
            {
                case LoadFrom.Resource:
                    sp = Resources.Load<Sprite>(path);
                    break;
                case LoadFrom.SpriteAtlas:
                    sp = atlas.GetSprite(path);
                    break;
                case LoadFrom.SpriteHolder:
                    var ret = spriteHolder.Sprites.FirstOrDefault(x=>x.Id == path);
                    if ( ret != null )
                    {
                        sp = ret.Sprite;
                    }
                    break;
            }

            if ( sp != null )
            {
                Image.sprite = sp;
            }
        }
    }
}
