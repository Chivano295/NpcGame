
using UnityEngine;


public class Support : MonoBehaviour
{
    static Texture2D _whiteTexture;

    public static Texture2D WhiteTexture
    {
        get
        {
            _whiteTexture = new Texture2D(1, 1);
            _whiteTexture.SetPixel(0, 0, Color.white);
            _whiteTexture.Apply();

            return _whiteTexture;
        }
    }

    public static void DrawRay(Vector3 position, Vector3 direction, Color color)
    {
        if (direction.sqrMagnitude <= 0.001f)
            return;

        //draw ray with solid disk at end
        Debug.DrawRay(position, direction, color);
        UnityEditor.Handles.color = color;
        UnityEditor.Handles.DrawSolidDisc(position + direction, Vector3.up, 0.25f);
    }

    static public void DrawLine(Vector3 from, Vector3 to, Color color)
    {
        Debug.DrawLine(from, to, color);
        UnityEditor.Handles.color = color;
        UnityEditor.Handles.DrawSolidDisc(from + (to - from), Vector3.up, 0.25f);
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
        if (radius <= 0)
            return;

        UnityEditor.Handles.DrawWireDisc(position, Vector3.up, radius);
        UnityEditor.Handles.color = color;
    }

    static public void DrawSolidDisc(Vector3 position, float radius, Color color)
    {
        if (radius <= 0)
            return;

        UnityEditor.Handles.color = color;
        UnityEditor.Handles.DrawSolidDisc(position, Vector3.up, radius);
    }


    // Used https://github.com/pickles976/RTS_selection/blob/master/Utils.cs functions for drawing a Selection Box
    static public void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
        GUI.color = Color.white;
    }

    public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    public static Rect GetScreenRect(Vector3 screenPosition0, Vector3 screenPosition1)
    {
        screenPosition0.y = Screen.height - screenPosition0.y;
        screenPosition1.y = Screen.height - screenPosition1.y;

        var topLeft     = Vector3.Min(screenPosition0, screenPosition1);
        var bottomRight = Vector3.Max(screenPosition0, screenPosition1);

        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }
}
