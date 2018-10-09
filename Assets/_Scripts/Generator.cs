using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
	public Color[] visLevelColors;
	public int width = 250;
	public int height = 200;
	public int minSplitableNodeWidth = 50;
	public int minSplitableNodeHeight = 40;

	public GameObject wallPrefab;
	public GameObject floorPrefab;
	public GameObject doorPrefab;
	Dictionary<TileType, GameObject> roomTypeDict;

	BSPTree bsp;
	GameGrid grid;
	bool vis = false;

	void Start()
	{
		roomTypeDict = new Dictionary<TileType, GameObject>() { { TileType.Wall, wallPrefab }, { TileType.Room, floorPrefab }, { TileType.Corridor, floorPrefab }, { TileType.Door, doorPrefab } };
		CreateBSPTree();
		CreateGameGrid();
		CreateGeometry();
		CreateGeometryFromGameGrid();
	}

	void Update()
	{
		if (Input.GetButtonDown("Jump"))
		{
			vis = false;
			CreateBSPTree();
		}
	}

	void CreateBSPTree()
	{
		bsp = new BSPTree(width, height, minSplitableNodeWidth, minSplitableNodeHeight);
		//Debug.Log($"all nodes count: {bsp.allNodes.Count}");
		bsp.CreateNodeLists();//TODO moeglicher refactor
		bsp.GenerateRooms();
		bsp.GenerateCorridors();
		vis = true;
	}

	void CreateGameGrid()
	{
		grid = new GameGrid();
		grid.Nodes = new Node[width, height];
		for (int i = 0; i < bsp.allNodes.Count; i++)
		{
			if (bsp.allNodes[i].tileType == TileType.Room || bsp.allNodes[i].tileType == TileType.Corridor)
			{
				Rect room = bsp.allNodes[i].room;
				grid.Nodes = SetGridTileTypesRange(grid.Nodes, bsp.allNodes[i].tileType, (int)room.xMin, (int)room.xMax, (int)room.yMin, (int)room.yMax);
				for (int j = 0; j < bsp.allNodes[i].doors.Count; j++)
				{
					Vector2Int coord = bsp.allNodes[i].doors[j];
					grid.Nodes[coord.x, coord.y].TileType = TileType.Door;
				}
			}
		}
	}

	void CreateGeometry()
	{
		GameObject geometry = new GameObject();
		for (int i = 0; i < bsp.allNodes.Count; i++)
		{
			var floorGeo = Instantiate(roomTypeDict[TileType.Room]);
			floorGeo.transform.parent = geometry.transform;
			Rect room = bsp.allNodes[i].room;
			floorGeo.transform.position = new Vector3(room.center.x, 0f, room.center.y);
			floorGeo.transform.localScale = new Vector3(room.size.x, 1f, room.size.y);
			var wallGeo = Instantiate(roomTypeDict[TileType.Wall]);
			wallGeo.transform.parent = geometry.transform;
			wallGeo.transform.position = new Vector3(room.center.x, 0f, room.center.y);
		}
	}

	void CreateGeometryFromGameGrid()
	{
		GameObject geometry = new GameObject();
		for (int y = 0; y < grid.Nodes.GetLength(1); y++)
		{
			for (int x = 0; x < grid.Nodes.GetLength(0); x++)
			{
				var go = Instantiate(roomTypeDict[grid.Nodes[x, y].TileType]);
				go.transform.parent = geometry.transform;
				go.transform.localPosition = new Vector3(x, 0f, y);
			}
		}
	}

	Node[,] SetGridTileTypesRange(Node[,] nodes, TileType tileType, int xMin, int xMax, int yMin, int yMax)
	{
		for (int y = yMin; y < yMax; y++)
		{
			for (int x = xMin; x < xMax; x++)
			{
				nodes[x, y].TileType = tileType;
			}
		}
		return nodes;
	}

	void OnDrawGizmos()
	{
		if (!vis) { return; }

		Gizmos.color = Color.white;
		foreach (BSPNode node in bsp.Leafs)
		{
			Gizmos.DrawWireCube(new Vector3(node.rect.x + (node.rect.width * 0.5f), 0f, node.rect.y + (node.rect.height * 0.5f)), new Vector3(node.rect.width, 0f, node.rect.height));
		}

		foreach (var node in bsp.Leafs)
		{
			Gizmos.DrawWireCube(new Vector3(node.room.x + (node.room.width * 0.5f), 0f, node.room.y + (node.room.height * 0.5f)), new Vector3(node.room.width, 0f, node.room.height));
		}

		Gizmos.color = Color.green;
		foreach (BSPNode node in bsp.allNodes)
		{
			Gizmos.color = visLevelColors[node.Level];
			Gizmos.DrawWireCube(new Vector3(node.room.x + (node.room.width * 0.5f), 0f, node.room.y + (node.room.height * 0.5f)), new Vector3(node.room.width, 0f, node.room.height));
		}
	}
}
