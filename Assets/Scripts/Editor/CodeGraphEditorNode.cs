using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

namespace CodeGraph.Editor
{
    public class CodeGraphEditorNode : Node
    {
        private CodeGraphNode m_graphNode;
        private Port m_outputPort;
        private List<Port> m_ports;
        private SerializedProperty m_serializedProperty;
        private SerializedObject m_serializedObject;
        public CodeGraphNode Node => m_graphNode;    
        public List<Port> Ports=> m_ports;
        public CodeGraphEditorNode(CodeGraphNode node, SerializedObject codeGraphObject)
        {
            this.AddToClassList("code-graph-node");

            m_graphNode = node;
            m_serializedObject = codeGraphObject;
            m_ports = new List<Port>();

            var typeInfo = node.GetType();
            var info = typeInfo.GetCustomAttribute<NodeInfoAttribute>();     

            title = info.Title.Replace("Node", "");

            string[] depths = info.MenuItem.Split('/');
            foreach (string depth in depths){
                this.AddToClassList(depth.ToLower().Replace(" ", "-"));
            }

            this.name = typeInfo.Name;

            if(info.HasFlowOutput)
            {
                CreateFlowOutputPort();
            }

            if(info.HasFlowInput)
            {
                CreateFlowInputPort();
            }

            foreach(var property in typeInfo.GetFields())
            {
                if(property.GetCustomAttribute<ExposedPropertyAttribute>() is ExposedPropertyAttribute exposedProperty)
                {
                    var field = DrawProperty(property.Name);
                    //field.RegisterValueChangeCallback(OnFieldChangeCallback);
                }
            }
            
            RefreshExpandedState();

        }

       

        private PropertyField DrawProperty(string propertyName)
        {
            if(m_serializedProperty == null)
            {
                FetchSerializedProperty();
            }

            var prop = m_serializedProperty.FindPropertyRelative(propertyName);
            var field = new PropertyField(prop);
            field.bindingPath = prop.propertyPath;
            extensionContainer.Add(field);
            return field;

        }

        private void FetchSerializedProperty()
        {
            var nodes = m_serializedObject.FindProperty("m_nodes");

            if(nodes.isArray)
            {
                var size = nodes.arraySize;

                for(int i = 0; i < size; i++)
                {
                    var element = nodes.GetArrayElementAtIndex(i);
                    var elementId = element.FindPropertyRelative("m_guid");
                    if(elementId.stringValue == m_graphNode.id) 
                    {
                        m_serializedProperty = element;
                    }
                }

            }
        }

        private void CreateFlowInputPort()
        {
            var inputPort = InstantiatePort(
                Orientation.Horizontal,
                 Direction.Input, 
                 Port.Capacity.Single,
                 typeof(PortTypes.FlowPort));
                 
                 inputPort.portName = "In";
                 inputPort.tooltip = "Flow input";

                 m_ports.Add(inputPort);
                 inputContainer.Add(inputPort);
        }

        private void CreateFlowOutputPort()
        {
            m_outputPort = InstantiatePort(
                Orientation.Horizontal,
                 Direction.Output, 
                 Port.Capacity.Single,
                 typeof(PortTypes.FlowPort));
                 
                 m_outputPort.portName = "Out";
                 m_outputPort.tooltip = "Flow output";

                 m_ports.Add(m_outputPort);
                 outputContainer.Add(m_outputPort);
        }

        public void SavePosition()
        {
            m_graphNode.SetPosition(GetPosition());
        }
    }
}
