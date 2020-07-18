using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World
{
    public Map map;

    public World(Vector2Int mapSize)
    {
        map = new Map(mapSize);
	}
}
