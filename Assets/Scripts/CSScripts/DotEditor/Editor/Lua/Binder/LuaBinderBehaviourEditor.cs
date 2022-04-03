using DotEngine.Lua;
using UnityEditor;

namespace LuaEditor
{
    [CustomEditor(typeof(LuaBinderBehaviour), false)]
    public class LuaBinderBehaviourEditor : Editor
    {
        SerializedProperty bindScriptProperty;
        protected virtual void OnEnable()
        {
            bindScriptProperty = serializedObject.FindProperty("m_Binder");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.PropertyField(bindScriptProperty);
                }
                EditorGUILayout.EndVertical();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}