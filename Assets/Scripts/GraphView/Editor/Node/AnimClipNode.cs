using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEditor;
using System.Collections.Generic;
using System;

public class AnimClipNode : BaseNode
{
    public Label _animClipName = null;
    VisualElement m_DropArea;

    AnimationClip _animClip = null;

    public AnimationClipPlayable _animClipPlayable;

    public AnimClipNode() : base()
    {
        title = "AnimClip";

        _animClipName = new Label("no filename");
        contentContainer[0].Insert(1, _animClipName);

        m_DropArea = titleContainer;
        m_DropArea.AddToClassList("droparea");
        m_DropArea.RegisterCallback<DragEnterEvent>(OnDragEnterEvent);
        m_DropArea.RegisterCallback<DragLeaveEvent>(OnDragLeaveEvent);
        m_DropArea.RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
        m_DropArea.RegisterCallback<DragPerformEvent>(OnDragPerformEvent);
        m_DropArea.RegisterCallback<DragExitedEvent>(OnDragExitedEvent);


        inputContainer.Clear();

        for (int i = 0; i < outputContainer.childCount; ++i)
        {
            outputContainer[0].AddManipulator(new EdgeConnector<Edge>(new AnimNodeConnectorListener()));
        }

        void OnDragEnterEvent(DragEnterEvent e)
        {
            m_DropArea.AddToClassList("dragover");
        }

        void OnDragLeaveEvent(DragLeaveEvent e)
        {
            m_DropArea.RemoveFromClassList("dragover");
        }

        void OnDragUpdatedEvent(DragUpdatedEvent e)
        {
            m_DropArea.AddToClassList("dragover");

            object draggedLabel = DragAndDrop.GetGenericData(DraggableLabel.s_DragDataType);
            if (draggedLabel != null)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            }
            else
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
        }

        void OnDragPerformEvent(DragPerformEvent e)
        {
            DragAndDrop.AcceptDrag();

            object draggedObject = DragAndDrop.GetGenericData(DraggableLabel.s_DragDataType);
            if (draggedObject != null && draggedObject is DraggableLabel)
            {
                var label = (DraggableLabel)draggedObject;
                label.StopDraggingBox(e.localMousePosition);
            }
            else
            {
                foreach (var obj in DragAndDrop.objectReferences)
                {
                    var animClip = obj as AnimationClip;
                    if (animClip != null)
                    {
                        _animClipName.text = animClip.name;
                        _animClip = animClip;
                        _animClipPlayable = AnimationClipPlayable.Create(_playableGraph, _animClip);
                    }
                }
            }
        }

        void OnDragExitedEvent(DragExitedEvent e)
        {
            object draggedLabel = DragAndDrop.GetGenericData(DraggableLabel.s_DragDataType);
            m_DropArea.RemoveFromClassList("dragover");
        }
    }
}

public class DraggableLabel : Label
{
    public static string s_DragDataType = "DraggableLabel";

    private bool m_GotMouseDown;
    private Vector2 m_MouseOffset;

    public DraggableLabel()
    {
        //#define USE_MOUSE_EVENTS
#if USE_MOUSE_EVENTS
            RegisterCallback<MouseDownEvent>(OnMouseDownEvent);
            RegisterCallback<MouseMoveEvent>(OnMouseMoveEvent);
            RegisterCallback<MouseUpEvent>(OnMouseUpEvent);
#else
        RegisterCallback<PointerDownEvent>(OnPointerDownEvent);
        RegisterCallback<PointerMoveEvent>(OnPointerMoveEvent);
        RegisterCallback<PointerUpEvent>(OnPointerUpEvent);
#endif
    }

    void OnMouseDownEvent(MouseDownEvent e)
    {
        if (e.target == this && e.button == 0)
        {
            m_GotMouseDown = true;
            m_MouseOffset = e.localMousePosition;
        }
    }

    void OnPointerDownEvent(PointerDownEvent e)
    {
        if (e.target == this && e.isPrimary && e.button == 0)
        {
            m_GotMouseDown = true;
            m_MouseOffset = e.localPosition;
        }
    }

    void OnMouseMoveEvent(MouseMoveEvent e)
    {
        if (m_GotMouseDown && e.pressedButtons == 1)
        {
            StartDraggingBox();
            m_GotMouseDown = false;
        }
    }

    void OnPointerMoveEvent(PointerMoveEvent e)
    {
        if (m_GotMouseDown && e.isPrimary && e.pressedButtons == 1)
        {
            StartDraggingBox();
            m_GotMouseDown = false;
        }
    }

    void OnMouseUpEvent(MouseUpEvent e)
    {
        if (m_GotMouseDown && e.button == 0)
        {
            m_GotMouseDown = false;
        }
    }

    void OnPointerUpEvent(PointerUpEvent e)
    {
        if (m_GotMouseDown && e.isPrimary && e.button == 0)
        {
            m_GotMouseDown = false;
        }
    }

    public void StartDraggingBox()
    {
        DragAndDrop.PrepareStartDrag();
        DragAndDrop.SetGenericData(s_DragDataType, this);
        DragAndDrop.StartDrag(text);
    }

    public void StopDraggingBox(Vector2 mousePosition)
    {
        style.top = -m_MouseOffset.y + mousePosition.y;
        style.left = -m_MouseOffset.x + mousePosition.x;
    }
}

public class AnimNodeConnectorListener : IEdgeConnectorListener
{
    public void OnDrop(GraphView graphView, Edge edge)
    {
        var outputPort = edge.output as Port;
        if (outputPort != null)
        {
            var enumerator = outputPort.connections.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var inputPort = enumerator.Current.input as Port;
                if (inputPort != null)
                {
                    var playOutputNode = inputPort.node as PlayOutputNode;
                    if (playOutputNode != null)
                    {
                        playOutputNode.RefreshInputPorts();
                    }
                }
            }
        }
    }

    public void OnDropOutsidePort(Edge edge, Vector2 position)
    {

    }
}

public class AnimClipNode_SO : Node_SO
{
    public string _animClipName;
    public int _outPort;

    public override void Serialize(VisualElement root)
    {

    }
}