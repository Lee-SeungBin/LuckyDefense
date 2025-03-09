using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif
namespace UI
{
    public class MythListButton : Button
    {
        private Units.Tower m_Tower;

        [SerializeField] private TextMeshProUGUI towerNameText;

        public void SetMythListButton(string mythName)
        {
            towerNameText.text = mythName;
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(MythListButton))]
    public class MythListButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();

            EditorGUILayout.LabelField("Myth List Button Setting", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("towerNameText"));
            
            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif
    
}