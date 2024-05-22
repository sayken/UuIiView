# UuIiView
UuIiView（ウーイービュー）はUnity上でUIを開発する際、uGUIを使用したMVC(P)モデルでの開発におけるViewの部分の実装を汎用化させるためのモジュールです。  

## 始め方
PackageMangagerを使用します。  
`https://github.com/sayken/UuIiView.git#upm`

## Viewの表示の更新とEventの受取
![画像1](https://github.com/sayken/UuIiView/assets/6512883/60b36a6d-be44-45e8-866b-47d767974b81)

UIの表示は、UIViewRoot.SetData() に json形式のstring を渡すことで更新されます。  
(json以外にも、Dictionary<string,object>、classにも対応しています)  
jsonの形式は  
- GameObjectに値を渡す時は、通常の"key":"value"
- GameObjectに複数の値を渡す時はクラス
- リストに値を渡す時はクラスの配列

を渡します。
```
{  
    "GameObject名(string型)" : "UISetterに渡す値(object型)",
    "GameObject名(string型)" : 
    {
        "Id" : "このクラスを特定できる任意のユニークな文字列",
        "GameObject名(string型)" : "UISetterに渡す値(object型)",
        "GameObject名(string型)" : "UISetterに渡す値(object型)",
    }
    "GameObject名":[
        {
            "Id" : "このクラスを特定できる任意のユニークな文字列",
            "GameObject名":"値",
            "GameObject名":"値"
        },{
            "Id" : "このクラスを特定できる任意のユニークな文字列",
            "GameObject名":"値",
            "GameObject名":"値"
        }
    ]
}
```
クラスを渡す場合、GameObject名とは関係なく`Id`というキー（固定の文字列）を渡すと、下の項のEventの受取でIdが渡されるので、OnEventで呼び出し元を特定するのに使用できます。

## Viewで発生するEventの受取
UIで発生したEventは、UIViewRoot.SetReceiver() に Action<string> を渡すことで、Path形式でEventを受け取れます。  
（Path形式はCommandLinkクラスに変換することで、各種情報を取り出しやすくなっています）  
各種情報  
- Id：Jsonで渡したId。リスト表示などで使用。
- PanelName：UIPanel名
- EventName：Eventを発生させたGameObject名（ButtonならButtonのGameObject名、ScrollRectならScrollRect名）
- EventType：enum.EventTypeで設定されたもの( Button, Toggle, InputFieldValueChanged, InputFieldEndEdit, Open, Close, Slider など）
- IsOn：基本は true。Toggleの場合のみ true or false

各UIの要素には、UIViewRoot.SetData()経由でUISetterに渡されます。  
実際にどのように表示させるかについては、UISetterの実装に記述します。  
良く使いそうな基本的なものは既に実装済みですが、特殊なものは独自にUISetterを継承して実装します。  

## 画面間の遷移
![画像2](https://github.com/sayken/UuIiView/assets/6512883/ccb7abbd-ec2a-430f-9594-0ead78a43c74)

UI内でEventが発生すると、Presenterの上位にあるDispatherに通知されます。  
Dispatcherは渡されたCommandLinkから、処理対象のPresenterを判定しOnEventを呼び出します。

## UIPanelSettings
![画像3](https://github.com/sayken/UuIiView/assets/6512883/52c6e7dc-219d-4d0e-b464-be30a09fe82b)

まだ編集中 
