using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace UuIiView
{
    /// <summary>
    /// UIコンポーネントの種類を定義する列挙型
    /// </summary>
    public enum UIType
    {
        /// <summary>未設定</summary>
        None,
        /// <summary>テキスト（TextMeshProUGUI/Text）</summary>
        Text,
        /// <summary>画像（Image）</summary>
        Image,
        /// <summary>生画像（RawImage）- URL読み込み対応</summary>
        RawImage,
        /// <summary>ゲームオブジェクトの表示/非表示</summary>
        GameObject,
        /// <summary>カスタムボタン</summary>
        CustomButton,
        /// <summary>カスタムトグル</summary>
        CustomToggle,
        /// <summary>標準ボタン</summary>
        Button,
        /// <summary>標準トグル</summary>
        Toggle,
        /// <summary>スライダー</summary>
        Slider,
        /// <summary>TextMeshPro入力フィールド</summary>
        TMP_InputField,
        /// <summary>カスタムトグルグループ</summary>
        CustomToggleGroup
    }

    /// <summary>
    /// 汎用的なUI要素へのデータ設定を行うクラス
    /// UITypeに応じて適切なコンポーネントにデータを反映する
    /// </summary>
    public class UISetterSimple : UISetter
    {
        /// <summary>対象のUIコンポーネント種類</summary>
        [Tooltip("対象のUIコンポーネント種類（Noneの場合は自動検出）")]
        public UIType uiType;

        private void Awake()
        {
            if (uiType == UIType.None) uiType = SetType();
        }

        /// <summary>
        /// アタッチされているコンポーネントからUITypeを自動判定する
        /// </summary>
        /// <returns>検出されたUIType</returns>
        public UIType SetType()
        {
            UIType t = UIType.None;
            if (GetComponent<TextMeshProUGUI>() != null)
                t = UIType.Text;
            else if (GetComponent<RawImage>() != null)
                t = UIType.RawImage;
            else if (GetComponent<Image>() != null)
                t = UIType.Image;
            else if (GetComponent<CustomButton>() != null)
                t = UIType.CustomButton;
            else if (GetComponent<CustomToggle>() != null)
                t = UIType.CustomToggle;
            else if (GetComponent<Button>() != null)
                t = UIType.Button;
            else if (GetComponent<Toggle>() != null)
                t = UIType.Toggle;
            else if (GetComponent<Slider>() != null)
                t = UIType.Slider;
            else if (GetComponent<TMP_InputField>() != null)
                t = UIType.TMP_InputField;
            else if (GetComponent<CustomToggleGroup>() != null)
                t = UIType.CustomToggleGroup;
            else
                t = UIType.GameObject;

            return t;
        }

        /// <summary>
        /// UITypeに応じてデータをUIコンポーネントに反映する
        /// </summary>
        /// <param name="obj">設定するデータ</param>
        public override void Set(object obj)
        {
            if (obj == null) return;

            switch (uiType)
            {
                case UIType.Text:
                    SetText(obj);
                    break;
                case UIType.Image:
                    SetImage(obj.ToString());
                    break;
                case UIType.RawImage:
                    if (bool.TryParse(obj.ToString(), out bool b))
                    {
                        gameObject.SetActive(b);
                    }
                    else
                    {
                        gameObject.SetActive(true);
                        StartCoroutine(SetTexture(obj.ToString()));
                    }
                    break;
                case UIType.CustomButton:
                    if (TryParseBool(obj, out bool customButtonVal))
                    {
                        var customBtn = GetComponent<CustomButton>();
                        if (customBtn != null) customBtn.Interactable = customButtonVal;
                    }
                    break;
                case UIType.CustomToggle:
                    if (TryParseBool(obj, out bool customToggleVal))
                    {
                        var customTgl = GetComponent<CustomToggle>();
                        if (customTgl != null) customTgl.IsOn = customToggleVal;
                    }
                    break;
                case UIType.Button:
                    if (TryParseBool(obj, out bool buttonVal))
                    {
                        var btn = GetComponent<Button>();
                        if (btn != null) btn.interactable = buttonVal;
                    }
                    break;
                case UIType.Toggle:
                    if (TryParseBool(obj, out bool toggleVal))
                    {
                        var tgl = GetComponent<Toggle>();
                        if (tgl != null) tgl.SetIsOnWithoutNotify(toggleVal);
                    }
                    break;
                case UIType.Slider:
                    if (float.TryParse(obj.ToString(), out float f))
                    {
                        var slider = GetComponent<Slider>();
                        if (slider != null) slider.value = f;
                    }
                    break;
                case UIType.TMP_InputField:
                    var inputField = GetComponent<TMP_InputField>();
                    if (inputField != null) inputField.text = obj.ToString();
                    break;
                case UIType.CustomToggleGroup:
                    if (int.TryParse(obj.ToString(), out int idx))
                    {
                        var toggleGroup = GetComponent<CustomToggleGroup>();
                        if (toggleGroup != null) toggleGroup.SelectToggle(idx);
                    }
                    break;
                case UIType.GameObject:
                    if (TryParseBool(obj, out bool goVal))
                    {
                        gameObject.SetActive(goVal);
                    }
                    break;
            }
        }

        /// <summary>
        /// テキストコンポーネントにデータを設定する
        /// </summary>
        private void SetText(object obj)
        {
            var tmpro = GetComponent<TextMeshProUGUI>();
            if (tmpro != null)
            {
                tmpro.text = obj.ToString();
                return;
            }
            var txt = GetComponent<Text>();
            if (txt != null)
            {
                txt.text = obj.ToString();
            }
        }

        /// <summary>
        /// Imageコンポーネントにスプライトを設定する（Resources非同期読み込み）
        /// </summary>
        /// <param name="path">Resourcesパス</param>
        private void SetImage(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning($"[UISetterSimple] {gameObject.name}: Image path is empty");
                return;
            }

            var image = GetComponent<Image>();
            if (image == null)
            {
                Debug.LogWarning($"[UISetterSimple] {gameObject.name}: Image component not found");
                return;
            }

            var resReq = Resources.LoadAsync<Sprite>(path);
            resReq.completed += _ =>
            {
                if (resReq.asset is Sprite sprite)
                {
                    image.sprite = sprite;
                }
                else
                {
                    Debug.LogWarning($"[UISetterSimple] {gameObject.name}: Sprite not found at '{path}'");
                }
            };
        }

        /// <summary>
        /// objectをbool型に安全に変換する
        /// </summary>
        bool TryParseBool(object obj, out bool result)
        {
            result = false;
            if (obj == null) return false;

            if (obj is bool b)
            {
                result = b;
                return true;
            }

            return bool.TryParse(obj.ToString(), out result);
        }

        /// <summary>
        /// URLから画像を取得してRawImageに設定する
        /// </summary>
        /// <param name="uri">画像のURL</param>
        IEnumerator SetTexture(string uri)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri))
            {
                //画像を取得できるまで待つ
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(www.error);
                }
                else
                {
                    //取得した画像のテクスチャをRawImageのテクスチャに張り付ける
                    var rawImage = GetComponent<RawImage>();
                    if (rawImage != null)
                    {
                        rawImage.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    }
                }
            }
        }
    }
}
