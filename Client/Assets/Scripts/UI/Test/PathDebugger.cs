using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameEngine.Navigation
{
    public class PathDebugger : MonoBehaviour
    {
        public Transform start, end;

        GameHandler handler;

        private void Start()
        {
            handler = GameObject.Find("Game Handler").GetComponent<GameHandler>();
        }

        public void FindPath()
        {
            var result = handler.GetPath(start.position, end.position);
            if (result.success)
                GameUtility.CreateLineRenderer(Color.red, .2f, result.path);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PathDebugger))]
    public class PathDebuggerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            PathDebugger debugger = (PathDebugger)target;
            if (GUILayout.Button("½ÇÇà: FindPath"))
            {
                debugger.FindPath();
            }
        }
    }
#endif
}
