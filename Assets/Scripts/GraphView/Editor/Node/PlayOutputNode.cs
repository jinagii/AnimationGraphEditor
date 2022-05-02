using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEditor;

public class PlayOutputNode : TempBaseNode
{
    AnimationPlayableOutput _animPlayableOutput;
    AnimatorControllerPlayable _animControllerPlayable;
    AnimationMixerPlayable _animMixerPlayable;
    List<AnimationClipPlayable> _animClipPlayables = new List<AnimationClipPlayable>();

    Animator _animator;
    RuntimeAnimatorController _runtimeAnimator;

    VisualElement _dropArea;
    public Label _outputName = null;


    public PlayOutputNode() : base()
    {
        title = "PlayOutput";

        capabilities -= Capabilities.Deletable;

        AddInputPort();

        // drag and drop
        //_outputName = new Label("no animator");
        //contentContainer[0].Insert(1, _outputName);

        //_dropArea = titleContainer;
        //_dropArea.AddToClassList("droparea");
        //_dropArea.RegisterCallback<DragEnterEvent>(OnDragEnterEvent);
        //_dropArea.RegisterCallback<DragLeaveEvent>(OnDragLeaveEvent);
        //_dropArea.RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
        //_dropArea.RegisterCallback<DragPerformEvent>(OnDragPerformEvent);
        //_dropArea.RegisterCallback<DragExitedEvent>(OnDragExitedEvent);


        //outputContainer.Clear();

        //void OnDragEnterEvent(DragEnterEvent e)
        //{
        //    _dropArea.AddToClassList("dragover");
        //}

        //void OnDragLeaveEvent(DragLeaveEvent e)
        //{
        //    _dropArea.RemoveFromClassList("dragover");
        //}

        //void OnDragUpdatedEvent(DragUpdatedEvent e)
        //{
        //    _dropArea.AddToClassList("dragover");

        //    object draggedLabel = DragAndDrop.GetGenericData(DraggableLabel.s_DragDataType);
        //    if (draggedLabel != null)
        //    {
        //        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
        //    }
        //    else
        //    {
        //        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        //    }
        //}

        //void OnDragPerformEvent(DragPerformEvent e)
        //{
        //    DragAndDrop.AcceptDrag();

        //    object draggedObject = DragAndDrop.GetGenericData(DraggableLabel.s_DragDataType);
        //    if (draggedObject != null && draggedObject is DraggableLabel)
        //    {
        //        var label = (DraggableLabel)draggedObject;
        //        label.StopDraggingBox(e.localMousePosition);
        //    }
        //    else
        //    {
        //        foreach (var obj in DragAndDrop.objectReferences)
        //        {
        //            var animator = obj as Animator;
        //            if (animator != null)
        //            {
        //                _outputName.text = animator.name;
        //                _animator = animator;
        //            }
        //        }
        //    }
        //}

        //void OnDragExitedEvent(DragExitedEvent e)
        //{
        //    object draggedLabel = DragAndDrop.GetGenericData(DraggableLabel.s_DragDataType);
        //    _dropArea.RemoveFromClassList("dragover");
        //}
    }


    public void InitPlayables()
    {
        //_animator = //UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/Mecanim/StateMachineTransitions.controller");
        _animControllerPlayable = AnimatorControllerPlayable.Create(_playableGraph, _runtimeAnimator);
        //_animPlayableOutput = AnimationPlayableOutput.Create(_playableGraph, "AnimPlayableOutput", _animator);
        _animPlayableOutput = AnimationPlayableOutput.Create(_playableGraph, "AnimPlayableOutput", null); // null로 하면 재생기 없이 되는건가?

    }

    void AddInputPort()
    {
        var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(Port));
        inputPort.portName = "In";
        inputPort.AddManipulator(new EdgeConnector<Edge>(new MyPlayerNodeConnectorListener()));
        inputContainer.Add(inputPort);
    }

    void InitMixerAnimationPlayable()
    {
        _animMixerPlayable =
            AnimationMixerPlayable.Create(_playableGraph, inputContainer.childCount - 1);
    }

    void MixAnimationPlayable(List<AnimationClipPlayable> animClipPlayable)
    {
        foreach (AnimationClipPlayable anim in animClipPlayable)
        {
            _playableGraph.Connect(anim, 0, _animMixerPlayable, animClipPlayable.IndexOf(anim));
        }
    }

    void ConnectAnimationPlayable()
    {

    }

    public void RefreshInputPorts()
    {
        bool finalPortIsEmpty = false;

        // 비어 있는 중간 port 제거 및 마지막 빈 port 제거
        for (int i = inputContainer.childCount - 1; i >= 0; i--)
        {
            int count = 0;
            Port port = inputContainer[i] as Port;
            if (port != null)
            {
                var enumerator = port.connections.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    // animClipNode를 포트로부터 가져와 리스트에 등록
                    var outputPort = enumerator.Current.output as Port;
                    if (outputPort != null)
                    {
                        var animClipPlayable = outputPort.node as AnimClipNode;
                        _animClipPlayables.Add(animClipPlayable._animClipPlayable);
                    }

                    count++;
                }
            }

            if (count == 0 && i != inputContainer.childCount - 1)
            {
                inputContainer.RemoveAt(i);
            }
            else if (i == inputContainer.childCount - 1 && count == 0)
            {
                finalPortIsEmpty = true;
            }
        }

        // 마지막에 connect 없는 input 추가
        if (!finalPortIsEmpty)
            AddInputPort();

        // test
        //for (int i = inputContainer.childCount - 1; i >= 0; i--)
        //{
        //    Port port = inputContainer[i] as Port;
        //    if (port != null)
        //    {
        //        var enumerator = port.connections.GetEnumerator();

        //        var inputPort = enumerator.Current.input as Port;
        //        if (inputPort != null)
        //        {
        //            var animClipPlayable = inputPort.node as AnimClipNode;
        //            _animClipPlayables.Add(animClipPlayable._animClipPlayable);
        //        }
        //    }
        //}



    }
}

public class MyPlayerNodeConnectorListener : IEdgeConnectorListener
{
    public void OnDrop(GraphView graphView, Edge edge)
    {
        var playerNode = edge.input.node as PlayOutputNode;
        if (playerNode != null)
        {
            playerNode.RefreshInputPorts();
        }
    }

    public void OnDropOutsidePort(Edge edge, Vector2 position)
    {
        // 해당 input의 connection을 지움
        //var playerNode = edge.input.node as SamplePlayerNode;
        //var inputPort = edge.input as Port;

        //if (inputPort != null)
        //{
        //    // connection 제거
        //    var enumerator = inputPort.connections.GetEnumerator();
        //    while (enumerator.MoveNext())
        //    {
        //        var outputPort = enumerator.Current.output as Port;
        //        if (outputPort != null)
        //        {
        //            outputPort.DisconnectAll();
        //        }
        //    }

        //    inputPort.DisconnectAll();
        //}


        //if (playerNode != null)
        //{
        //    playerNode.RefreshInputPorts();
        //}
    }
}

public class PlayerNode_SO : Node_SO
{
    // in
    public int[] _inputPorts;
}