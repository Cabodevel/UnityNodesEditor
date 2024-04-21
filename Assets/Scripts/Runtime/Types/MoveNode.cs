using UnityEngine;

namespace CodeGraph
{
    [NodeInfo("Move self", "Transform/Move self")]
    public class MoveNode : CodeGraphNode
    {
       [ExposedProperty()]
       public Vector3 direction;

        public override string OnProcess(CodeGraphAsset currentGraph)
        {
            currentGraph.gameObject.transform.position += direction;
            return base.OnProcess(currentGraph);
        }
    }
}
