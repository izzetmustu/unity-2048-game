using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int coordinates {get; set;}
    public Tile tile {get; set;}
    public bool isOccupied => tile != null;
    public bool empty => tile == null;
}
