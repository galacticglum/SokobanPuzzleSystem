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

    private void Reset()
    {
        size = new Vector2(2, 2);
        offset = Vector2.zero;
    }
}
