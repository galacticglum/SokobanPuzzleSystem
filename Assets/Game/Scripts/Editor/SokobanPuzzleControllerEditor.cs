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

    private void OnEnable()
    {
        propertyManager = new SerializedPropertyManager(serializedObject);
        boxBoundsHandle = new BoxBoundsHandle
        {
            axes = PrimitiveBoundsHandle.Axes.X | PrimitiveBoundsHandle.Axes.Y
        };
    }

    public override void OnInspectorGUI()
    {
        EditMode.DoEditModeInspectorModeButton(EditMode.SceneViewEditMode.Collider, "Edit Bounds", EditModeButton, () => new Bounds(boxBoundsHandle.center, boxBoundsHandle.size), this);

        propertyManager["size"].vector2Value = EditorGUILayout.Vector2Field(new GUIContent("Size"), 
            propertyManager["size"].vector2Value);

        propertyManager["offset"].vector2Value = EditorGUILayout.Vector2Field(new GUIContent("Offset"), propertyManager["offset"].vector2Value);

        propertyManager["topDownGrid"].boolValue = EditorGUILayout.Toggle(new GUIContent("Is Topdown Grid"),
            propertyManager["topDownGrid"].boolValue);

        propertyManager.Target.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        SokobanPuzzleController controller = (SokobanPuzzleController) target;
        using (new Handles.DrawingScope(controller.GridViewMatrix))
        {
            boxBoundsHandle.center = propertyManager["offset"].vector2Value;
            boxBoundsHandle.size = propertyManager["size"].vector2Value;

            EditorGUI.BeginChangeCheck();
            boxBoundsHandle.DrawHandle();
            if (!EditorGUI.EndChangeCheck()) return;

            Undo.RecordObject((SokobanPuzzleController)target,
                $"Modify {ObjectNames.NicifyVariableName(target.GetType().Name)}");

            Vector2 oldSize = propertyManager["size"].vector2Value;
            propertyManager["size"].vector2Value = boxBoundsHandle.size;
            if (propertyManager["size"].vector2Value != oldSize)
            {
                //propertyManager["offset"].vector2Value = boxBoundsHandle.center;
            }
        }

        propertyManager.Target.ApplyModifiedProperties();
    }
}
