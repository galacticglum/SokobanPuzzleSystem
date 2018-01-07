using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(SokobanPuzzleController))]
public class SokobanPuzzleControllerEditor : Editor
{
    private static GUIContent EditModeButton => EditorGUIUtility.IconContent("EditCollider");

    private SokobanPuzzleController sokobanPuzzleController;
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

        sokobanPuzzleController = (SokobanPuzzleController)target;
    }

    public override void OnInspectorGUI()
    {
        EditMode.DoEditModeInspectorModeButton(EditMode.SceneViewEditMode.Collider, "Edit Bounds", EditModeButton, () => new Bounds(boxBoundsHandle.center, boxBoundsHandle.size), this);

        propertyManager["size"].vector2Value = EditorGUILayout.Vector2Field(new GUIContent("Size"), 
            propertyManager["size"].vector2Value);

        GUI.enabled = false;
        EditorGUILayout.Vector3Field(new GUIContent("Offset"), sokobanPuzzleController.transform.position);
        GUI.enabled = true;

        propertyManager["topDownGrid"].boolValue = EditorGUILayout.Toggle(new GUIContent("Is Topdown Grid"),
            propertyManager["topDownGrid"].boolValue);

        propertyManager.Target.ApplyModifiedProperties();

        EditorGUILayout.BeginHorizontal();
        Rect buttonRect = GUILayoutUtility.GetRect(new GUIContent("Generate Level"), GUI.skin.button);
        if (GUI.Button(buttonRect, "Generate Level"))
        {
            ((SokobanPuzzleController)target).InitializeTiles();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void OnSceneGUI()
    {
        using (new Handles.DrawingScope(sokobanPuzzleController.GridViewMatrix))
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

                    propertyManager["size"].vector2Value = boxBoundsHandle.size;
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

            int halfWidth = sokobanPuzzleController.Width / 2;
            int halfHeight = sokobanPuzzleController.Height / 2;

            for (int x = -halfWidth; x < halfWidth + 1; x++)
            {
                InitializeGizmoColour(x);

                Vector3 a = new Vector3(x, -halfHeight, 0);
                Vector3 b = new Vector3(x, halfHeight, 0);

                Handles.DrawLine(a, b);
            }

            // draw the vertical lines
            for (int y = -halfHeight; y < halfHeight + 1; y++)
            {
                InitializeGizmoColour(y);

                Vector3 a = new Vector3(-halfWidth, y, 0);
                Vector3 b = new Vector3(halfWidth, y, 0);

                Handles.DrawLine(a, b);
            }

            if (!canEditBounds)
            {
                // Reset tint
                Handles.color = Color.white;
                for (int y = -halfHeight; y < halfHeight; y++)
                {
                    for (int x = -halfWidth; x < halfWidth; x++)
                    {
                        Vector3 position = new Vector3(x, y, 0);

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
        }

        propertyManager.Target.ApplyModifiedProperties();
    }

    private static void InitializeGizmoColour(int value) => Handles.color = value == 0 ? Color.white : Color.green;
}
