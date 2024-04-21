using System;

namespace CodeGraph
{
    public class NodeInfoAttribute : Attribute
    {
        private string m_nodeTitle;
        private string m_menuItem;
        private bool m_hasFlowInput;
        private bool m_hasFlowOutput;

        public string MenuItem => m_menuItem;
        public string Title => m_nodeTitle;
        public bool HasFlowInput => m_hasFlowInput;
        public bool HasFlowOutput => m_hasFlowOutput;

        public NodeInfoAttribute(string title, string menuItem = "", bool hasFlowInput = true, bool hasFlowOutput = true)
        {
            m_nodeTitle = title;
            m_menuItem = menuItem;
            m_hasFlowInput = hasFlowInput;
            m_hasFlowOutput = hasFlowOutput;
        }
    }
}
