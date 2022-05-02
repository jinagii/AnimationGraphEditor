using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class RunnerMoveNode : BaseNode
{
    public RunnerMoveNode() : base()
    {
        title = "RunnerMove";

        inputContainer.Clear();

        for (int i = 0; i < outputContainer.childCount; ++i)
        {
            outputContainer[0].AddManipulator(new EdgeConnector<Edge>(new AnimNodeConnectorListener()));
        }
    }
}

public class SampleRunnerMoveNodeSO : Node_SO
{
    public int _outPort;
}