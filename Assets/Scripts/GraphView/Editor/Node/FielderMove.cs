using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class FielderMoveNode : BaseNode
{
    public FielderMoveNode() : base()
    {
        title = "FielderMove";

        inputContainer.Clear(); 

        for (int i = 0; i < outputContainer.childCount; ++i)
        {
            outputContainer[0].AddManipulator(new EdgeConnector<Edge>(new AnimNodeConnectorListener()));
        }
    }
} 

public class FielderMoveNode_SO : Node_SO
{
    public int _outPort;
}