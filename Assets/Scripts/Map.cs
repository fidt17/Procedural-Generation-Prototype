using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    private Vector2Int _mapSize;
    private Cell[,] _cells;

    public Map(Vector2Int newSize) 
    {
        SetMapSize(newSize);
        _cells = new Cell[_mapSize.x, _mapSize.y];
	}

    public ref Cell[,] GetCells()
    {
        return ref _cells;
	}

    public Cell GetCellAt(Vector2Int position)
    {
        if(position.x < 0 || position.x >= _mapSize.x || position.y < 0 || position.y >= _mapSize.y)
            Debug.LogError("IndexOutOfBounds. " + position);

        return _cells[position.x, position.y];
	}

    public void SetCellTerrainAt(Vector2Int position, Cell.TERRAIN_TYPE newType)
    {
        if(position.x < 0 || position.x >= _mapSize.x || position.y < 0 || position.y >= _mapSize.y)
            Debug.LogError("IndexOutOfBounds. " + position);

        _cells[position.x, position.y].SetTerrainType(newType);  
	}

    public bool IsPositionViable(Vector2Int pos)
    {
        if(pos.x >= 0 && pos.x < _mapSize.x)
            if(pos.y >= 0 && pos.y < _mapSize.y)
                return true;

        return false;
	}
    
    public Vector2Int GetMapSize()
    {
        return _mapSize;
	}

    private void SetMapSize(Vector2Int newMapSize)
    {
        if(newMapSize.x < 0 || newMapSize.y < 0)
            Debug.LogError("Map size cannot be negative.");

        _mapSize = newMapSize;
	}
}
