using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace UuIiView
{
    /// <summary>
    /// Animatorパラメータを設定するためのUISetter
    /// Dictionary/JSON形式でBool/Float/Intパラメータを一括設定可能
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class UISetterAnimator : UISetter
    {
        /// <summary>
        /// Animatorパラメータを設定する
        /// </summary>
        /// <param name="obj">Dictionary&lt;string, object&gt;またはJSON文字列</param>
        public override void Set(object obj)
        {
            if (obj == null) return;

            var animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError($"[UISetterAnimator] {gameObject.name}: Animatorコンポーネントが見つかりません。");
                return;
            }

            // Dictionary型への安全な変換
            Dictionary<string, object> paramDic = ParseToDictionary(obj);
            if (paramDic == null) return;

            foreach (var p in animator.parameters)
            {
                if (paramDic.ContainsKey(p.name))
                {
                    switch (p.type)
                    {
                        case AnimatorControllerParameterType.Bool:
                            {
                                if (bool.TryParse(paramDic[p.name].ToString(), out bool val))
                                {
                                    animator.SetBool(p.name, val);
                                }
                                else
                                {
                                    Debug.LogError($"[UISetterAnimator] {gameObject.name}: boolに変換できません: {paramDic[p.name]}");
                                }
                            }
                            break;
                        case AnimatorControllerParameterType.Float:
                            {
                                if (float.TryParse(paramDic[p.name].ToString(), out float val))
                                {
                                    animator.SetFloat(p.name, val);
                                }
                                else
                                {
                                    Debug.LogError($"[UISetterAnimator] {gameObject.name}: floatに変換できません: {paramDic[p.name]}");
                                }
                            }
                            break;
                        case AnimatorControllerParameterType.Int:
                            {
                                if (int.TryParse(paramDic[p.name].ToString(), out int val))
                                {
                                    animator.SetInteger(p.name, val);
                                }
                                else
                                {
                                    Debug.LogError($"[UISetterAnimator] {gameObject.name}: intに変換できません: {paramDic[p.name]}");
                                }
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// オブジェクトをDictionaryに変換する
        /// </summary>
        /// <param name="obj">変換元オブジェクト</param>
        /// <returns>変換されたDictionary（失敗時はnull）</returns>
        private Dictionary<string, object> ParseToDictionary(object obj)
        {
            if (obj is Dictionary<string, object> dic)
            {
                return dic;
            }

            if (obj.ToString().StartsWith("{"))
            {
                try
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, object>>(obj.ToString());
                }
                catch (JsonException e)
                {
                    Debug.LogError($"[UISetterAnimator] {gameObject.name}: JSONパースエラー: {e.Message}");
                    return null;
                }
            }

            Debug.LogError($"[UISetterAnimator] {gameObject.name}: サポートされていない型です: {obj.GetType()}。Dictionary<string, object>またはJSON文字列が必要です。");
            return null;
        }
    }
}
