using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerNode : TempBaseNode
{
    public PlayerNode() : base()
    {
        title = "PlayOutput";

        capabilities -= Capabilities.Deletable;

        AddInputPort();        
    }

    void AddInputPort()
    {
        var inputPort = Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(Port));
        inputPort.portName = "In";
        inputPort.AddManipulator(new EdgeConnector<Edge>(new MyPlayerNodeConnectorListener()));
        inputContainer.Add(inputPort);
    }

    public void RefreshInputPorts()
    {
        bool finalPortIsEmpty = false;

        // 비어 있는 중간 port 제거 및 마지막 빈 port 제거
        for(int i = inputContainer.childCount - 1; i >= 0;i--)
        {
            int count = 0;
            Port port = inputContainer[i] as Port;
            if(port != null)
            {
                var enumerator = port.connections.GetEnumerator();
                while(enumerator.MoveNext())
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

        // 마지막에 connect 없는 input 추가
        if (!finalPortIsEmpty)
            AddInputPort();
    }
}

public class MyPlayerNodeConnectorListener : IEdgeConnectorListener
{
    public void OnDrop(GraphView graphView, Edge edge)
    {
        var playerNode = edge.input.node as PlayerNode;
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

public class PlayerNodeSO : Node_SO
{
    // in
    public int[] _inputPorts;
}