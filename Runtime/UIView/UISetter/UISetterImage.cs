using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.U2D;

namespace UuIiView
{
    /// <summary>
    /// 画像を設定するためのUISetter
    /// Resources/SpriteAtlas/SpriteHolderからの読み込みに対応
    /// JSON形式で色とパスを同時に指定可能
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class UISetterImage : UISetter
    {
        private Image image;

        /// <summary>Imageコンポーネントへの参照（遅延取得）</summary>
        private Image Image => image ??= GetComponent<Image>();

        /// <summary>
        /// スプライトの読み込み元を定義する列挙型
        /// </summary>
        public enum LoadFrom
        {
            /// <summary>Resourcesフォルダから読み込み</summary>
            Resource,
            /// <summary>SpriteHolderから読み込み</summary>
            SpriteHolder,
            /// <summary>SpriteAtlasから読み込み</summary>
            SpriteAtlas
        }

        /// <summary>スプライトの読み込み元</summary>
        [Tooltip("スプライトの読み込み元")]
        public LoadFrom loadFrom;

        /// <summary>SpriteHolder参照（LoadFrom.SpriteHolder時に使用）</summary>
        [HideInInspector] public SpriteHolder spriteHolder;

        /// <summary>SpriteAtlas参照（LoadFrom.SpriteAtlas時に使用）</summary>
        [HideInInspector] public SpriteAtlas atlas;

        /// <summary>
        /// データに基づいて画像を設定する
        /// JSON形式（{"color":"#FFFFFF","path":"..."}）または文字列パスを受け付ける
        /// </summary>
        /// <param name="obj">パス文字列またはJSON形式のオブジェクト</param>
        public override void Set(object obj)
        {
            if (obj == null) return;

            string path = ParseInput(obj);
            if (string.IsNullOrEmpty(path)) return;

            // bool値の場合はGameObjectの表示/非表示を切り替え
            if (bool.TryParse(path, out var activeSelf))
            {
                gameObject.SetActive(activeSelf);
                return;
            }

            gameObject.SetActive(true);
            Sprite sp = LoadSprite(path);
            if (sp != null)
            {
                Image.sprite = sp;
            }
            else
            {
                Debug.LogWarning($"[UISetterImage] {gameObject.name}: Sprite not found at '{path}' (LoadFrom: {loadFrom})");
            }
        }

        /// <summary>
        /// 入力オブジェクトを解析してパス文字列を取得する
        /// </summary>
        /// <param name="obj">入力オブジェクト</param>
        /// <returns>パス文字列（解析失敗時は空文字）</returns>
        private string ParseInput(object obj)
        {
            var objStr = obj.ToString();
            if (!objStr.StartsWith("{"))
            {
                return objStr;
            }

            // JSON形式のパース
            try
            {
                var dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(objStr);
                if (dict == null) return string.Empty;

                if (dict.TryGetValue("color", out var colorStr) && ColorUtility.TryParseHtmlString(colorStr, out var color))
                {
                    Image.color = color;
                }
                return dict.TryGetValue("path", out var pathValue) ? pathValue : string.Empty;
            }
            catch (Newtonsoft.Json.JsonException e)
            {
                Debug.LogError($"[UISetterImage] {gameObject.name}: JSONのパースに失敗しました。\n{e.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 指定されたLoadFromに基づいてスプライトを読み込む
        /// </summary>
        /// <param name="path">スプライトのパスまたはID</param>
        /// <returns>読み込んだSprite（失敗時はnull）</returns>
        private Sprite LoadSprite(string path)
        {
            switch (loadFrom)
            {
                case LoadFrom.Resource:
                    return Resources.Load<Sprite>(path);

                case LoadFrom.SpriteAtlas:
                    if (atlas == null)
                    {
                        Debug.LogError($"[UISetterImage] {gameObject.name}: SpriteAtlas is not assigned");
                        return null;
                    }
                    return atlas.GetSprite(path);

                case LoadFrom.SpriteHolder:
                    if (spriteHolder == null)
                    {
                        Debug.LogError($"[UISetterImage] {gameObject.name}: SpriteHolder is not assigned");
                        return null;
                    }
                    var ret = spriteHolder.Sprites.FirstOrDefault(x => x.Id == path);
                    return ret?.Sprite;

                default:
                    return null;
            }
        }
    }
}
