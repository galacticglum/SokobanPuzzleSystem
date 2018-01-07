using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class SokobanPuzzleController : MonoBehaviour
{
    public Matrix4x4 GridViewMatrix 
    {
        get
        {
            Vector3 rotation = Vector3.zero;
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

    public Vector2 Offset => topDownGrid ? new Vector2(transform.position.x, transform.position.z) : new Vector2(transform.position.x, transform.position.y);

    public int Width => Mathf.FloorToInt(size.x);
    public int Height => Mathf.FloorToInt(size.y);

    [SerializeField]
    private Vector2 size = new Vector2(2, 2);

    [SerializeField]
    private bool topDownGrid = true;

    private SokobanTile[,] tiles;

    public void InitializeTiles()
    {   
        tiles = new SokobanTile[Width, Height];

        Transform tileParent = new GameObject("__LEVEL__").transform;
        tileParent.SetParent(transform);
        
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Vector2 position = new Vector2(Width / 2 - x + Offset.x - 0.5f, Height / 2 - y + Offset.y - 0.5f);
                tiles[x, y] = new SokobanTile(SokobanTileType.Empty, position);
                    
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                Vector3 spawnPosition = new Vector3(position.x, position.y, 0);
                if (topDownGrid)
                {
                    spawnPosition = new Vector3(position.x, transform.position.y, position.y);
                }

                cube.transform.position = spawnPosition;
                cube.transform.SetParent(tileParent);
            }
        }
    }
}
