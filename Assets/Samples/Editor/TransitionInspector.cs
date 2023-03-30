using UnityEngine;
using UnityEditor;
using DG.Tweening;
using System.Collections.Generic;

namespace UuIiView.Sample
{
[CustomEditor(typeof(Transition))]
public class TransitionInspector : Editor
{
        protected Dictionary<string, SerializedProperty> prop = new Dictionary<string, SerializedProperty>();

        protected void Add(params string[] names)
        {
            foreach (var name in names)
            {
                prop.Add(name, serializedObject.FindProperty(name));
            }
        }

        private void OnEnable()
    {
        Add(
            nameof(Transition.canvasGroup),
            nameof(Transition.inTweenType),
            nameof(Transition.inDuration),
            nameof(Transition.inEaseType),
            nameof(Transition.outTweenType),
            nameof(Transition.outDuration),
            nameof(Transition.outEaseType),
            nameof(Transition.scaleRate),

            nameof(Transition.anim),
            nameof(Transition.inAnimClip),
            nameof(Transition.outAnimClip)
        );
    }


    public override void OnInspectorGUI()
    {
        var t = target as Transition;

        DrawDefaultInspector();

        BoldStyle();

        serializedObject.Update();

        // DOTween選択時
        if ( t.inType == Transition.TransitionType.Tween || t.outType == Transition.TransitionType.Tween )
        {
            GUILayout.Space(10f);
            GUILayout.Label("[ DOTween ]", bold);

            bool view = true;

            if ( t.inTweenType==Transition.TweenType.Fade || t.inTweenType==Transition.TweenType.Scale ||
                 t.outTweenType==Transition.TweenType.Fade || t.outTweenType==Transition.TweenType.Scale)
            {
                var canvasGroup = t.GetComponent<CanvasGroup>();

                if ( canvasGroup==null )
                {
                    view = false;
                    if (GUILayout.Button("Add CanvasGroup"))
                    {
                        prop["canvasGroup"].objectReferenceValue = (CanvasGroup)t.gameObject.AddComponent(typeof(CanvasGroup));
                        view = true;
                    }
                }
                else if ( t.canvasGroup == null )
                {
                    prop["canvasGroup"].objectReferenceValue = canvasGroup;
                }
            }

            if ( view )
            {
                if (t.inType == Transition.TransitionType.Tween)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("In", GUILayout.Width(100));

                    GUILayout.Label("Type", GUILayout.Width(50));
                    prop["inTweenType"].enumValueIndex = (int)(Transition.TweenType)EditorGUILayout.EnumPopup((Transition.TweenType)prop["inTweenType"].enumValueIndex, GUILayout.Width(100));
                    GUILayout.Space(30);

                    GUILayout.Label("Duration", GUILayout.Width(70));
                    prop["inDuration"].floatValue = EditorGUILayout.FloatField(prop["inDuration"].floatValue, GUILayout.Width(40));
                    GUILayout.Space(30);

                    GUILayout.Label("Ease", GUILayout.Width(50));
                    prop["inEaseType"].enumValueIndex = (int)(Ease)EditorGUILayout.EnumPopup((Ease)prop["inEaseType"].enumValueIndex, GUILayout.Width(100));
                    EditorGUILayout.EndHorizontal();
                }

                if (t.outType == Transition.TransitionType.Tween)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Out", GUILayout.Width(100));

                    GUILayout.Label("Type", GUILayout.Width(50));
                    prop["outTweenType"].enumValueIndex = (int)(Transition.TweenType)EditorGUILayout.EnumPopup((Transition.TweenType)prop["outTweenType"].enumValueIndex, GUILayout.Width(100));
                    GUILayout.Space(30);

                    GUILayout.Label("Duration", GUILayout.Width(70));
                    prop["outDuration"].floatValue = EditorGUILayout.FloatField(prop["outDuration"].floatValue, GUILayout.Width(40));
                    GUILayout.Space(30);

                    GUILayout.Label("Ease", GUILayout.Width(50));
                    prop["outEaseType"].enumValueIndex = (int)(Ease)EditorGUILayout.EnumPopup((Ease)prop["outEaseType"].enumValueIndex, GUILayout.Width(100));
                    EditorGUILayout.EndHorizontal();
                }

                if (t.inTweenType == Transition.TweenType.Scale || t.outTweenType == Transition.TweenType.Scale)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("ScaleRate", GUILayout.Width(100));
                    prop["scaleRate"].floatValue = EditorGUILayout.FloatField(prop["scaleRate"].floatValue, GUILayout.Width(70));
                    EditorGUILayout.EndHorizontal();
                }
            }
        }




        // Animation選択時
        if (t.inType == Transition.TransitionType.LegacyAnimation || t.outType == Transition.TransitionType.LegacyAnimation)
        {
            GUILayout.Space(10f);
            GUILayout.Label("[ Animation (Legacy) ]", bold);
            var anim = t.GetComponent<Animation>();
            if (anim == null)
            {
                if ( GUILayout.Button("Add Animation" ) )
                {
                    anim = (Animation)t.gameObject.AddComponent(typeof(Animation));
                    anim.playAutomatically = false;
                    prop["anim"].objectReferenceValue = anim;
                }
            }
            else
            {
                if ( t.anim == null )
                {
                    anim = (Animation)t.GetComponent<Animation>();
                    prop["anim"].objectReferenceValue = anim;
                }

                if (t.inType == Transition.TransitionType.LegacyAnimation)
                {
                    prop["inAnimClip"].objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField("In", prop["inAnimClip"].objectReferenceValue, typeof(AnimationClip), true);
                    if ( t.inAnimClip==null || !t.inAnimClip.legacy )
                    {
                        EditorGUILayout.HelpBox("AnimationClip is null or not Legacy", MessageType.Warning);
                    }
                }
                if (t.outType == Transition.TransitionType.LegacyAnimation)
                {
                    prop["outAnimClip"].objectReferenceValue = (AnimationClip)EditorGUILayout.ObjectField("Out", prop["outAnimClip"].objectReferenceValue, typeof(AnimationClip), true);
                    if ( t.outAnimClip==null || !t.outAnimClip.legacy)
                    {
                        EditorGUILayout.HelpBox("AnimationClip is null or not Legacy", MessageType.Warning);
                    }
                }
            }
        }

        // 動作確認ボタン（デバッグ用）
        if (EditorApplication.isPlaying)
        {
            GUILayout.Space(20f);
            GUILayout.Label("Layer is : "+ ((t.isTransitionIn) ? "In" : "Out"));
            GUILayout.Space(20f);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("[ Debug func ]  ");
            if (GUILayout.Button("Reset", GUILayout.Width(150)))
            {
                t.Reset();
            }
            if (GUILayout.Button("Play Transiton In", GUILayout.Width(150)))
            {
                t.TransitionIn(() => { Debug.Log("Transition In Completed"); });
            }
            if (GUILayout.Button("Play Transiton Out", GUILayout.Width(150)))
            {
                t.TransitionOut(() => { Debug.Log("Transition Out Completed"); });
            }
            EditorGUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();
    }

    GUIStyle bold = new GUIStyle();

    void BoldStyle()
    {
        bold.fontStyle = FontStyle.Bold;
        bold.normal.textColor = new Color(0.7f, 0.7f, 0.7f);
    }
}
    }