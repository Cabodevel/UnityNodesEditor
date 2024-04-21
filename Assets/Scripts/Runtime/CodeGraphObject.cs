using UnityEngine;

namespace CodeGraph
{
    public class CodeGraphObject : MonoBehaviour
    {
        [SerializeField]
        private CodeGraphAsset m_graphAsset;
        private CodeGraphAsset graphInstance;

        private void OnEnable() 
        {
            graphInstance = Instantiate(m_graphAsset);
            ExecuteAsset();
        }

        private void ExecuteAsset()
        {
            graphInstance.Init(gameObject);
            var startNode = graphInstance.GetStartNode(); 
            ProcessAndMoveToNextNode(startNode);
        }

        private void ProcessAndMoveToNextNode(CodeGraphNode startNode)
        {
            var nextNodeId = startNode.OnProcess(graphInstance);

            if(!string.IsNullOrEmpty(nextNodeId))
            {
                var node = graphInstance.GetNode(nextNodeId);
                ProcessAndMoveToNextNode(node);
            }
        }
    }
}
