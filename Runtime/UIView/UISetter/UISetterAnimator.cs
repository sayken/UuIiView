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
            var animator = GetComponent<Animator>();
            var paramDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(obj.ToString());

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
