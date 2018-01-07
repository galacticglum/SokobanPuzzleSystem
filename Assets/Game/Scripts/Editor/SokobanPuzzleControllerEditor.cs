using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(SokobanPuzzleController))]
public class SokobanPuzzleControllerEditor : Editor
{
    private static GUIContent EditModeButton => EditorGUIUtility.IconContent("EditCollider");

    private SerializedPropertyManager propertyManager;
    private BoxBoundsHandle boxBoundsHandle;
    private bool canEditBounds;

    private void OnEnable()
    {
        propertyManager = new SerializedPropertyManager(serializedObject);
        boxBoundsHandle = new BoxBoundsHandle
        {
            axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y
        };

        EditMode.onEditModeStartDelegate += (editor, mode) => canEditBounds = true;
        EditMode.onEditModeEndDelegate += editor => canEditBounds = false;
    }

    public override void OnInspectorGUI()
    {
        EditMode.DoEditModeInspectorModeButton(EditMode.SceneViewEditMode.Collider, "Edit Bounds", EditModeButton, () => new Bounds(boxBoundsHandle.center, boxBoundsHandle.size), this);

        propertyManager["size"].vector2Value = EditorGUILayout.Vector2Field(new GUIContent("Size"), 
            propertyManager["size"].vector2Value);

        GUI.enabled = false;
        propertyManager["offset"].vector2Value = EditorGUILayout.Vector2Field(new GUIContent("Offset"), propertyManager["offset"].vector2Value);
        GUI.enabled = true;

        propertyManager["topDownGrid"].boolValue = EditorGUILayout.Toggle(new GUIContent("Is Topdown Grid"),
            propertyManager["topDownGrid"].boolValue);

        propertyManager.Target.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        SokobanPuzzleController controller = (SokobanPuzzleController) target;
        using (new Handles.DrawingScope(controller.GridViewMatrix))
        {
            if (canEditBounds)
            {
                boxBoundsHandle.center = Vector3.zero;
                boxBoundsHandle.size = propertyManager["size"].vector2Value;

                EditorGUI.BeginChangeCheck();
                boxBoundsHandle.DrawHandle();
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject((SokobanPuzzleController)target,
                        $"Modify {ObjectNames.NicifyVariableName(target.GetType().Name)}");

                    Vector2 oldSize = propertyManager["size"].vector2Value;

                    propertyManager["size"].vector2Value = boxBoundsHandle.size;
                    if (propertyManager["size"].vector2Value != oldSize)
                    {
                        propertyManager["offset"].vector2Value = boxBoundsHandle.center;
                    }
                }
            }
            else
            {
                Vector2 bottomLeft = new Vector2(-propertyManager["size"].vector2Value.x / 2f, -propertyManager["size"].vector2Value.y / 2f);
                Vector2 bottomRight = new Vector2(propertyManager["size"].vector2Value.x / 2f, -propertyManager["size"].vector2Value.y / 2f);

                Vector2 topLeft = new Vector2(-propertyManager["size"].vector2Value.x / 2f, propertyManager["size"].vector2Value.y / 2f);
                Vector2 topRight = new Vector2(propertyManager["size"].vector2Value.x / 2f, propertyManager["size"].vector2Value.y / 2f);

                Handles.DrawLine(bottomLeft, bottomRight);
                Handles.DrawLine(bottomLeft, topLeft);
                Handles.DrawLine(topLeft, topRight);
                Handles.DrawLine(topRight, bottomRight);
            }

            // Draw grid lines
            int width = Mathf.FloorToInt(propertyManager["size"].vector2Value.x / 2f);
            int height = Mathf.FloorToInt(propertyManager["size"].vector2Value.y / 2f);

            for (int x = -width; x < width + 1; x++)
            {
                InitializeGizmoColour(x);

                Vector3 a = new Vector3(x + propertyManager["offset"].vector2Value.x, -height + propertyManager["offset"].vector2Value.y, 0);
                Vector3 b = new Vector3(x + propertyManager["offset"].vector2Value.x, height + propertyManager["offset"].vector2Value.y, 0);

                Handles.DrawLine(a, b);
            }

            // draw the vertical lines
            for (int y = -height; y < height + 1; y++)
            {
                InitializeGizmoColour(y);

                Vector3 a = new Vector3(-width + propertyManager["offset"].vector2Value.x, y + propertyManager["offset"].vector2Value.y, 0);
                Vector3 b = new Vector3(width + propertyManager["offset"].vector2Value.x, y + propertyManager["offset"].vector2Value.y, 0);

                Handles.DrawLine(a, b);
            }


            if (canEditBounds) return;

            // Reset tint
            Handles.color = Color.white;
            for (int y = -height; y < height; y++)
            {
                for (int x = -width; x < width; x++)
                {
                    Vector3 position = new Vector3(x + propertyManager["offset"].vector2Value.x, y + propertyManager["offset"].vector2Value.y, 0);

                    // Draw tile
                    Handles.DrawSolidRectangleWithOutline(new Rect(position, new Vector2(1, 1)), new Color(0.933f, 0.95f, 0.25f, 0.1f), Color.green);

                    //// Draw picker inner
                    //Handles.DrawSolidRectangleWithOutline(new Rect(position + new Vector3(0.375f, 0.375f), new Vector2(0.25f, 0.25f)), new Color(0, 1, 0, 0.2f), new Color(0, 0, 0, 1));

                    if (Handles.Button(position + new Vector3(0.5f, 0.5f), Quaternion.identity, 0.125f, 0.125f,
                        Handles.DotHandleCap))
                    {

                    }
                }
            }
        }

        propertyManager.Target.ApplyModifiedProperties();
    }

    private static void InitializeGizmoColour(int value) => Handles.color = value == 0 ? Color.white : Color.green;
}
