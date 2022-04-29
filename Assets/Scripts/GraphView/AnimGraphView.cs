using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;

public class AnimGraphView : GraphView
{
    public PlayerNode _root;

    public AnimGraphView() : base()
    {
        //EditorUtility.SetDirty(_root);
        // 마우스 휠 -> 줌 컨트롤
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        Insert(0, new GridBackground()); // 그리드로 정리

        this.AddManipulator(new SelectionDragger()); // 선택옮기기

        _root = new PlayerNode();
        AddElement(_root); // 시작할때 PlayNode 하나 추가

        var searchWindowProvider = ScriptableObject.CreateInstance<SearchWindowProvider>();
        searchWindowProvider.Initialize(this);

        nodeCreationRequest += context =>
        {
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindowProvider);
        };
    }

    public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();
        foreach (var port in ports.ToList())
        {
            if (startAnchor.node == port.node ||
                startAnchor.direction == port.direction ||
                startAnchor.portType != port.portType)
            {
                continue;
            }

            compatiblePorts.Add(port);
        }
        return compatiblePorts;
    }

    public TempBaseNode[] GetAllNode()
    {


        return null;
    }
}
