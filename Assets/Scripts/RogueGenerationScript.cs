using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueGenerationScript : MonoBehaviour
{
    public static RogueGenerationScript Instance;

    public GameObject tilePrefab;
    public Color floorColor, wallColor;
    
    private void Awake()
    {
        if(Instance != null) {
            Debug.LogError("Only one RogueGenerationScript can exist at a time.", gameObject);
            Destroy(gameObject);
		}

        Instance = this;
	}

    public void SetSeed(int seed)
    {
        Random.seed = seed;
	}

	#region Rectangle generation

    public void CreateRooms(int count, Vector2Int minMaxWidth, Vector2Int minMaxHeight) {

        if(GameManagerScript.Instance.world == null)
            return;
        
        List<RectRoom> createdRooms = new List<RectRoom>();

        for (int i = 0; i < count; i++) {
            RectRoom room = CreateRoom(minMaxWidth, minMaxHeight);

            if (room != null)
		        createdRooms.Add(room);
        }

        CreatePathways(createdRooms);
	}

    public void CreatePathways(List<RectRoom> rooms) {
        
        if(rooms.Count <= 1)
            return;

        for (int i = 0; i < rooms.Count - 1; i++) {

            AStarPathfinder pathfinder = new AStarPathfinder(GameManagerScript.Instance.world.map);


            RectRoom startRoom = rooms[i];
            RectRoom nextRoom = rooms[i + 1];

            List<Cell> listOfRoomCellsStart = startRoom.GetListOfRoomCells();
            Cell startCell = listOfRoomCellsStart[Random.Range(0, listOfRoomCellsStart.Count - 1)];

            List<Cell> listOfRoomCellsNext = nextRoom.GetListOfRoomCells();
            Cell nextCell = listOfRoomCellsNext[Random.Range(0, listOfRoomCellsNext.Count - 1)];

            bool b = pathfinder.FindPath(startCell, nextCell);

            if(!b)
                continue;

            for (int j = 0; j < pathfinder.path.Count - 1; j++) {

                Cell node = GameManagerScript.Instance.world.map.GetCellAt(new Vector2Int(pathfinder.path[j].x, pathfinder.path[j].y));
                Cell nextNode = GameManagerScript.Instance.world.map.GetCellAt(new Vector2Int(pathfinder.path[j + 1].x, pathfinder.path[j + 1].y));

                Vector2Int position1 = node.GetPosition();
                Vector2Int position2 = nextNode.GetPosition();

                if(listOfRoomCellsStart.Contains(node) || listOfRoomCellsNext.Contains(node))
                    continue;

                GameManagerScript.Instance.world.map.GetCellAt(position1).GetGameObject().GetComponent<SpriteRenderer>().color = floorColor;

                Vector2Int dif = position1 - position2;

                if(dif == new Vector2Int(0, 1) || dif == new Vector2Int(0, -1)) {
                    
                    Vector2Int leftNode = new Vector2Int(position1.x - 1, position1.y);
                    Vector2Int rightNode = new Vector2Int(position1.x + 1, position1.y);
                    
                    if(GameManagerScript.Instance.world.map.IsPositionViable(leftNode)) {
                        GameManagerScript.Instance.world.map.GetCellAt(new Vector2Int(leftNode.x, leftNode.y)).GetGameObject().GetComponent<SpriteRenderer>().color = wallColor;
                        GameManagerScript.Instance.world.map.GetCellAt(new Vector2Int(leftNode.x, leftNode.y)).SetTerrainType(Cell.TERRAIN_TYPE.wall);
					}

                    if(GameManagerScript.Instance.world.map.IsPositionViable(rightNode)) {
                        GameManagerScript.Instance.world.map.GetCellAt(new Vector2Int(rightNode.x, rightNode.y)).GetGameObject().GetComponent<SpriteRenderer>().color = wallColor;
                        GameManagerScript.Instance.world.map.GetCellAt(new Vector2Int(rightNode.x, rightNode.y)).SetTerrainType(Cell.TERRAIN_TYPE.wall);
					}
				}
			}
		}
	}

    public RectRoom CreateRoom(Vector2Int minMaxWidth, Vector2Int minMaxHeight)
    {
        if(GameManagerScript.Instance.world == null)
            return null;

        Map map = GameManagerScript.Instance.world.map;

        int mapWidth = map.GetMapSize().x;
        int mapHeight = map.GetMapSize().y;

        int minimumSpace = 10;


        int tryCount = 0;

        START_OF_LOOP:
        tryCount++;

        if(tryCount > 1000000)
            return null;

        int X = Random.Range(0, mapWidth);
        int Y = Random.Range(0, mapHeight);

        Vector2Int position = new Vector2Int(X, Y);

		int width = Random.Range(minMaxWidth.x, minMaxWidth.y);
        int height = Random.Range(minMaxHeight.x, minMaxHeight.y);

        Vector2Int dimensions = new Vector2Int(width, height);

        for(int x = position.x; x < position.x + width; x++) {
            for(int y = position.y; y < position.y + height; y++) {
                    
                Vector2Int checkPos = new Vector2Int(x, y);

                if(map.IsPositionViable(checkPos) == false)
                    goto START_OF_LOOP;

                    Cell c = map.GetCellAt(checkPos);

                if(c.GetTerrainType() != Cell.TERRAIN_TYPE.empty)
                    goto START_OF_LOOP;
			}
		}

        for(int x = position.x; x < position.x + width; x++) {
            for(int y = position.y; y < position.y + height; y++) {
                    
                Vector2Int checkPos = new Vector2Int(x, y);
                Cell c = map.GetCellAt(checkPos);


                if(x == position.x || x == position.x + width - 1 || y == position.y || y == position.y + height - 1) {
                        
                    c.SetTerrainType(Cell.TERRAIN_TYPE.wall);
                    c.GetGameObject().GetComponent<SpriteRenderer>().color = wallColor;
			    } else {
                    c.SetTerrainType(Cell.TERRAIN_TYPE.floor);
                    c.GetGameObject().GetComponent<SpriteRenderer>().color = floorColor;
			    }
			}
		}

        RectRoom room = new RectRoom(position, dimensions);
        return room;    
	}

	#endregion
}

public class RectRoom {

    public Vector2Int position;
    public Vector2Int dimensions;

    public RectRoom(Vector2Int position, Vector2Int dimensions) {

        this.position = position;
        this.dimensions = dimensions;
	}

    //Returns a list off all floor cells(without walls)
    public List<Cell> GetListOfRoomCells()
    {
        List<Cell> cells = new List<Cell>();

        for(int x = position.x + 1; x < position.x + dimensions.x - 1; x++) {
            for(int y = position.y + 1; y < position.y + dimensions.y - 1; y++) {
                
                Cell c = GameManagerScript.Instance.world.map.GetCellAt(new Vector2Int(x, y));
                cells.Add(c);
			}
		}

        return cells;
	}
}