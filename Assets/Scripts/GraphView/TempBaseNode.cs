using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class TempBaseNode : Node
{
    // 임시? Node상속 추후에 공통적인 부분 추가 예정?
}

public abstract class Node_SO : ScriptableObject
{
    public string _name;

    public virtual void Serialize(VisualElement root)
    {

    }
}