using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Playables;
using UnityEngine.Animations;

public abstract class TempBaseNode : Node
{
    public PlayableGraph _playableGraph { get; set; }
}

public abstract class Node_SO : ScriptableObject
{
    public string _name;

    public virtual void Serialize(VisualElement root)
    {

    }
}