using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
	public static GameManagerScript Instance;
	public World world;	
	public int seed = 1200;
	public float fBias = 2f;
	public int octavesCount = 1;

	private void Awake()
	{
		if(Instance != null) {
			Debug.LogError("Only one GameManagerScript can exist at a moment.", gameObject);
			Destroy(Instance.gameObject);
		}

		Instance = this;
	}

	private void Start()
	{
		CreateWorld(new Vector2Int(50, 50));
		MapCreationScript.Instance.pn = new PerlinNoise((int)Random.Range(1000, 2000));
		seed = (int) Random.Range(1, 1000);
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.P)) {
			
			MapCreationScript.Instance.ClearWorld();

			Vector2Int startPos = new Vector2Int((int) Random.Range(0, 49), (int) Random.Range(0, 49));
			Vector2Int targetPos = new Vector2Int((int) Random.Range(0, 49), (int) Random.Range(0, 49));

			Cell startCell = world.map.GetCellAt(startPos);
			Cell targetCell = world.map.GetCellAt(targetPos);

			AStarPathfinder aPathfinder = new AStarPathfinder(world.map);

			bool result = aPathfinder.FindPath(startCell, targetCell);

			foreach(ANode node in aPathfinder.visited) {
				world.map.GetCellAt(new Vector2Int(node.x, node.y)).GetGameObject().GetComponent<SpriteRenderer>().color = Color.black;
			}

			foreach(ANode node in aPathfinder.path) {
				world.map.GetCellAt(new Vector2Int(node.x, node.y)).GetGameObject().GetComponent<SpriteRenderer>().color = Color.red;
			}
		}
	}

	public void CreateWorld(Vector2Int dimensions)
	{
		DeleteWorld();
		world = new World(dimensions);
		MapCreationScript.Instance.CreateMap(ref world.map);
		LocateCamera();
	}

	private void DeleteWorld()
	{
		MapCreationScript.Instance.DeleteWorld();
	}

	private void LocateCamera()
	{
		float cameraSize = Mathf.Max(world.map.GetMapSize().x, world.map.GetMapSize().y) * 0.6f;
		float cameraX = world.map.GetMapSize().x / 2;
		float cameraY = world.map.GetMapSize().y / 2;
		float cameraZ = -10;

		Camera.main.transform.position = new Vector3(cameraX, cameraY, cameraZ);
		Camera.main.GetComponent<Camera>().orthographicSize = cameraSize;
	}
}
