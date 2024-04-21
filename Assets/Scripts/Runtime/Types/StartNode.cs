using UnityEngine;

namespace CodeGraph
{
    [NodeInfo("Start Node", "Process/Start", false, true)]
    public class StartNode : CodeGraphNode
    {
        public override string OnProcess(CodeGraphAsset currentGraph)
        {
            Debug.Log("Start node");
            return base.OnProcess(currentGraph);
        }
    }
}
