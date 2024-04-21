using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace CodeGraph.Editor
{
    [CustomEditor(typeof(CodeGraphAsset))]
    public class CodeGraphAssetEditor : UnityEditor.Editor
    {
        //Abre el editor con doble click
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int index)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceId);
            
            if(obj is CodeGraphAsset asset)
            {
                CodeGraphEditorWindow.Open(asset);
                return true;
            }
            return false;
        } 
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open"))
            {
                CodeGraphEditorWindow.Open((CodeGraphAsset)target);
            }
        }
    }
}
