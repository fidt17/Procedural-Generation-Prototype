using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
	public enum TERRAIN_TYPE {
		 
		empty,
		earth,
		sand,
		water,
		wall,
		floor
	}

	private GameObject _GO;
	private Vector2Int _position;
	private TERRAIN_TYPE _terrain;
	private bool _isWalkable;

	public Cell(Vector2Int newPosition, GameObject newGO)
	{
		SetPosition(newPosition);
		SetGameObject(newGO);
		SetWalkable(true);
		_terrain = TERRAIN_TYPE.empty;
	}

	public TERRAIN_TYPE GetTerrainType()
	{
		return _terrain;
	}

	public void SetTerrainType(TERRAIN_TYPE newTerrain)
	{
		_terrain = newTerrain;
	}

	private void SetGameObject(GameObject newGO)
	{
		_GO = newGO;
	}

	public GameObject GetGameObject()
	{
		return _GO;
	}

	public Vector2Int GetPosition()
	{
		return _position;
	}

	private void SetPosition(Vector2Int newPosition)
	{
		_position = newPosition;
	}

	public bool IsWalkable()
	{
		return _isWalkable;
	}

	private void SetWalkable(bool b)
	{
		_isWalkable = b;
	}
}
