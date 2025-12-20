using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UuIiView
{
    /// <summary>
    /// リスト表示を行うUISetter
    /// データ配列に基づいてセルを生成・再利用する
    /// </summary>
    public class UISetterList : UISetter
    {
        private UIViewRoot uiViewRoot;

        /// <summary>セルのプレハブ</summary>
        [SerializeField]
        [Tooltip("リストセルのプレハブ（UIViewRootを持つ）")]
        private UIViewRoot cellPrefab;

        /// <summary>セルの配置先Transform</summary>
        [SerializeField]
        [Tooltip("セルを配置するルートTransform")]
        private Transform listRoot;

        /// <summary>セルに付与する名前</summary>
        [SerializeField]
        [Tooltip("生成されたセルのGameObject名")]
        private string itemName;

        private readonly List<UIViewRoot> itemCells = new List<UIViewRoot>();

        /// <summary>
        /// リストデータを設定してセルを生成・更新する
        /// </summary>
        /// <param name="obj">IList型のデータ配列</param>
        public override void Set(object obj)
        {
            if (obj == null) return;

            // 入力検証
            if (!ValidateInputs(out var errorMessage))
            {
                Debug.LogError($"[UISetterList] {gameObject.name}: {errorMessage}");
                return;
            }

            // IListへの変換
            IList dataList = ConvertToList(obj);
            if (dataList == null) return;

            // 親UIViewRootの取得
            uiViewRoot = GetComponentInParent<UIViewRoot>();
            if (uiViewRoot == null)
            {
                Debug.LogError($"[UISetterList] {gameObject.name}: UIViewRootが見つかりません。");
                return;
            }

            // セルの生成・更新
            UpdateCells(dataList);

            // Event系コンポーネントに親名を設定
            RegisterParentName();
        }

        /// <summary>
        /// 必須パラメータの検証
        /// </summary>
        private bool ValidateInputs(out string errorMessage)
        {
            if (cellPrefab == null)
            {
                errorMessage = "cellPrefabが設定されていません。";
                return false;
            }
            if (listRoot == null)
            {
                listRoot = transform; // デフォルトは自身
            }
            errorMessage = null;
            return true;
        }

        /// <summary>
        /// オブジェクトをIListに変換する
        /// </summary>
        private IList ConvertToList(object obj)
        {
            if (obj is not IList list)
            {
                Debug.LogWarning($"[UISetterList] {gameObject.name}: objがIListではありません。Type={obj.GetType()}");
                return null;
            }

            // JArrayの場合はDictionaryに変換
            if (obj.GetType() == typeof(Newtonsoft.Json.Linq.JArray))
            {
                var convertedList = new List<object>();
                foreach (var o in list)
                {
                    try
                    {
                        var d = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(o.ToString());
                        convertedList.Add(d);
                    }
                    catch (Newtonsoft.Json.JsonException e)
                    {
                        Debug.LogError($"[UISetterList] {gameObject.name}: JSONパースエラー: {e.Message}");
                    }
                }
                return convertedList;
            }

            return list;
        }

        /// <summary>
        /// セルの生成と更新
        /// </summary>
        /// <param name="dataList">表示するデータのリスト</param>
        private void UpdateCells(IList dataList)
        {
            // 必要に応じてセルを追加生成
            for (int i = itemCells.Count; i < dataList.Count; i++)
            {
                var vm = Instantiate(cellPrefab, listRoot);
                if (vm == null)
                {
                    Debug.LogError($"[UISetterList] {gameObject.name}: セルのInstantiateに失敗しました。");
                    continue;
                }
                if (!string.IsNullOrWhiteSpace(itemName)) vm.gameObject.name = itemName;
                itemCells.Add(vm);
            }

            // セルの表示/非表示とデータ設定
            for (int i = 0; i < itemCells.Count; i++)
            {
                if (dataList.Count > i)
                {
                    itemCells[i].Init(uiViewRoot.RootUIViewRoot, dataList[i]);
                    itemCells[i].gameObject.SetActive(true);
                }
                else
                {
                    itemCells[i].gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 子のIUICustomコンポーネントに親名を登録する
        /// </summary>
        private void RegisterParentName()
        {
            foreach (var uiCustom in GetComponentsInChildren<IUICustom>(true))
            {
                uiCustom.ParentName = gameObject.name;
            }
        }
    }
}