
using UnityEngine;


    public class Support : MonoBehaviour
    {
        public static void DrawRay(Vector3 position, Vector3 direction, Color color)
        {
            //draw ray with solid disk at end
            if (direction.sqrMagnitude > 0.001f)
            {
                Debug.DrawRay(position, direction, color);
                UnityEditor.Handles.color = color;
                UnityEditor.Handles.DrawSolidDisc(position + direction, Vector3.up, 0.25f);
            }
        }
        public static void DrawLabel(Vector3 position, string label, Color color)
        {
            //draw alabel at a certain position with color
            UnityEditor.Handles.BeginGUI();
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.Label(position, label);
            UnityEditor.Handles.EndGUI();
        }
        public static void DrawWireDisc(Vector3 position, float radius, Color color)
    {
        if(radius > 0.0f)
        {
            UnityEditor.Handles.DrawWireDisc(position, Vector3.up, radius);
            UnityEditor.Handles.color = color;
        }
    }
    }
