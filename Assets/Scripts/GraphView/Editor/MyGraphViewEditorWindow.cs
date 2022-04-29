using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class MyGraphViewEditorWindow : EditorWindow
{
    [MenuItem("Window/UI Toolkit/MyGraphViewEditorWindow")]
    public static void ShowExample()
    {
        MyGraphViewEditorWindow wnd = GetWindow<MyGraphViewEditorWindow>();
        wnd.titleContent = new GUIContent("MyGraphViewEditorWindow");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/GraphView/Editor/MyGraphViewEditorWindow.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/GraphView/Editor/MyGraphViewEditorWindow.uss");
        VisualElement labelWithStyle = new Label("Hello World! With Style");
        labelWithStyle.styleSheets.Add(styleSheet);
        root.Add(labelWithStyle);
    }

    private void OnGUI()
    {
        var dropArea = GUILayoutUtility.GetRect(100, 100);
        GUI.Box(dropArea, "Drag and Drop");

        var evt = Event.current;      //현재 이벤트 얻어오기.
        switch (evt.type)
        {
            case EventType.DragUpdated:     // 드래그 하는 동안 업데이트 되는 이벤트 타입.
            case EventType.DragPerform:    // 드래그 후 마우스 버튼 업인 상태일때.
                {
                    if (!dropArea.Contains(evt.mousePosition))    // 마우스 포지션이 박스안에 영역인지 확인.
                        break;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (evt.type == EventType.DragPerform)   // // 드래그 후 마우스 버튼 업인 상태일때.
                    {
                        DragAndDrop.AcceptDrag();   // 드래그앤 드랍을 허용함.
                        foreach (var draggedObj in DragAndDrop.objectReferences)    // objectReferences: 드래그한 오브젝트들의 레퍼런스
                        {
                            var go = draggedObj as GameObject;
                            if (go != null)
                            {
                                EditorUtility.DisplayDialog("Message", go.name, "ok"); EditorUtility.DisplayDialog("Message", go.name, "ok");
                                continue;
                            }

                            var animClip = draggedObj as AnimationClip;
                            if (animClip != null)
                            {
                                EditorUtility.DisplayDialog("Message", animClip.name, "ok"); EditorUtility.DisplayDialog("Message", animClip.name, "ok");
                                continue;
                            }
                        }
                    }
                }
                Event.current.Use();  // 이벤트 사용한 후, 이벤트의 타입을 변경해준다. (EventType.Used)
                break;
        }
    }
 }