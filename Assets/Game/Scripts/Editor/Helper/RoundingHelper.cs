using UnityEngine;

public static class RoundingHelper
{
    public static int FloorToEven(this int value) => value % 2 == 0 ? value : value - 1;
    public static int CeilToEven(this int value) => value % 2 == 0 ? value : value - 1;

    public static int FloorToEven(this float value) => FloorToEven(Mathf.FloorToInt(value));
    public static int CeilToEven(this float value) => CeilToEven(Mathf.CeilToInt(value));

    public static Vector2 FloorToEven(this Vector2 vector) => new Vector2(vector.x.FloorToEven(), vector.y.FloorToEven());
    public static Vector2 CeilToEven(this Vector2 vector) => new Vector2(vector.x.CeilToEven(), vector.y.CeilToEven());

    public static Vector3 FloorToEven(this Vector3 vector) => new Vector3(vector.x.FloorToEven(), vector.y.FloorToEven(), vector.z.FloorToEven());
    public static Vector3 CeilToEven(this Vector3 vector) => new Vector3(vector.x.CeilToEven(), vector.y.CeilToEven(), vector.z.CeilToEven());
}
