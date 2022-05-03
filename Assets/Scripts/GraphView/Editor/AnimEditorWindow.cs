using System;
using System.IO;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;

public class AnimEditorWindow : EditorWindow
{
    AnimGraphView _graphView;
    string _path = null;

    [MenuItem("Window/Open AnimationGraphView")]
    public static void Open()
    {        
        //GetWindow<AnimEditorWindow>("AnimationGraphView");
        CreateWindow<AnimEditorWindow>("AnimationGraphView");        
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/GraphView/Editor/MyGraphViewEditorWindow.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        Button loadButton = labelFromUXML[0][0] as Button;
        if (loadButton != null)
        {
            //loadButton.RegisterCallback<ClickEvent>(OnClickLoadButton2);
            loadButton.RegisterCallback<ClickEvent>(OnClickLoadButton);
        }

        Button saveButton = labelFromUXML[0][1] as Button;
        if (saveButton != null)
        {
            saveButton.RegisterCallback<ClickEvent>(OnClickSaveButton);
        }

        Button saveAsButton = labelFromUXML[0][2] as Button;
        if (saveAsButton != null)
        {
            saveAsButton.RegisterCallback<ClickEvent>(OnClickSaveAsButton);
        }

        _graphView = new AnimGraphView()
        {
            style = { flexGrow = 1 },
            name = "graphView"
        };

        root.Add(_graphView);
    }

    void OnClickLoadButton(ClickEvent e)
    {
        //string path = EditorUtility.OpenFilePanel("Open", "Assets/Editor/Save", "asset"); 
        string path = EditorUtility.OpenFilePanel("Open", "Assets/Resources", "asset"); 
        if (path.Length != 0)
        {
            path = path.Substring(path.IndexOf("/Assets/") + 1);

            var sampleGraphViewSO = AssetDatabase.LoadAssetAtPath<GraphView_SO>(path);
            if (sampleGraphViewSO != null)
            {
                for(int i = 0;i < sampleGraphViewSO._sampleNodes.Length;i++)
                {
                    var node = sampleGraphViewSO._sampleNodes[i];
                    if (node != null)
                    {
                        //node.Serialize(rootVisualElement);
                        _graphView.AddElement(node.Serialize(rootVisualElement));
                    }
                }

                _path = path;
            }
        }    
    }

    //void OnClickLoadButton2(ClickEvent e)
    //{
    //    // .amin이나 .fbx에서 animation을 받아야함
    //    string path = EditorUtility.OpenFilePanel("Open", "", "anim"); // 다이얼로그제목, 첫디렉터리, 허용확장자

    //    if (path.Length != 0)        {

    //        //var fileContent = File.ReadAllBytes(path);
    //        path = path.Substring(path.IndexOf("/Assets/") + 1);

    //        var sampleAnimation = AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip));
    //        if (sampleAnimation != null)
    //        {
    //            var node = Activator.CreateInstance<SampleAnimClipNode>();
    //            node._animClip = (AnimationClip)sampleAnimation;
    //            node._animClipName.text = sampleAnimation.name;
    //            _graphView.AddElement(node);
    //            _path = path;
    //        }
    //    }
    //}


    void OnClickSaveButton(ClickEvent e)
    {
        if (_path == null || _path == "")
        {
            OnClickSaveAsButton(e);
        }
        else
        {
            GraphView_SO sampleGraphViewSO = ScriptableObject.CreateInstance<GraphView_SO>();
            AssetDatabase.CreateAsset(sampleGraphViewSO, _path);
            AssetDatabase.SaveAssets();

            var dataBase = Resources.Load<GraphView_SO>("NoTitle");
            dataBase._sampleNodes = new Node_SO[_graphView._rootOutput._animClipNodes.Count];


            //sampleGraphViewSO._animNodes = _graphView._rootOutput._animClipNodes;

            AssetDatabase.Refresh(); // 저장 파일 위치한 곳 새로고침 


            var enumerator = _graphView._rootOutput.parent.Children().GetEnumerator();
            while (enumerator.MoveNext())
            {

                Debug.Log($"{enumerator.Current}"); 
            }

            //AssetDatabase.CreateAsset(sampleGraphViewSO, _path);
        }
    }

    void OnClickSaveAsButton(ClickEvent e)
    {
        string path = EditorUtility.SaveFilePanel("Save", "Assets/Editor/Save", "NoTitle", "asset");



        if (path.Length != 0)
        {
            path = path.Substring(path.IndexOf("/Assets/") + 1);

            GraphView_SO sampleGraphViewSO = ScriptableObject.CreateInstance<GraphView_SO>();

            var enumerator = _graphView._rootOutput.parent.contentContainer.Children().GetEnumerator();
            while (enumerator.MoveNext())
            {
                Debug.Log($"{enumerator.Current.name}");
            }

            //AssetDatabase.CreateAsset(sampleGraphViewSO, path);
            _path = path;
        }
    }
}
