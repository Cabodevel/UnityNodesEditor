using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CodeGraph
{
    [CreateAssetMenu(menuName = "Code Graph/New Graph")]
    public class CodeGraphAsset : ScriptableObject
    {
        [SerializeReference]
        private List<CodeGraphNode> m_nodes;
        [SerializeField]
        private List<CodeGraphConnection> m_connections;
        private Dictionary<string, CodeGraphNode> m_NodeDictionary;

        public List<CodeGraphNode> Nodes => m_nodes;
        public List<CodeGraphConnection> Connections => m_connections;

        public GameObject gameObject;

        public CodeGraphAsset()
        {
            m_nodes = new List<CodeGraphNode>();
            m_connections = new List<CodeGraphConnection>();
        }

        public void Init(GameObject gameObject)
        {
            this.gameObject = gameObject;
            m_NodeDictionary = new Dictionary<string, CodeGraphNode>();
            foreach (var node in Nodes)
            {
                m_NodeDictionary.Add(node.id, node);
            }
        }

        public CodeGraphNode GetStartNode()
        {
            var startNodes = Nodes.OfType<StartNode>().ToList();

            if(startNodes.Count == 0){
                Debug.LogError("No start node in the graph");
                return null;
            }

            return startNodes[0];
        }

        public CodeGraphNode GetNode(string nextNodeId)
        {
            if(m_NodeDictionary.TryGetValue(nextNodeId, out var node)){
                return node;
            }

            return null;
        }

        public CodeGraphNode GetNodeFromOutput(string outputNodeId, int index)
        {
            foreach (var connection in m_connections)
            {
                if(connection.outputPort.nodeId == outputNodeId
                    && connection.outputPort.portIndex == index)
                {
                    var nodeId = connection.inputPort.nodeId;
                    var inputNode = m_NodeDictionary[nodeId];
                    return inputNode;
                }
            }
            return null;
        }
    }

}
