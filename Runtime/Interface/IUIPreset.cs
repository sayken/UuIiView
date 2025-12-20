namespace UuIiView
{
    /// <summary>
    /// UIプリセット設定用インターフェース
    /// UIViewの初期化時に事前設定を行うコンポーネントが実装する
    /// </summary>
    public interface IUIPreset
    {
        /// <summary>
        /// プリセット設定を適用する
        /// UIViewの初期化フェーズで呼び出される
        /// </summary>
        /// <param name="obj">設定データ（型はコンポーネントによって異なる）</param>
        void Preset(object obj);
    }
}
