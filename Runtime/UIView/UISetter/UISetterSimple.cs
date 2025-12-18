using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace UuIiView
{
    public enum UIType
    {
        None,
        Text,
        Image,
        RawImage,
        GameObject,
        CustomButton,
        CustomToggle,
        Button,
        Toggle,
        Slider,
        TMP_InputField,
        CustomToggleGroup
    }

    public class UISetterSimple : UISetter
    {
        public UIType uiType;

        private void Awake()
        {
            if (uiType == UIType.None) uiType = SetType();
        }

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

        public override void Set(object obj)
        {
            if (obj == null) return;

            switch (uiType)
            {
                case UIType.Text:
                    var tmpro = GetComponent<TextMeshProUGUI>();
                    if ( tmpro != null )
                    {
                        tmpro.text = obj.ToString();
                        break;
                    }
                    var txt = GetComponent<Text>();
                    if ( txt != null )
                    {
                        txt.text = obj.ToString();
                    }
                    break;
                case UIType.Image:
                    var resReq = Resources.LoadAsync<Sprite>(obj.ToString());
                    resReq.completed += (obj) =>
                    {
                        GetComponent<Image>().sprite = (Sprite)resReq.asset;
                    };
                    break;
                case UIType.RawImage:
                    if ( bool.TryParse( obj.ToString(), out bool b) )
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
                        GetComponent<CustomButton>().Interactable = customButtonVal;
                    }
                    break;
                case UIType.CustomToggle:
                    if (TryParseBool(obj, out bool customToggleVal))
                    {
                        GetComponent<CustomToggle>().IsOn = customToggleVal;
                    }
                    break;
                case UIType.Button:
                    if (TryParseBool(obj, out bool buttonVal))
                    {
                        GetComponent<Button>().interactable = buttonVal;
                    }
                    break;
                case UIType.Toggle:
                    if (TryParseBool(obj, out bool toggleVal))
                    {
                        GetComponent<Toggle>().SetIsOnWithoutNotify(toggleVal);
                    }
                    break;
                case UIType.Slider:
                    if (float.TryParse(obj.ToString(), out float f))
                    {
                        GetComponent<Slider>().value = f;
                    }
                    break;
                case UIType.TMP_InputField:
                    GetComponent<TMP_InputField>().text = obj.ToString();
                    break;
                case UIType.CustomToggleGroup:
                    if ( int.TryParse(obj.ToString(), out int idx))
                    {
                        GetComponent<CustomToggleGroup>().SelectToggle(idx);
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

        // ===== For RawImage =============================================================================================
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
