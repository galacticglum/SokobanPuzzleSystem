using UnityEngine;

public class SokobanTile
{
    public SokobanTileType Type { get; }
    public Vector2 Position { get; }

    public SokobanTile(SokobanTileType type, Vector2 position)
    {
        Type = type;
        Position = position;
    }

    public bool IsNeighbourOf(SokobanTile target) => Vector2.Distance(Position, target.Position) == 1;
}