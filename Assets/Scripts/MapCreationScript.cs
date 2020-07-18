using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreationScript : MonoBehaviour
{
    public static MapCreationScript Instance;

    public GameObject earthCellPrefab, sandCellPrefab, waterCellPrefab, frameCellPrefab;
    public Transform cellsParent;
    public bool render = true;
    public int seed = 10;

    public Color seaColorMin, seaColorMax;
    public Color groundColorMin, groundColorMax;
    public Color mountainColorMin, mountainColorMax;

    public PerlinNoise pn;

    private Map _map;
    private Vector2Int _mapSize;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("Only one MapCreation Script can exist at a time.", gameObject);

        Instance = this;
	}

    public void CreateMap(ref Map map)
    {
        _map = map;
        _mapSize = _map.GetMapSize();
            
        Cell[,] cells = _map.GetCells();

        for (int x = 0; x < _mapSize.x; x++) {
            for (int y = 0; y < _mapSize.y; y++) {
                
                Vector2Int position = new Vector2Int(x, y);

                GameObject cellGO = Instantiate(frameCellPrefab);
                cellGO.name = "Cell " + x + "_" + y;
                cellGO.transform.parent = cellsParent;
                cellGO.transform.position = new Vector3(position.x, position.y, 0);
                cellGO.GetComponent<SpriteRenderer>().color = Color.white;

                Cell newCell = new Cell(position, cellGO);
                newCell.SetTerrainType(Cell.TERRAIN_TYPE.empty);
                cells[x,y] = newCell;
			}
		}

        CreateFrameAroundTheMap();
	} 

    public void DeleteWorld()
    {
        StopAllCoroutines();

        foreach(Transform t in cellsParent)
            Destroy(t.gameObject);
	}

    public void ClearWorld()
    {
        Cell[,] cells = _map.GetCells();

        for(int x = 0; x < _map.GetMapSize().x; x++) {
            for(int y = 0; y < _map.GetMapSize().y; y++) {
                Cell c = cells[x,y];

                c.SetTerrainType(Cell.TERRAIN_TYPE.empty);
                c.GetGameObject().GetComponent<SpriteRenderer>().color = Color.white;
			}
		}
	}

    private void CreateFrameAroundTheMap()
    {
        for(int x = -1; x <= _map.GetMapSize().x; x++) {
            
            GameObject cellGO = Instantiate(frameCellPrefab);
            cellGO.name = "Frame";
            cellGO.transform.parent = cellsParent;
            cellGO.transform.position = new Vector3(x, -1, 0);
		}

        for(int x = -1; x <= _map.GetMapSize().x; x++) {
            
            GameObject cellGO = Instantiate(frameCellPrefab);
            cellGO.name = "Frame";
            cellGO.transform.parent = cellsParent;
            cellGO.transform.position = new Vector3(x, _map.GetMapSize().y, 0);
		}

        for(int y = 0; y < _map.GetMapSize().y; y++) {
            
            GameObject cellGO = Instantiate(frameCellPrefab);
            cellGO.name = "Frame";
            cellGO.transform.parent = cellsParent;
            cellGO.transform.position = new Vector3(-1, y, 0);
		}

        for(int y = 0; y < _map.GetMapSize().y; y++) {
            
            GameObject cellGO = Instantiate(frameCellPrefab);
            cellGO.name = "Frame";
            cellGO.transform.parent = cellsParent;
            cellGO.transform.position = new Vector3(_map.GetMapSize().x, y, 0);
		}
	}

	#region Perlin Terrain func

    public void Generate2DPerlinTerrain(int nOctaves, int perlinSeed, float fBias, float seaLevel)
    {
        if(_map == null)
            return;
        
        PerlinNoise pn = new PerlinNoise(perlinSeed);

        float[,] perlinArray = new float[_map.GetMapSize().x, _map.GetMapSize().y];

        pn.Get2DPerlinNoise(_map.GetMapSize().x, _map.GetMapSize().y, nOctaves, fBias, ref perlinArray);

        float minP = 1;
        float maxP = 0;

        for(int x = 0; x < _map.GetMapSize().x; x++) {
            for(int y = 0; y < _map.GetMapSize().y; y++) {
                
                float v = perlinArray[x,y];

                minP = Mathf.Min(v, minP);
                maxP = Mathf.Max(v, maxP);
			}
		}

        float elevationRange = maxP - minP;

        float sea = minP + elevationRange * seaLevel;

        float A = maxP - sea;
        float ground = sea + A * 0.8f;

        int waterCells = 0;
        int groundCells = 0;
        int mountainCells = 0;

        for (int x = 0; x < _map.GetMapSize().x; x++) {
            for(int y = 0; y < _map.GetMapSize().y; y++) {

                float height = perlinArray[x,y];

                GameObject cell = _map.GetCellAt(new Vector2Int(x, y)).GetGameObject();

                if(height < sea) {
                    
                    float maxHeight = sea;
                    float percent = height/maxHeight;

                    float resultR = seaColorMin.r + percent*(seaColorMax.r - seaColorMin.r);
                    float resultG = seaColorMin.g + percent*(seaColorMax.g - seaColorMin.g);
                    float resultB = seaColorMin.b + percent*(seaColorMax.b - seaColorMin.b);
                    Color heightColor = new Color(resultR, resultG, resultB, 1);
                    cell.GetComponent<SpriteRenderer>().color = heightColor;

                    waterCells++;
				} else if(height < ground) {
                    
                    float maxHeight = ground - sea;
                    float percent = (height - sea) / maxHeight;

                    float resultR = groundColorMin.r + percent*(groundColorMax.r - groundColorMin.r);
                    float resultG = groundColorMin.g + percent*(groundColorMax.g - groundColorMin.g);
                    float resultB = groundColorMin.b + percent*(groundColorMax.b - groundColorMin.b);
                    Color heightColor = new Color(resultR, resultG, resultB, 1);
                    cell.GetComponent<SpriteRenderer>().color = heightColor;

                    groundCells++;
				} else {

                    float maxHeight = 1 - ground;
                    float percent = (height - ground) / maxHeight;

                    float resultR = mountainColorMin.r + percent*(mountainColorMax.r - mountainColorMin.r);
                    float resultG = mountainColorMin.g + percent*(mountainColorMax.g - mountainColorMin.g);
                    float resultB = mountainColorMin.b + percent*(mountainColorMax.b - mountainColorMin.b);
                    Color heightColor = new Color(resultR, resultG, resultB, 1);
                    cell.GetComponent<SpriteRenderer>().color = heightColor;

                    mountainCells++;
				}
			}
		}
	}

	#endregion

	#region Perlin Noise func
	public void PerlinNoise1DTerrainGeneration(int nOctaves, int perlinSeed, float fBias)
    {
        if(_map == null)
            return;

        int maxOctavesCount = 0;
        int worldWidth = _map.GetMapSize().x;

        while(worldWidth != 0) {
            worldWidth /= 2;
            maxOctavesCount++;
		}

        if(nOctaves > maxOctavesCount)
            nOctaves = maxOctavesCount;
        
        ClearWorld();

        float defaultChange = 1.0f / _map.GetMapSize().y;

        PerlinNoise pn = new PerlinNoise(perlinSeed);

        float[] perlinArray = new float[_map.GetMapSize().x];

        pn.Get1DPerlinNoise(_map.GetMapSize().x, nOctaves, fBias, ref perlinArray);

        for (int x = 0; x < _map.GetMapSize().x; x++) {

            int height = (int) (perlinArray[x] * _map.GetMapSize().y);

            for(int y = 0; y < height; y++) {
                
                GameObject cell = _map.GetCellAt(new Vector2Int(x, y)).GetGameObject();

                float f = defaultChange * y;
                Color heightColor = new Color(f, f, f, 1);
                cell.GetComponent<SpriteRenderer>().color = heightColor;
			}
		}
	}

    public void PerlinNoise2DTerrainGeneration(int nOctaves, int perlinSeed, float fBias)
    {
        if(_map == null)
            return;

        int maxOctavesCount = 0;
        int worldWidth = _map.GetMapSize().x;

        while(worldWidth != 0) {
            worldWidth /= 2;
            maxOctavesCount++;
		}

        if(nOctaves > maxOctavesCount)
            nOctaves = maxOctavesCount;
        
        PerlinNoise pn = new PerlinNoise(perlinSeed);

        float[,] perlinArray = new float[_map.GetMapSize().x, _map.GetMapSize().y];

        pn.Get2DPerlinNoise(_map.GetMapSize().x, _map.GetMapSize().y, nOctaves, fBias, ref perlinArray);

        for (int x = 0; x < _map.GetMapSize().x; x++) {
            for(int y = 0; y < _map.GetMapSize().y; y++) {

                float height = perlinArray[x,y];

                GameObject cell = _map.GetCellAt(new Vector2Int(x, y)).GetGameObject();

                Color heightColor = new Color(height, height, height, 1);
                cell.GetComponent<SpriteRenderer>().color = heightColor;
			}
		}
	}
    
    public void PerlinNoise2DTerrainGeneration(int nOctaves, int perlinSeed, float fBias, float scale)
    {
        if(_map == null)
            return;

        int maxOctavesCount = 0;
        
        int scaledX = (int) (_map.GetMapSize().x * 1);
        int scaledY = (int) (_map.GetMapSize().y * 1);

        int worldWidth = scaledX;

        while(worldWidth != 0) {
            worldWidth /= 2;
            maxOctavesCount++;
		}

        if(nOctaves > maxOctavesCount)
            nOctaves = maxOctavesCount;
        
        PerlinNoise pn = new PerlinNoise(perlinSeed);

        float[,] perlinArray = new float[scaledX, scaledY];

        pn.Get2DPerlinNoise(scaledX, scaledY, nOctaves, fBias, ref perlinArray);

        for (int x = 0; x < _map.GetMapSize().x; x++) {
            for(int y = 0; y < _map.GetMapSize().y; y++) {

                float height = perlinArray[x,y] * scale;

                GameObject cell = _map.GetCellAt(new Vector2Int(x, y)).GetGameObject();

                Color heightColor = new Color(height, height, height, 1);
                cell.GetComponent<SpriteRenderer>().color = heightColor;
			}
		}
	}

    public IEnumerator PerlinNoise1DTerrainAnimation(int nOctaves, int perlinSeed, float fBias)
    {
        if(_map == null)
            yield break;

        int maxOctavesCount = 0;
        int worldWidth = _map.GetMapSize().x;

        while(worldWidth != 0) {
            worldWidth /= 2;
            maxOctavesCount++;
		}

        if(nOctaves > maxOctavesCount)
            nOctaves = maxOctavesCount;

        
        float defaultChange = 1.0f / _map.GetMapSize().y;

        PerlinNoise pn = new PerlinNoise(perlinSeed);

        int nFrames = 250;

        float[,] perlinArray = new float[_map.GetMapSize().x, nFrames];

        pn.Get2DPerlinNoise(_map.GetMapSize().x, nFrames, nOctaves, fBias, ref perlinArray);

        for(int i = 0; i < nFrames; i++) {
            for (int x = 0; x < _map.GetMapSize().x; x++) {

                int height = (int) (perlinArray[x, i] * _map.GetMapSize().y);

                for(int y = 0; y < height; y++) {
                
                    GameObject cell = _map.GetCellAt(new Vector2Int(x, y)).GetGameObject();

                    float f = defaultChange * y;
                    Color heightColor = new Color(f, f, f, 1);
                    cell.GetComponent<SpriteRenderer>().color = heightColor;
			    }

                for(int y = height; y < _map.GetMapSize().y; y++) {
                    GameObject cell = _map.GetCellAt(new Vector2Int(x, y)).GetGameObject();
                    cell.GetComponent<SpriteRenderer>().color = Color.white;
				}
		    }
            yield return null;
	    }
    }
	
    public IEnumerator PerlinNoise2DTerrainAnimation(int nOctaves, int perlinSeed, float fBias)
    {
        if(_map == null)
            yield break;

        int maxOctavesCount = 0;
        int worldWidth = _map.GetMapSize().x;

        while(worldWidth != 0) {
            worldWidth /= 2;
            maxOctavesCount++;
		}

        if(nOctaves > maxOctavesCount)
            nOctaves = maxOctavesCount;

        
        PerlinNoise pn = new PerlinNoise(perlinSeed);

        int nFrames = 100;

        float[,,] perlinArray = new float[_map.GetMapSize().x, _map.GetMapSize().y, nFrames];

        pn.Get3DPerlinNoise(_map.GetMapSize().x, _map.GetMapSize().y, nFrames, nOctaves, fBias, ref perlinArray);

        for(int i = 0; i < nFrames; i++) {
            for (int x = 0; x < _map.GetMapSize().x; x++) {
                for(int y = 0; y < _map.GetMapSize().y; y++) {

                    float height = perlinArray[x, y, i];
                    GameObject cell = _map.GetCellAt(new Vector2Int(x, y)).GetGameObject();
                    Color heightColor = new Color(height, height, height, 1);
                    cell.GetComponent<SpriteRenderer>().color = heightColor;
				}
		    }

            yield return null;
	    }
    }
    #endregion

	#region Random noise func
	public IEnumerator StartRandomNoise1DTerrainGeneration(bool isBlackAndWhite)
    {
        if(_map == null)
            yield break;

        ClearWorld();
        RandomNoise rn = new RandomNoise(seed);

        float rNorm = earthCellPrefab.GetComponent<SpriteRenderer>().color.r;
        float gNorm = earthCellPrefab.GetComponent<SpriteRenderer>().color.g;
        float bNorm = earthCellPrefab.GetComponent<SpriteRenderer>().color.b;

        float rChange = rNorm/_map.GetMapSize().y;
        float gChange = gNorm/_map.GetMapSize().y;
        float bChange = bNorm/_map.GetMapSize().y;
        float defaultChange = 1.0f/_map.GetMapSize().y;

        for (int x = 0; x < _map.GetMapSize().x; x++) {

            int height = rn.RandomNoise1D(_map.GetMapSize().y);

            for(int y = 0; y <= height; y++) {
                    
                Vector2Int position = new Vector2Int(x, y);

                GameObject cell = _map.GetCellAt(position).GetGameObject();
                
                Color heightColor = Color.red;

                if(!isBlackAndWhite) {
                    heightColor = new Color(rChange * height, gChange * height, bChange * height, 1);
                } else {
                    float f = defaultChange * height;
                    heightColor = new Color(f, f, f, 1);
                }

                cell.GetComponent<SpriteRenderer>().color = heightColor;
			}
		}
	}

    public IEnumerator StartRandomNoise2DTerrainGeneration(bool isBlackAndWhite)
    {
        if(_map == null)
            yield break;

        RandomNoise rn = new RandomNoise(seed);

        float rNorm = earthCellPrefab.GetComponent<SpriteRenderer>().color.r;
        float gNorm = earthCellPrefab.GetComponent<SpriteRenderer>().color.g;
        float bNorm = earthCellPrefab.GetComponent<SpriteRenderer>().color.b;

        float rChange = rNorm/_map.GetMapSize().y;
        float gChange = gNorm/_map.GetMapSize().y;
        float bChange = bNorm/_map.GetMapSize().y;

        float defaultChange = 1.0f/_map.GetMapSize().y;

        for(int x = 0; x < _map.GetMapSize().x; x++) {
            for(int y = 0; y < _map.GetMapSize().y; y++) {
                
                int height = rn.RandomNoise1D(_map.GetMapSize().y);

                Vector2Int position = new Vector2Int(x, y);
                GameObject cellGO = _map.GetCellAt(position).GetGameObject();

                Color heightColor = Color.red;
                if(!isBlackAndWhite) {
                    heightColor = new Color(rChange * height, gChange * height, bChange * height, 1);
				} else {
                    float f = defaultChange * height;
                    heightColor = new Color(f, f, f);
				}


                cellGO.GetComponent<SpriteRenderer>().color = heightColor;
			}
		}

        yield break;
	}

    public IEnumerator StartRandomNoise1DTerrainAnimation()
    {
        for(int i = 0; i < 1000; i++) {
            yield return StartCoroutine(StartRandomNoise1DTerrainGeneration(true));
            seed--;
            yield return null;
		}
	}

    public IEnumerator StartRandomNoise2DTerrainAnimation()
    {
        for(int i = 0; i < 1000; i++) {
            yield return StartCoroutine(StartRandomNoise2DTerrainGeneration(true));
            seed--;
            yield return null;
		}
	}
	#endregion

	#region Dijkstra terrain
	public IEnumerator StartTerrainGenerationDijkstra(int e, int s, int w)
    {
        if(_map == null)
            yield break;

        ClearWorld();

        List<Vector2Int> eStartPositions = new List<Vector2Int>();
        List<Vector2Int> sStartPositions = new List<Vector2Int>();
        List<Vector2Int> wStartPositions = new List<Vector2Int>();

        int controlE = e;
        int controlS = s;
        int controlW = w;

        for(int i = 0; i < e+s+w; i++) {

            Vector2Int startPosition = new Vector2Int( (int) Random.Range(0, _map.GetMapSize().x), (int) Random.Range(0, _map.GetMapSize().y));

            while(eStartPositions.Contains(startPosition)
                || sStartPositions.Contains(startPosition)
                || wStartPositions.Contains(startPosition)) {

                startPosition = new Vector2Int( (int) Random.Range(0, _map.GetMapSize().x), (int) Random.Range(0, _map.GetMapSize().y));
			}

            if(controlE > 0) {
                controlE--;
                eStartPositions.Add(startPosition);
			} else if(controlS > 0) {
                controlS--;
                sStartPositions.Add(startPosition);
			} else if(controlW > 0) {
                controlW--;
                wStartPositions.Add(startPosition);
			}
		}

        List<List<Cell>> earthUncheckedCells = new List<List<Cell>>();
        List<List<Cell>> sandUncheckedCells = new List<List<Cell>>();
        List<List<Cell>> waterUncheckedCells = new List<List<Cell>>();

        for(int i = 0; i < e; i++) {
            earthUncheckedCells.Add(new List<Cell>());
            earthUncheckedCells[i].Add(_map.GetCellAt(eStartPositions[i]));
		}

        for(int i = 0; i < s; i++) {
            sandUncheckedCells.Add(new List<Cell>());
            sandUncheckedCells[i].Add(_map.GetCellAt(sStartPositions[i]));
		}

        for(int i = 0; i < w; i++) {
            waterUncheckedCells.Add(new List<Cell>());
            waterUncheckedCells[i].Add(_map.GetCellAt(wStartPositions[i]));
		}

        int checkedCells = 0;

        while(checkedCells != _map.GetMapSize().x * _map.GetMapSize().y)
        {
            for(int i = 0; i < earthUncheckedCells.Count; i++) {
                checkedCells += NextDijkstraTerrainIteration(ref earthUncheckedCells, i, Cell.TERRAIN_TYPE.earth, eStartPositions[i]);
			}

            for(int i = 0; i < sandUncheckedCells.Count; i++) {
                checkedCells += NextDijkstraTerrainIteration(ref sandUncheckedCells, i, Cell.TERRAIN_TYPE.sand, sStartPositions[i]);
			}

            for(int i = 0; i < waterUncheckedCells.Count; i++) {
                checkedCells += NextDijkstraTerrainIteration(ref waterUncheckedCells, i, Cell.TERRAIN_TYPE.water, wStartPositions[i]);
			}

            if(render)
                yield return null; 
		}
	}

    private int NextDijkstraTerrainIteration(ref List<List<Cell>> availableCellsList, int listIndex, Cell.TERRAIN_TYPE desiredTerrain, Vector2Int startCell)
    {
        List<Cell> availableCells = availableCellsList[listIndex];

        if(availableCells.Count == 0)
            return 0;
        
        //Choosing initial cell
        Cell initialCell = null;

        float minDistance = 100000;
        int indexToClosest = -1;

        for(int i = availableCells.Count - 1; i >= 0; i--)
        {
            Cell c = availableCells[i];

            Vector2Int dist = c.GetPosition() - startCell;
            float d = dist.magnitude;

            if(d < minDistance && c.GetTerrainType() == Cell.TERRAIN_TYPE.empty) {
                minDistance = d;
                indexToClosest = i;
			}
		}

        if(indexToClosest == -1)
            return 0;

        initialCell = availableCells[indexToClosest];

        for(int i = availableCells.Count - 1; i >= 0; i--)
        {
            Cell c = availableCells[i];
            if(c.GetTerrainType() != Cell.TERRAIN_TYPE.empty || c == initialCell) {
                availableCells.RemoveAt(i);
                continue;
			}
		}
        //////

        //Adding surrounding cells to available list
        for (int x = initialCell.GetPosition().x - 1; x <= initialCell.GetPosition().x + 1; x++) {
            for(int y = initialCell.GetPosition().y - 1; y <= initialCell.GetPosition().y + 1; y++) {

                Vector2Int checkPosition = new Vector2Int(x, y);

                if(checkPosition == initialCell.GetPosition()
                   || !_map.IsPositionViable(checkPosition) 
                   || availableCells.Contains(_map.GetCellAt(checkPosition))
                   || _map.GetCellAt(checkPosition).GetTerrainType() != Cell.TERRAIN_TYPE.empty)
                    continue;

                availableCells.Insert(0, _map.GetCellAt(checkPosition));
			}
		}
        //////

        initialCell.SetTerrainType(desiredTerrain);
        if(desiredTerrain == Cell.TERRAIN_TYPE.earth) {
            initialCell.GetGameObject().GetComponent<SpriteRenderer>().color = earthCellPrefab.GetComponent<SpriteRenderer>().color;
		} else if(desiredTerrain == Cell.TERRAIN_TYPE.sand) {
            initialCell.GetGameObject().GetComponent<SpriteRenderer>().color = sandCellPrefab.GetComponent<SpriteRenderer>().color;
		} else {
            initialCell.GetGameObject().GetComponent<SpriteRenderer>().color = waterCellPrefab.GetComponent<SpriteRenderer>().color;
		}

        return 1;
	}
	#endregion
}
