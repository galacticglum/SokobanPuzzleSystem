using UnityEngine;

public class SokobanPuzzleController : MonoBehaviour
{
    public Matrix4x4 GridViewMatrix 
    {
        get
        {
            Vector3 rotation = new Vector3(0, 0, 0);
            Vector3 position = transform.position;

            if (!topDownGrid)
            {
                return Matrix4x4.Rotate(Quaternion.Euler(rotation)) * Matrix4x4.Translate(position);
            }

            rotation.x = 90;

            float oldY = position.y;
            position.y = position.z;
            position.z = -oldY;

            return Matrix4x4.Rotate(Quaternion.Euler(rotation)) * Matrix4x4.Translate(position);
        }
    }

    [SerializeField]
    private Vector2 size = Vector2.one;
    [SerializeField]
    private Vector2 offset;
    [SerializeField]
    private bool topDownGrid = true;

    private void OnDrawGizmos()
    {
        Gizmos.matrix = GridViewMatrix;

        int width = Mathf.FloorToInt(size.x / 2f);
        int height = Mathf.FloorToInt(size.y / 2f);

        // draw the horizontal lines
        for (int x = -width; x < width + 1; x++)
        {
            InitializeGizmoColour(x);

            Vector3 a = new Vector3(x, -height, 0);
            Vector3 b = new Vector3(x, height, 0);

            Gizmos.DrawLine(a, b);
        }

        // draw the vertical lines
        for (int y = -height; y < height + 1; y++)
        {
            InitializeGizmoColour(y);

            Vector3 a = new Vector3(-width, y, 0);
            Vector3 b = new Vector3(width, y, 0);

            Gizmos.DrawLine(a, b);
        }
    }

    private static void InitializeGizmoColour(int value) => Gizmos.color = value == 0 ? Color.white : Color.green;
}
