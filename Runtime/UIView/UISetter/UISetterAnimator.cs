using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace UuIiView
{
    [RequireComponent(typeof(Animator))]
    public class UISetterAnimator : UISetter
    {
        public override void Set(object obj)
        {
            if (obj == null) return;

            var animator = GetComponent<Animator>();

            // Dictionary型への安全な変換
            Dictionary<string, object> paramDic;
            if (obj is Dictionary<string, object> dic)
            {
                paramDic = dic;
            }
            else if (obj.ToString().StartsWith("{"))
            {
                // JSON文字列からDictionaryに変換
                paramDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(obj.ToString());
            }
            else
            {
                Debug.LogError($"[UISetterAnimator] Unsupported type: {obj.GetType()}. Expected Dictionary<string, object> or JSON string.");
                return;
            }

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
                                    Debug.LogError("Cannot parse to bool : " + paramDic[p.name]);
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
                                    Debug.LogError("Cannot parse to float : " + paramDic[p.name]);
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
                                    Debug.LogError("Cannot parse to int : " + paramDic[p.name]);
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}
