namespace UuIiView
{
    /// <summary>
    /// カスタムUIコンポーネント用インターフェース
    /// リストアイテム等で親要素の名前を保持するために使用
    /// </summary>
    public interface IUICustom
    {
        /// <summary>
        /// 親要素の名前
        /// リストセル内のボタン等で、どのセルからのイベントかを識別するために使用
        /// </summary>
        string ParentName { get; set; }
    }
}
