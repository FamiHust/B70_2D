using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//https://github.com/mclift/SimpleAStarExample/blob/master/SimpleAStarExample/Node.cs

public enum NodeState {
	Untested,
	Open,
	Closed
}
	

public class SearchParameters {
	public Vector2 startLocation { get; set; }
	public Vector2 endLocation { get; set; }
	public bool[,] map { get; set; }

	public SearchParameters(Vector2 startLocation, Vector2 endLocation, bool[,] map){
		this.startLocation = startLocation;
		this.endLocation = endLocation;
		this.map = map;
	}
}

public class Node : IComparable<Node>
{
	private Node _parentNode;

	public Vector2 location { get; private set; }

	//True when the node may be traversed, otherwise false
	public bool isWalkable { get; set; }


    // Cost from start to here
    public float G { get; set; }

    // Estimated cost from here to end
    public float H { get; private set; }

	// Flags whether the node is open, closed or untested by the PathFinder
	public NodeState State { get; set; }

	// Estimated total cost (F = G + H)
	public float F {
		get { return this.G + this.H; }
	}

	// Gets or sets the parent node. The start node's parent is always null.
	public Node parentNode {
		get { return this._parentNode; }
		set
		{
			// When setting the parent, also calculate the traversal cost from the start node to here (the 'G' value)
			this._parentNode = value;
            this.G = this._parentNode.G + GetTraversalCost(this.location, this._parentNode.location) * GetTerrainCost();
        }
	}

	public Node(int x, int y, bool isWalkable, Vector2 endLocation){
		this.location = new Vector2(x, y);
		this.State = NodeState.Untested;
		this.isWalkable = isWalkable;
		this.H = GetTraversalCost(this.location, endLocation);
		this.G = 0;
	}


	// Gets the distance between two points
	internal static float GetTraversalCost(Vector2 location, Vector2 otherLocation){
		float deltaX = otherLocation.x - location.x;
		float deltaY = otherLocation.y - location.y;
		return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
	}

    public float GetTerrainCost()
    {
        int x = (int)location.x;
        int y = (int)location.y;

        if (GroundManager.instance.roadNodes[x, y])
            return 1f; // road

        return 8f; // grass penalty
    }

    public int CompareTo(Node other)
    {
        int compare = F.CompareTo(other.F);
        if (compare == 0) compare = H.CompareTo(other.H);
        return compare;
    }
}

public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> data = new List<T>();

    public int Count => data.Count;

    public void Enqueue(T item)
    {
        data.Add(item);
        int ci = data.Count - 1;
        while (ci > 0)
        {
            int pi = (ci - 1) / 2;
            if (data[ci].CompareTo(data[pi]) >= 0) break;
            T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
            ci = pi;
        }
    }

    public T Dequeue()
    {
        int li = data.Count - 1;
        T frontItem = data[0];
        data[0] = data[li];
        data.RemoveAt(li);

        --li;
        int pi = 0;
        while (true)
        {
            int ci = pi * 2 + 1;
            if (ci > li) break;
            int rc = ci + 1;
            if (rc <= li && data[rc].CompareTo(data[ci]) < 0) ci = rc;
            if (data[pi].CompareTo(data[ci]) <= 0) break;
            T tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp;
            pi = ci;
        }
        return frontItem;
    }
}

public class PathFinder{
	private int width;
	private int height;
	private Node[,] nodes;
	private Node startNode;
	private Node endNode;
	private SearchParameters searchParameters;

	public PathFinder(SearchParameters searchParameters){
		this.searchParameters = searchParameters;
		InitializeNodes(searchParameters.map);
		this.startNode = this.nodes[(int)searchParameters.startLocation.x, (int)searchParameters.startLocation.y];
		this.startNode.State = NodeState.Open;
		this.endNode = this.nodes[(int)searchParameters.endLocation.x, (int)searchParameters.endLocation.y];
	}


    public List<Vector2> FindPath()
    {
        PriorityQueue<Node> openList = new PriorityQueue<Node>();

        startNode.State = NodeState.Open;
        openList.Enqueue(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList.Dequeue();
            currentNode.State = NodeState.Closed;

            if (currentNode.location == endNode.location)
            {
                return CalculatePath(currentNode);
            }

            foreach (var neighbor in GetAdjacentWalkableNodes(currentNode))
            {
                if (neighbor.State == NodeState.Closed) continue;

                float distance = Node.GetTraversalCost(currentNode.location, neighbor.location);
                float newG = currentNode.G + distance * neighbor.GetTerrainCost();

                if (neighbor.State != NodeState.Open || newG < neighbor.G)
                {
                    neighbor.G = newG;
                    neighbor.parentNode = currentNode;

                    if (neighbor.State != NodeState.Open)
                    {
                        neighbor.State = NodeState.Open;
                        openList.Enqueue(neighbor);
                    }
                }
            }
        }
        return new List<Vector2>();
    }

    private List<Vector2> CalculatePath(Node node)
    {
        List<Vector2> path = new List<Vector2>();
        while (node.parentNode != null)
        {
            path.Add(node.location);
            node = node.parentNode;
        }
        path.Reverse();
        return path;
    }

    private void InitializeNodes(bool[,] map) {
		this.width = map.GetLength(0);
		this.height = map.GetLength(1);
		this.nodes = new Node[this.width, this.height];
		for (int y = 0; y < this.height; y++){
			for (int x = 0; x < this.width; x++){
				this.nodes[x, y] = new Node(x, y, map[x, y], this.searchParameters.endLocation);
			}
		}
	}

	private bool Search(Node currentNode) {
		// Set the current node to Closed since it cannot be traversed more than once
		currentNode.State = NodeState.Closed;
		List<Node> nextNodes = GetAdjacentWalkableNodes(currentNode);

		// Sort by F-value so that the shortest possible routes are considered first
		nextNodes.Sort((node1, node2) => node1.F.CompareTo(node2.F));
		foreach (var nextNode in nextNodes)
		{
			// Check whether the end node has been reached
			if (nextNode.location == this.endNode.location)
			{
				return true;
			}
			else
			{
				// If not, check the next set of nodes
				if (Search(nextNode)) // Note: Recurses back into Search(Node)
					return true;
			}
		}

		// The method returns false if this path leads to be a dead end
		return false;
	}


    private List<Node> GetAdjacentWalkableNodes(Node fromNode)
    {
        List<Node> walkableNodes = new List<Node>();
        int curX = (int)fromNode.location.x;
        int curY = (int)fromNode.location.y;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = curX + x;
                int checkY = curY + y;

                if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
                {
                    Node node = nodes[checkX, checkY];
                    if (node.isWalkable) walkableNodes.Add(node);
                }
            }
        }
        return walkableNodes;
    }

    private static IEnumerable<Vector2> GetAdjacentLocations(Vector2 fromLocation) {
		return new Vector2[] {
			new Vector2(fromLocation.x-1, fromLocation.y-1),
			new Vector2(fromLocation.x-1, fromLocation.y  ),
			new Vector2(fromLocation.x-1, fromLocation.y+1),
			new Vector2(fromLocation.x,   fromLocation.y+1),
			new Vector2(fromLocation.x+1, fromLocation.y+1),
			new Vector2(fromLocation.x+1, fromLocation.y  ),
			new Vector2(fromLocation.x+1, fromLocation.y-1),
			new Vector2(fromLocation.x,   fromLocation.y-1)
		};
	}
}
