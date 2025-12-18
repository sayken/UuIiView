# UuIiView

UuIiView（ウーイービュー）はUnity上でUIを開発する際、uGUIを使用したMVC(P)モデルでの開発におけるViewの部分の実装を汎用化させるためのモジュールです。

## インストール

Package Managerを使用します。

```
https://github.com/sayken/UuIiView.git#upm
```

## 主な機能

- JSON / Dictionary / Class からUIへのデータバインディング
- UIイベント（Button, Toggle, Slider等）の統一的なハンドリング

## ディレクトリ構成

```
UuIiView/
├── Runtime/
│   ├── UIView/
│   │   ├── UIViewRoot.cs       # コア処理（データバインディング・イベント受信）
│   │   ├── UISetter/           # UI更新コンポーネント群
│   │   │   ├── UISetter.cs           # 基底クラス
│   │   │   ├── UISetterSimple.cs     # 汎用UI更新
│   │   │   ├── UISetterText.cs       # テキスト表示（フォーマット対応）
│   │   │   ├── UISetterImage.cs      # 画像表示
│   │   │   ├── UISetterList.cs       # リスト表示（セル再利用）
│   │   │   ├── UISetterDialog.cs     # ダイアログボタン生成
│   │   │   ├── UISetterGauge.cs      # ゲージ表示
│   │   │   └── UISetterAnimator.cs   # Animatorパラメータ制御
│   │   └── CustomUI/           # カスタムUIコンポーネント群
│   │       ├── CustomButton.cs       # 拡張ボタン（長押し対応）
│   │       ├── CustomToggle.cs       # 拡張トグル
│   │       ├── CustomToggleGroup.cs  # トグルグループ管理
│   │       └── EventRelay.cs         # 標準UIイベント中継
│   ├── Interface/              # インターフェース定義
│   ├── Utils/                  # ユーティリティ
│   └── ViewTest/               # テスト用
└── Editor/                     # エディタ拡張
```

## 基本的な使い方

### 1. データの設定

`UIViewRoot.SetData()` にデータを渡すことでUIが更新されます。

```csharp
// JSON文字列
uiViewRoot.SetData("{\"PlayerName\": \"太郎\", \"Score\": 100}");

// Dictionary
var data = new Dictionary<string, object>
{
    { "PlayerName", "太郎" },
    { "Score", 100 }
};
uiViewRoot.SetData(data);

// クラス（プロパティ名がGameObject名と一致）
uiViewRoot.SetData(playerData);
```

### 2. イベントの受信

`UIViewRoot.SetReceiver()` または `Init()` でイベントハンドラを設定します。

```csharp
uiViewRoot.Init(data, (commandLink) =>
{
    Debug.Log($"Event received: {commandLink}");
    // commandLink形式: "PanelName/EventType/ActionType/EventName/ParentName/Id"
});
```

## JSON形式

```json
{
    "TextObject": "表示するテキスト",
    "ImageObject": "Resources/path/to/sprite",
    "ButtonObject": true,
    "NestedObject": {
        "Id": "unique-id-001",
        "ChildText": "子要素のテキスト"
    },
    "ListObject": [
        {
            "Id": "item-001",
            "ItemName": "アイテム1",
            "ItemCount": 10
        },
        {
            "Id": "item-002",
            "ItemName": "アイテム2",
            "ItemCount": 20
        }
    ]
}
```

### 特殊なキー

- `Id`: イベント発生時に呼び出し元を特定するためのユニークID

## UISetterの種類

| クラス | 用途 | 受け取る値 |
|--------|------|-----------|
| UISetterSimple | 汎用（Text, Image, Button等） | 型に応じて自動判定 |
| UISetterText | テキスト表示 | string / JSON（color, text） |
| UISetterImage | 画像表示 | string（パス）/ JSON（color, path） |
| UISetterList | リスト表示 | IList |
| UISetterDialog | ダイアログボタン | IList（IsPositive, Name, EventName） |
| UISetterGauge | ゲージ/プログレスバー | double / float（0.0〜1.0） |
| UISetterAnimator | Animatorパラメータ | Dictionary（パラメータ名: 値） |

### UISetterSimpleの対応UIType

| UIType | コンポーネント | 値の型 |
|--------|---------------|--------|
| Text | TextMeshProUGUI / Text | string |
| Image | Image | string（Resourcesパス） |
| RawImage | RawImage | string（URL）/ bool |
| GameObject | - | bool（SetActive） |
| CustomButton | CustomButton | bool（Interactable） |
| CustomToggle | CustomToggle | bool（IsOn） |
| Button | Button | bool（interactable） |
| Toggle | Toggle | bool（isOn） |
| Slider | Slider | float（value） |
| TMP_InputField | TMP_InputField | string |
| CustomToggleGroup | CustomToggleGroup | int（選択index） |

## CustomUIコンポーネント

### CustomButton

Animator連携と長押し対応のボタン。

```
ActionType:
- None: 何もしない
- Open: パネルを開く
- Close: パネルを閉じる
- CloseAndOpen: 閉じて開く
- Action: アクション実行
- ActionToPanel: 特定パネルへアクション
- CloseGroupAndOpen: グループを閉じて開く
```

### CustomToggle

トグル機能を持つボタン。CustomToggleGroupと連携可能。

### CustomToggleGroup

複数のCustomToggleを管理。
- `allowSwitchOff`: すべてOFFを許可
- `allowMultiSelect`: 複数選択を許可
- `allowMaxSelect`: 最大選択数

## イベント形式

イベントは以下のPath形式で通知されます：

```
PanelName/EventType/ActionType/EventName/ParentName/Id
```

### EventType

| 値 | 説明 |
|----|------|
| Button | ボタンクリック |
| Toggle | トグル変更 |
| LongTap | 長押し |
| Slider | スライダー変更 |
| Input | 入力フィールド変更 |

## 動作環境

- Unity 2021.3 以上
- 依存: Newtonsoft.Json, TextMeshPro

## ライセンス

LICENSE.md を参照してください。

## 作者

sayken (sayken2000@gmail.com)
