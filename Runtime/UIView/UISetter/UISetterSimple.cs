using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;

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
        List,
        TMP_InputField
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
            else if (GetComponent<Image>() != null)
                t = UIType.Image;
            else if (GetComponent<RawImage>() != null)
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
            else if (GetComponent<ScrollRect>() != null)
                t = UIType.List;
            else if (GetComponent<TMP_InputField>() != null)
                t = UIType.TMP_InputField;
            else
                t = UIType.GameObject;

            return t;
        }

        public override void Set(object obj)
        {
            switch (uiType)
            {
                case UIType.Text:
                    GetComponent<TextMeshProUGUI>().text = obj.ToString();
                    break;
                case UIType.Image:
                    var resReq = Resources.LoadAsync<Sprite>(obj.ToString());
                    resReq.completed += (obj) =>
                    {
                        GetComponent<Image>().sprite = (Sprite)resReq.asset;
                    };
                    break;
                case UIType.RawImage:
                    StartCoroutine(SetTexture(obj.ToString()));
                    break;
                case UIType.CustomButton:
                    GetComponent<CustomButton>().Interactable = (bool)obj;
                    break;
                case UIType.CustomToggle:
                    GetComponent<CustomToggle>().IsOn = (bool)obj;
                    break;
                case UIType.Button:
                    GetComponent<Button>().interactable = (bool)obj;
                    break;
                case UIType.Toggle:
                    GetComponent<Toggle>().SetIsOnWithoutNotify((bool)obj);
                    break;
                case UIType.Slider:
                    if (float.TryParse(obj.ToString(), out float f))
                    {
                        GetComponent<Slider>().value = f;
                    }
                    break;
                case UIType.List:
                    SetList(obj);
                    break;
                case UIType.TMP_InputField:
                    GetComponent<TMP_InputField>().text = obj.ToString();
                    break;
                case UIType.GameObject:
                    gameObject.SetActive((bool)obj);
                    break;
            }
        }

        // ===== For RawImage =============================================================================================
        IEnumerator SetTexture(string uri)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri);

            //画像を取得できるまで待つ
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                //取得した画像のテクスチャをRawImageのテクスチャに張り付ける
                GetComponent<RawImage>().texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            }
        }

        // ===== For List =================================================================================================
        public UIViewRoot rootUIViewRoot;
        public UIViewRoot cellPrefab;
        public string itemName;

        List<UIViewRoot> itemCells = new List<UIViewRoot>();

        void SetList(object obj)
        {
            var dataList = (IList)obj;
            if (obj.GetType() == typeof(Newtonsoft.Json.Linq.JArray))
            {
                dataList = new List<object>();
                foreach (var o in (IList)obj)
                {
                    var d = JsonConvert.DeserializeObject<Dictionary<string, object>>(o.ToString());
                    dataList.Add(d);
                }
            }

            var listRoot = GetComponent<ScrollRect>().content;

            for ( int i=0 ; i<dataList.Count ; i++)
            {
                if ( itemCells.Count > i )
                {
                    // 既にitemCellsにデータがあるので流用
                    itemCells[i].Init(rootUIViewRoot, dataList[i]);
                }
                else
                {
                    // itemCellsにデータが無い（or 足りない）ので追加
                    var vm = Instantiate(cellPrefab, listRoot);
                    if (!string.IsNullOrWhiteSpace(itemName)) vm.gameObject.name = itemName;
                    vm.Init(rootUIViewRoot, dataList[i]);
                    itemCells.Add(vm);
                }
            }

            // 必要なitemCellだけ表示状態にする
            for ( int i=0; i<itemCells.Count; i++)
            {
                itemCells[i].gameObject.SetActive(dataList.Count > i);
            }
        }
    }
}
