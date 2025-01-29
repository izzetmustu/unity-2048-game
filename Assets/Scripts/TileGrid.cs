using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public TileRow[] rows {get; private set;}
    public TileCell[] cells {get; private set;}
    
    public int size => cells.Length;
    public int height => rows.Length;
    public int width => size/height;

    private void Awake()
    {
        rows = GetComponentsInChildren<TileRow>();
        cells = GetComponentsInChildren<TileCell>();
    }

    private void Start()
    {
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                rows[y].cells[x].coordinates = new Vector2Int(x, y);
            }
        }
    }

    public TileCell GetCell(Vector2Int coordinates)
    {
        if(coordinates.x < 0 || coordinates.x >= width || coordinates.y < 0 || coordinates.y >= height)
        {
            return null;
        }
        return rows[coordinates.y].cells[coordinates.x];
    }


    public TileCell GetAdjacentCell(TileCell cell, Vector2Int direction)
    {
        Vector2Int coordinates = cell.coordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;
        return GetCell(coordinates);
    }

    public TileCell GetRandomEmptyCell()
    {
        int index = Random.Range(0, size);
        int startingIndex = index;
        while(cells[index].isOccupied)
        {
            index = (index + 1) % size;
            if(index == startingIndex)
            {
                return null;
            }
        }
        return cells[index];
    }
}
