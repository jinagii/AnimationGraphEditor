using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System.Collections.Generic;

public abstract class TempBaseNode : Node
{
    public PlayableGraph _playableGraph { get; set; }
}

public abstract class Node_SO : ScriptableObject
{
    public string _name;
    public PlayableGraph _playableGraph;
    //public GraphView _graphView;
    public virtual TempBaseNode Serialize(VisualElement root)
    {
        return null;
    }
}