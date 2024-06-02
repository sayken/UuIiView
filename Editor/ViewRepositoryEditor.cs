using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text;
using System.IO;

namespace UuIiView
{
    public class ViewRepositoryEditor : EditorWindow
    {
        [MenuItem("Tools/UuIiView/View Repository Contents")]
        static void ShowEditor()
        {
            var window = EditorWindow.GetWindow<ViewRepositoryEditor>(typeof(SceneView));
            Texture icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Packages/UuIiView/Editor/Icons/icon.png");
            window.titleContent = new GUIContent(" View Repository", icon);
        }

        GameObject canvasRoot;
        Dispatcher dispatcher;
        string[] uiPanelNames;
        int selectedIndex = 0;

        bool prepared = false;

        private void OnGUI()
        {
            // 再生チェック
            if ( Application.isPlaying == false )
            {
                EditorGUILayout.LabelField("再生中でのみ機能します");
                prepared = false;
                return;
            }

            // 必要なものが揃ってるかチェック
            if ( prepared == false )
            {
                return;
            }

            // 実処理
            EditorGUILayout.BeginHorizontal();
            selectedIndex = EditorGUILayout.Popup(new UnityEngine.GUIContent("UIPanelName"), selectedIndex, uiPanelNames);

            if ( GUILayout.Button("Show Repository") )
            {
                var presenter = (ReactivePresenter)dispatcher.GetPresenter(uiPanelNames[selectedIndex]);
                presenter.ViewModel.Log();

                // string cmd = uiPanelNames[selectedIndex] +"/Log/None/ShowLog/ParentName/Id";
                // dispatcher.Dispatch(new CommandLink(cmd));
            }
            EditorGUILayout.EndHorizontal();
        }

        void OnFocus()
        {
            if ( Application.isPlaying == false ) return;

            prepared = Prepare();
        }

        bool Prepare()
        {
            if ( canvasRoot == null ) canvasRoot = GameObject.Find("CanvasRoot");
            if ( dispatcher == null ) dispatcher = GameObject.Find("LayerController").GetComponent<Dispatcher>();

            if ( canvasRoot == null )
            {
                Debug.LogError("CanvasRootが見つかりません");
                return false;
            }
            if ( dispatcher == null )
            {
                Debug.LogError("Dispatcherが見つかりません");
                return false;
            }

            var uiPanels = canvasRoot.GetComponentsInChildren<UIPanel>(true);
            if ( uiPanels != null && uiPanels.Length > 0 )
            {
                uiPanelNames = uiPanels.Select(_=>_.name).ToArray();
            }
            if ( uiPanelNames.Length < selectedIndex )
            {
                selectedIndex = uiPanelNames.Length - 1;
            }

            return true;
        }
    }
}
