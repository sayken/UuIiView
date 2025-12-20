using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Reflection;
using System.Text;

namespace UuIiView
{
    /// <summary>
    /// UIイベントの種類を定義する列挙型
    /// </summary>
    public enum EventType
    {
        /// <summary>イベントなし</summary>
        None,
        /// <summary>ログ出力用</summary>
        Log,
        /// <summary>ボタンクリック</summary>
        Button = 10,
        /// <summary>トグル切り替え</summary>
        Toggle,
        /// <summary>長押し</summary>
        LongTap,
        /// <summary>スライダー値変更</summary>
        Slider,
        /// <summary>入力フィールド変更</summary>
        Input,
        /// <summary>ドラッグ&ドロップ</summary>
        DragAndDrop,
    }

    /// <summary>
    /// UIアクションの種類を定義する列挙型
    /// </summary>
    public enum ActionType
    {
        /// <summary>アクションなし</summary>
        None,
        /// <summary>パネルを開く</summary>
        Open,
        /// <summary>パネルを閉じる</summary>
        Close,
        /// <summary>現在のパネルを閉じて別のパネルを開く</summary>
        CloseAndOpen,
        /// <summary>カスタムアクション</summary>
        Action,
        /// <summary>データ同期</summary>
        DataSync,
        /// <summary>グループを閉じて別のパネルを開く</summary>
        CloseGroupAndOpen,
        /// <summary>別パネルへのアクション</summary>
        ActionToPanel,
    }

    /// <summary>
    /// UuIiViewのコア処理を担当するコンポーネント
    /// JSON/Dictionary/クラスデータを受け取り、子UISetterにデータを配信する
    /// 子コンポーネントからのイベントをCommandLink形式に変換してReceiverに通知する
    /// </summary>
    public class UIViewRoot : MonoBehaviour
    {
        private object data;
        private List<UISetter> uiSetters;

        /// <summary>ルートのUIViewRoot参照</summary>
        public UIViewRoot RootUIViewRoot { get; private set;}
        /// <summary>イベント発生時のコールバック</summary>
        public Action<string> OnEvent { get; private set; }

        /// <summary>
        /// データとイベントハンドラを指定して初期化する
        /// </summary>
        /// <param name="d">表示するデータ（JSON/Dictionary/クラス）</param>
        /// <param name="onEvent">イベント発生時のコールバック</param>
        public void Init(object d, Action<string> onEvent) => InitInternal(null, d, onEvent);

        /// <summary>
        /// 親UIViewRootを指定して初期化する（ネスト用）
        /// </summary>
        /// <param name="root">親のUIViewRoot</param>
        /// <param name="d">表示するデータ</param>
        public void Init(UIViewRoot root, object d) => InitInternal(root, d, root?.OnEvent);

        /// <summary>
        /// 初期化の内部処理
        /// </summary>
        void InitInternal(UIViewRoot root, object d, Action<string> onEvent)
        {
            SetReceiver(onEvent);
            SetData(root, d);
        }

        /// <summary>
        /// UIイベントを受け取る（データ指定あり）
        /// </summary>
        /// <param name="targetPanelName">対象パネル名</param>
        /// <param name="name">イベント名</param>
        /// <param name="type">イベント種類</param>
        /// <param name="actType">アクション種類</param>
        /// <param name="parentName">親要素名</param>
        /// <param name="data">イベントに関連するデータ</param>
        /// <param name="isOn">トグル状態（Toggle用）</param>
        public void ReceiveEvent(string targetPanelName, string name, EventType type, ActionType actType, string parentName, object data, bool isOn = true)
            => ReceiveEventInternal(targetPanelName, name, type, actType, parentName, data, isOn);

        /// <summary>
        /// UIイベントを受け取る（現在のデータを使用）
        /// </summary>
        public void ReceiveEvent(string targetPanelName, string name, EventType type, ActionType actType, string parentName, bool isOn = true)
            => ReceiveEventInternal(targetPanelName, name, type, actType, parentName, data, isOn);

        /// <summary>
        /// UIイベントを受け取る（ルートパネル名を自動取得）
        /// </summary>
        public void ReceiveEvent(string name, EventType type, ActionType actType, string parentName, bool isOn = true)
        {
            var panelName = RootUIViewRoot != null ? RootUIViewRoot.gameObject.name : gameObject.name;
            ReceiveEventInternal(panelName, name, type, actType, parentName, data, isOn);
        }

        /// <summary>
        /// イベントを受け取ってCommandLink形式に変換し、OnEventに通知する
        /// </summary>
        void ReceiveEventInternal(string panelName, string name, EventType eventType, ActionType actionType, string parentName, object data, bool isOn)
        {
            StringBuilder commandLink = new StringBuilder();
            commandLink.Append($"{panelName}/{eventType}/{actionType}/{name}/{parentName}/");
            if (data != null)
            {
                // IdというKeyが含まれていたら、後ろにつける
                Dictionary<string, object> dic = new();
                if ( data.GetType() == typeof(Dictionary<string,object>) )
                {
                    dic = (Dictionary<string, object>)data;
                    if (dic.ContainsKey("Id"))
                    {
                        commandLink.Append(dic["Id"]);
                    }
                }

                // 名前の最後にIdが付くものは、パラメータとしてCommandLinkに追加
                dic.Where(_=>_.Key!="Id" && _.Key.EndsWith("Id")).ToList().ForEach(kv=>commandLink.Append($"/{kv.Key}={kv.Value}"));
            }

            commandLink.Append( eventType switch
            {
                EventType.Input => "/Input=" + data.ToString(),
                EventType.Slider => "/Slider=" + data.ToString(),
                EventType.Toggle => "/Toggle=" + isOn,
                _ => ""
            });

            OnEvent?.Invoke(commandLink.ToString());
        }

        /// <summary>
        /// イベント受信用のコールバックを設定する
        /// </summary>
        /// <param name="onEvent">イベント発生時に呼び出されるコールバック</param>
        public void SetReceiver(Action<string> onEvent)
        {
            this.OnEvent = onEvent;
        }

        /// <summary>
        /// データを設定してUIを更新する
        /// </summary>
        /// <param name="d">表示するデータ（JSON文字列/Dictionary/クラス）</param>
        public void SetData(object d) => SetData(null, d);

        /// <summary>
        /// ルートUIViewRootを指定してデータを設定する
        /// </summary>
        /// <param name="root">ルートのUIViewRoot（nullの場合は自身がルート）</param>
        /// <param name="d">表示するデータ</param>
        public void SetData(UIViewRoot root, object d)
        {
            RootUIViewRoot = root == null ? GetComponent<UIViewRoot>() : root;
            
            if ( d == null )
            {
                return;
            }
            if (d.GetType() == typeof(string))
            {
                //Log(d.ToString());
                try
                {
                    var dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(d.ToString());
                    if (dic != null)
                    {
                        UpdateDataByDic(dic);
                    }
                }
                catch (Newtonsoft.Json.JsonException e)
                {
                    Debug.LogError($"[UIViewRoot] {gameObject.name}: JSONのパースに失敗しました。\n{e.Message}");
                }
            }
            else if ( d.GetType() == typeof(Dictionary<string,object>))
            {
                //Log((Dictionary<string, object>)d);
                UpdateDataByDic((Dictionary<string, object>)d);
            }
            else
            {
                UpdateDataByClass(d);
            }
        }

        /// <summary>
        /// クラスのプロパティからUIを更新する（リフレクション使用）
        /// </summary>
        /// <param name="d">データクラスのインスタンス</param>
        void UpdateDataByClass(object d)
        {
            data = d;
            uiSetters ??= gameObject.GetComponentsInChildren<UISetter>(true).ToList();

            var infos = data.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            //Log(infos);
            foreach (UISetter u in uiSetters)
            {
                var prop = infos.FirstOrDefault(_ => _.Name == u.gameObject.name);
                if (prop == null) continue;
                SetObj(u, prop.GetValue(data));
            }
        }

        /// <summary>
        /// DictionaryからUIを更新する
        /// </summary>
        /// <param name="dic">キーと値のDictionary</param>
        void UpdateDataByDic(Dictionary<string,object> dic)
        {
            data = dic;
            uiSetters ??= gameObject.GetComponentsInChildren<UISetter>(true).ToList();

            foreach (UISetter u in uiSetters)
            {
                if (dic.TryGetValue(u.gameObject.name, out var value))
                {
                    SetObj(u, value);
                }
            }
        }

        /// <summary>
        /// UISetterにデータを設定する
        /// </summary>
        /// <param name="uiSetter">対象のUISetter</param>
        /// <param name="obj">設定するデータ</param>
        void SetObj(UISetter uiSetter, object obj)
        {
            if (uiSetter == null) return;

            try
            {
                uiSetter.SetObj(obj);

                if (uiSetter.transform == gameObject.transform) return;
                uiSetter.GetComponent<UIViewRoot>()?.InitInternal(RootUIViewRoot, obj, OnEvent);
            }
            catch(Exception e)
            {
                Debug.LogError("e = " + e.ToString() +"\nobj = "+ obj?.ToString());
            }
        }

        // =====================================================================================================
        // ログ出力（デバッグ用）
        // =====================================================================================================

        /// <summary>
        /// 文字列データをログ出力する（デバッグ用）
        /// </summary>
        /// <param name="d">出力する文字列</param>
        void Log(string d)
        {
            Debug.Log($"<color=yellow>[UuIiView] SetData (string) {gameObject.name}</color>\n{d}");
        }
        /// <summary>
        /// Dictionaryデータをログ出力する（デバッグ用）
        /// </summary>
        /// <param name="dic">出力するDictionary</param>
        void Log(Dictionary<string, object> dic)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var kv in dic)
            {
                sb.Append(kv.Key).Append(" : ").AppendLine(kv.Value?.ToString() ?? "null");
            }
            Debug.Log($"<color=yellow>[UuIiView] SetData (Dictionary) {gameObject.name}</color>\n{sb.ToString()}");
        }

        /// <summary>
        /// クラスのプロパティ情報をログ出力する（デバッグ用）
        /// </summary>
        /// <param name="infos">出力するPropertyInfo配列</param>
        void Log(PropertyInfo[] infos)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var pi in infos)
            {
                var value = pi.GetValue(data);
                sb.Append(pi.Name).Append(" : ").AppendLine(value?.ToString() ?? "null");
            }
            Debug.Log($"<color=yellow>[UuIiView] SetData (Proto) {gameObject.name}</color>\n{sb.ToString()}");
        }
    }
}
