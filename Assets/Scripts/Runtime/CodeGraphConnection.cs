using System;

namespace CodeGraph
{
    [Serializable]
    public class CodeGraphConnection
    {
        public CodeGraphConnectionPort inputPort;
        public CodeGraphConnectionPort outputPort;

        public CodeGraphConnection(CodeGraphConnectionPort input, CodeGraphConnectionPort output) 
        {
            inputPort = input;
            outputPort = output;
        }

        public CodeGraphConnection(string inputPortId, int inputIndex, string outputPortId, int outputPortIndex)
        {
            inputPort = new CodeGraphConnectionPort(inputPortId, inputIndex);
            outputPort = new CodeGraphConnectionPort(outputPortId, outputPortIndex); 
        }

        [Serializable]
        public struct CodeGraphConnectionPort
        {
            public string nodeId;
            public int portIndex;

            public CodeGraphConnectionPort(string id, int index)
            {
                nodeId = id;
                portIndex = index;
            }
        }
    }
}
