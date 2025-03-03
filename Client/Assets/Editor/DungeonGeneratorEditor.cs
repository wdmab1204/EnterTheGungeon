using UnityEditor;
using UnityEngine;

namespace GameEngine.Editor
{
    [CustomEditor(typeof(DungeonGeneratorBase), true)]
    public class DungeonGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var dungeonGenerator = (DungeonGeneratorBase)target;

            EditorGUILayout.LabelField("Dungeon config", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(DungeonGeneratorBase.RoomTemplates)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(DungeonGeneratorBase.RoomCount)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(DungeonGeneratorBase.ShowGizmos)));

            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Generate dungeon"))
            {
                dungeonGenerator.Generate();
            }
            EditorGUIUtility.labelWidth = 0f;
        }
    }
}


