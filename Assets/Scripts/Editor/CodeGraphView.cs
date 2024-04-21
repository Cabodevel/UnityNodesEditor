using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CodeGraph.Editor
{
    //Vista principal del editor
    public class CodeGraphView : GraphView
    {
        private CodeGraphAsset m_codeGraph;
        private SerializedObject m_serializedObject;
        private CodeGraphEditorWindow m_window;
        private CodeGraphWindowSearchProvider m_searchProvider;

        public CodeGraphEditorWindow window => m_window;
        public List<CodeGraphEditorNode> m_graphNodes;
        public Dictionary<string, CodeGraphEditorNode> m_nodeDictionary;
        public Dictionary<Edge, CodeGraphConnection> m_connectionDictionary;


        public CodeGraphView(SerializedObject serializedObject, CodeGraphEditorWindow window)
        {
            m_serializedObject = serializedObject;
            m_codeGraph = (CodeGraphAsset)serializedObject.targetObject;
            m_window = window;

            m_graphNodes = new List<CodeGraphEditorNode>();
            m_nodeDictionary = new Dictionary<string, CodeGraphEditorNode>();
            m_connectionDictionary = new Dictionary<Edge, CodeGraphConnection>();
            m_searchProvider = ScriptableObject.CreateInstance<CodeGraphWindowSearchProvider>();

            m_searchProvider.graph = this;

            this.nodeCreationRequest = ShowSearchWindow;

            var styles = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/USS/CodeGraphEditor.uss");
            styleSheets.Add(styles);

            var bg = new GridBackground();
            bg.name = "Grid";

            Add(bg);
            bg.SendToBack();

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
            this.AddManipulator(new ContentZoomer());

            DrawNodes();
            DrawConnections();
            graphViewChanged += OnGraphViewChangedEvent;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var allPorts = new List<Port>();
            var ports = new List<Port>();       

            foreach (var node in m_graphNodes)
            {
                allPorts.AddRange(node.Ports);
            }

            foreach (var port in allPorts)
            {
                if(port != startPort
                    && port.node != startPort.node
                    && startPort.direction != port.direction
                    && port.portType == startPort.portType)   
                {
                    ports.Add(port);
                }

            }

            return ports;
        }
        private void DrawNodes()
        {
            foreach (var node in m_codeGraph.Nodes){
                AddNodeToGraph(node);
            }
            Bind();
        }

        private void DrawConnections()
        {
            if(m_codeGraph.Connections != null)
            {
                foreach(var connection in m_codeGraph.Connections)
                {
                    DrawConnection(connection);
                }
            }
        }
        private void DrawConnection(CodeGraphConnection connection)
        {
            var inputNode = GetNode(connection.inputPort.nodeId);
            var outputNode = GetNode(connection.outputPort.nodeId);

            if(inputNode == null
                || outputNode == null){
                return;
            }

            var inPort = inputNode.Ports[connection.inputPort.portIndex];
            var outPort = outputNode.Ports[connection.outputPort.portIndex];
            var edge = inPort.ConnectTo(outPort);
            AddElement(edge);
            m_connectionDictionary.Add(edge, connection);
        }

        private void RemoveConnection(Edge e)
        {
            if(m_connectionDictionary.TryGetValue(e, out var connection)){
                m_codeGraph.Connections.Remove(connection);
                m_connectionDictionary.Remove(e);
            }
        }
         private CodeGraphEditorNode GetNode(string nodeId)
        {
            CodeGraphEditorNode node = null;
            m_nodeDictionary.TryGetValue(nodeId, out node);
            return node;
        }
        private void ShowSearchWindow(NodeCreationContext context)
        {
            m_searchProvider.target = (VisualElement)focusController.focusedElement;
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), m_searchProvider);
        }

        public void Add(CodeGraphNode node)
        {
            Undo.RecordObject(m_serializedObject.targetObject, "Added Node");
            
            m_codeGraph.Nodes.Add(node);
            m_serializedObject.Update();

            AddNodeToGraph(node);
            Bind();
        }

        private GraphViewChange OnGraphViewChangedEvent(GraphViewChange graphViewChange)
        {
            if(graphViewChange.movedElements != null)
            {
                Undo.RecordObject(m_serializedObject.targetObject, "Moved elements in graph");
                foreach(var editorNode in graphViewChange.movedElements.OfType<CodeGraphEditorNode>())
                {
                    editorNode.SavePosition();
                }
                
            }

            if(graphViewChange.elementsToRemove != null)
            {
                Undo.RecordObject(m_serializedObject.targetObject, "Removed elements from graph");
                var nodesToRemove = graphViewChange.elementsToRemove.OfType<CodeGraphEditorNode>().ToList();
                
                foreach(var nodeToRemove in nodesToRemove)
                {
                    RemoveNode(nodeToRemove);
                }

                foreach(var edge in graphViewChange.elementsToRemove.OfType<Edge>())
                {
                    RemoveConnection(edge);
                }
            }
            
            if(graphViewChange.edgesToCreate != null)
            {
                 Undo.RecordObject(m_serializedObject.targetObject, "Added Connections");

                 foreach (var edge in graphViewChange.edgesToCreate)
                 {
                    CreateEdge(edge);
                 }
            }

            return graphViewChange;
        }

        private void CreateEdge(Edge edge)
        {
            var inputNode = (CodeGraphEditorNode)edge.input.node;
            var inputIndex = inputNode.Ports.IndexOf(edge.input);

            var outputNode = (CodeGraphEditorNode)edge.output.node;
            var outputIndex = outputNode.Ports.IndexOf(edge.output);

            var connection = new CodeGraphConnection(inputNode.Node.id, inputIndex, outputNode.Node.id, outputIndex);

            m_codeGraph.Connections.Add(connection);
            m_connectionDictionary.Add(edge, connection);
        }

        private void RemoveNode(CodeGraphEditorNode nodeToRemove)
        {
            m_codeGraph.Nodes.Remove(nodeToRemove.Node);
            m_nodeDictionary.Remove(nodeToRemove.Node.id);
            m_graphNodes.Remove(nodeToRemove);
            m_serializedObject.Update();
        }

        private void AddNodeToGraph(CodeGraphNode node)
        {
            node.typeName = node.GetType().AssemblyQualifiedName;

            var editorNode = new CodeGraphEditorNode(node, m_serializedObject);

            editorNode.SetPosition(node.position);

            m_graphNodes.Add(editorNode);
            m_nodeDictionary.Add(node.id, editorNode);

            AddElement(editorNode);
        }

        private void Bind()
        {
            Debug.Log("Bind");
            m_serializedObject.Update();
            this.Bind(m_serializedObject);

        }
    }
}
