using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder
{
    private int grid_width, grid_height;
    private ANode[,] grid;
    private Map _map;

    public List<ANode> path;
    public List<ANode> visited;

    public AStarPathfinder(Map map) {

        this.grid_width = map.GetMapSize().x;
        this.grid_height = map.GetMapSize().y;

        _map = map;

        CreateGrid();
	}

    private void CreateGrid()
    {
        grid = new ANode[grid_width, grid_height];
        path = new List<ANode>();
        visited = new List<ANode>();

        for(int x = 0; x < grid_width; x++) {
            for(int y = 0; y < grid_height; y++) {

                Vector2Int pos = new Vector2Int(x, y);
                Cell c = _map.GetCellAt(pos);

                grid[x, y] = new ANode(c);
            }
        }
    }
    
    public bool FindPath(Cell start, Cell target) {
    
        ANode startNode = grid[start.GetPosition().x, start.GetPosition().y];
        ANode targetNode = grid[target.GetPosition().x, target.GetPosition().y];

        List<ANode> openSet = new List<ANode>();
        List<ANode> closedSet = new List<ANode>();

        openSet.Add(startNode);

        while (openSet.Count > 0) {

            ANode currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++) {
                
                if((openSet[i].fCost() < currentNode.fCost()
                   || openSet[i].fCost() == currentNode.fCost())
                   && openSet[i].hCost < currentNode.hCost) {
                   
                   currentNode = openSet[i];
				}
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            visited.Add(currentNode);

            if (currentNode == targetNode) {
                path = RetracePath(startNode, targetNode);
                return true;
            }

            foreach (ANode neighbour in GetNeighbours(currentNode)) {

                if( !(neighbour.walkable) || closedSet.Contains(neighbour))
                    continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {

                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return false;
    }

    private List<ANode> RetracePath(ANode startNode, ANode endNode)
    {
        List<ANode> path = new List<ANode>();
        ANode currentNode = endNode;

        while(currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        return path;
	}

    private int GetDistance(ANode A, ANode B)
    {
        int dstX = Mathf.Abs(A.x - B.x);
        int dstY = Mathf.Abs(A.y - B.y);

        //14 - cost of traversing towards diagonal node
        //10 - horizontal/vertical node

        if(dstX > dstY)
            return 14*dstY + 10*(dstX - dstY);

        return 14*dstX + 10 * (dstY - dstX);
	}

    private List<ANode> GetNeighbours(ANode node)
    {
        List<ANode> neighbours = new List<ANode>();

        Vector2Int checkPosition = new Vector2Int(node.x, node.y - 1);
        if(_map.IsPositionViable(checkPosition))
            neighbours.Add(grid[checkPosition.x, checkPosition.y]);

        checkPosition.x = node.x;
        checkPosition.y = node.y + 1;
        if(_map.IsPositionViable(checkPosition))
            neighbours.Add(grid[checkPosition.x, checkPosition.y]);

        checkPosition.x = node.x - 1;
        checkPosition.y = node.y;
        if(_map.IsPositionViable(checkPosition))
            neighbours.Add(grid[checkPosition.x, checkPosition.y]);

        checkPosition.x = node.x + 1;
        checkPosition.y = node.y;
        if(_map.IsPositionViable(checkPosition))
            neighbours.Add(grid[checkPosition.x, checkPosition.y]);

       return neighbours;
	}
}

public class ANode
{
    public bool walkable;
    public int x,y;
    public int gCost, hCost;
    public ANode parent;
    public Cell c;

    public ANode(Cell c) {
        
        this.c = c;
        walkable = c.IsWalkable();
        x = c.GetPosition().x;
        y = c.GetPosition().y;
    }

    public int fCost() {

        return gCost + hCost;
    }
};
