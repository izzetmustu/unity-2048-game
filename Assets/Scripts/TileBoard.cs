using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using UnityEditor.Build.Content;

public class TileBoard : MonoBehaviour
{
    public GameManager gameManager;
    public Tile tilePrefab;
    public TileState[] tileStates;
    private TileGrid grid;
    private List<Tile> tiles;
    private bool waiting;
    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);
    }

    public void Clear()
    {
        foreach(var cell in grid.cells)
        {
            cell.tile = null;
        }

        foreach(Tile tile in tiles)
        {
            Destroy(tile.gameObject);
        }
        tiles.Clear();
    }

    public void CreateTile()
    {
        TileCell cell = grid.GetRandomEmptyCell();
        if(cell != null)
        {   
            Tile tile = Instantiate(tilePrefab, grid.transform);
            tile.SetState(tileStates[0], 2);
            tile.Spawn(cell);
            tiles.Add(tile);
        }

    }

    private void Update()
    {
        if(waiting)
        {
            return;
        }
        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            // Move up
            MoveTiles(Vector2Int.up, 0, 1, 1, 1);
        }
        else if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            // Move down
            MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1);
        }
        else if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // Move left
            MoveTiles(Vector2Int.left, 1, 1, 0, 1);
        }
        else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Move right
            MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
        }
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            CreateTile();
        }
    }

    private void MoveTiles(Vector2Int direction, int startX, int stepX, int startY, int stepY)
    {
        bool moved = false;
        for(int x = startX; (x >= 0) && (x < grid.width); x += stepX)
        {
            for(int y = startY; (y >= 0) && (y < grid.height); y += stepY)
            {
                TileCell cell = grid.GetCell(new Vector2Int(x, y));
                if(cell.isOccupied)
                {
                    moved |= MoveTile(cell.tile, direction);
                }
            }
        }
        if(moved)
        {
            StartCoroutine(WaitForAnimation());
        }
    }

    private bool CanMerge(Tile tile, Tile otherTile)
    {
        return tile.number == otherTile.number && !otherTile.locked;
    }

    private void MergeTiles(Tile tile, Tile otherTile)
    {
        tiles.Remove(tile);
        tile.Merge(otherTile.cell);

        int index = Math.Clamp(IndexOf(tile.state) + 1, 0, tileStates.Length - 1);
        int number = otherTile.number * 2;
        otherTile.SetState(tileStates[index], number);
        gameManager.IncreaseScore(number);
    }

    private int IndexOf(TileState state)
    {
        for(int i = 0; i < tileStates.Length; i++)
        {
            if(tileStates[i] == state)
            {
                return i;
            }
        }
        return -1;
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacentCell = grid.GetAdjacentCell(tile.cell, direction);
        while(adjacentCell != null)
        {
            if(adjacentCell.isOccupied)
            {
                if(CanMerge(tile, adjacentCell.tile))
                {
                    MergeTiles(tile, adjacentCell.tile);
                    return true;
                }
                else
                {
                    break;
                }
            }
            else
            {
                newCell = adjacentCell;
                adjacentCell = grid.GetAdjacentCell(adjacentCell, direction);
            }
        }
        if(newCell != null)
        {
            tile.Move(newCell);
            return true;
        }
        return false;
    }

    private IEnumerator WaitForAnimation()
    {
        waiting = true;
        yield return new WaitForSeconds(0.1f);
        waiting = false;

        foreach(Tile tile in tiles)
        {
            tile.locked = false;
        }

        CreateTile();

        if(CheckGameOver())
        {
            gameManager.GameOver();
        }
    }

    private bool CheckGameOver()
    {
        if(tiles.Count < 16)
        {
            return false;
        }

        foreach(Tile tile in tiles)
        {
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if(up != null && CanMerge(tile, up.tile))
            {
                return false;
            }
            if(down != null && CanMerge(tile, down.tile))
            {
                return false;
            }
            if(left != null && CanMerge(tile, left.tile))
            {
                return false;
            }
            if(right != null && CanMerge(tile, right.tile))
            {
                return false;
            }
        }
        return true;
    }
}
