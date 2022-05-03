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
    public List<AnimClipNode> _animClipNodes = new List<AnimClipNode>();
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
        inputPort.AddManipulator(new EdgeConnector<Edge>(new PlayOutputNodeConnectorListenerㅒ()));
        inputContainer.Add(inputPort);
    }

    void RemoveInputPort(int index)
    {

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

    public void SetOutputTarget(Animator animator)
    {
        if (_animPlayableOutput.GetTarget() != animator)
        {
            _animPlayableOutput.SetTarget(animator);
        }
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

        // AnimNode들 다시 정렬
        ReCollectInputNodes();

        // 마지막에 connect 없는 input 추가
        if (!finalPortIsEmpty)
        {
            AddInputPort();
        }
    }

    public void ReCollectInputNodes()
    {
        // 커넥션 달라지면 리프레시하고 다시 정렬하기
        _animClipNodes.Clear();

        //for (int i = inputContainer.childCount - 1; i >= 0; i--)
        int nodeCount = 0;
        for (int i = 0; i < inputContainer.childCount; i++)
        {
            Port port = inputContainer[i] as Port;
            if (port != null)
            {
                var enumerator = port.connections.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    // animClipNode를 포트로부터 가져와 리스트에 등록
                    var outputPort = enumerator.Current.output as Port;
                    if (outputPort.node != null)
                    {
                        var animClipPlayable = outputPort.node as AnimClipNode;
                        animClipPlayable._animClipIndex = nodeCount;
                        _animClipNodes.Add(animClipPlayable);                        
                    }

                    nodeCount++;
                }
            }
        }
    }

}

public class PlayOutputNodeConnectorListenerㅒ : IEdgeConnectorListener
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

public class PlayOutputNode_SO : Node_SO
{
    // in
    public int[] _inputPorts;
}