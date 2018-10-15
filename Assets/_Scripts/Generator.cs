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
	GameObject floor;
	Dictionary<TileType, GameObject> roomTypeDict;

	BSPTree bsp;
	GameGrid grid;
	bool vis = false;

	void Start()
	{
		roomTypeDict = new Dictionary<TileType, GameObject>() { { TileType.Wall, wallPrefab }, { TileType.Room, floorPrefab }, { TileType.Corridor, floorPrefab }, { TileType.Door, doorPrefab } };
		CreateBSPTree();
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
		bsp.CreateNodeLists();//TODO moeglicher refactor
		CreateGeometry();		
		vis = true;
	}

	void CreateGeometry()
	{			
		foreach (BSPNode node in bsp.Leafs)
		{
			GameObject wallParent = new GameObject();
			wallParent.name = "WallParent";
			Color color = Random.ColorHSV();
			int width = Mathf.CeilToInt(node.rect.width * 0.5f);
			int height = Mathf.CeilToInt(node.rect.height * 0.5f);
			floor = Instantiate(floorPrefab);
			floor.transform.position = new Vector3Int(node.rect.x + width, 0, node.rect.y + height);
			floor.transform.localScale = new Vector3Int(node.rect.width, 1, node.rect.height);

			for (int i = 0; i < node.rect.width; i++)
			{
				GameObject wallTile = Instantiate(wallPrefab, wallParent.transform);
				wallTile.transform.position = new Vector3Int(node.rect.x + i, 0, node.rect.y + node.rect.height);
				wallTile.transform.localScale = new Vector3Int(1, 5, 1); //just for testing
				wallTile.transform.GetChild(0).GetComponent<Renderer>().material.color = color;
			}
			for (int i = 0; i < node.rect.height; i++)
			{
				GameObject wallTile = Instantiate(wallPrefab, wallParent.transform);
				wallTile.transform.position = new Vector3Int(node.rect.x, 0, node.rect.y + node.rect.height - i);
				wallTile.transform.localScale = new Vector3Int(1, 5, 1); //just for testing
				wallTile.transform.GetChild(0).GetComponent<Renderer>().material.color = color;
			}
		}

		GameObject wallParentEnd = new GameObject();
		wallParentEnd.name = "WallParent";
		Color colorEnd = Random.ColorHSV();
		for (int i = 0; i < bsp.root.rect.width; i++)
		{
			GameObject wallTile = Instantiate(wallPrefab, wallParentEnd.transform);
			wallTile.transform.position = new Vector3Int(bsp.root.rect.x + i, 0, bsp.root.rect.y);
			wallTile.transform.localScale = new Vector3Int(1, 5, 1); //just for testing
			wallTile.transform.GetChild(0).GetComponent<Renderer>().material.color = colorEnd;
		}
		for (int i = 0; i < bsp.root.rect.height; i++)
		{
			GameObject wallTile = Instantiate(wallPrefab, wallParentEnd.transform);
			wallTile.transform.position = new Vector3Int(bsp.root.rect.x + bsp.root.rect.width, 0, bsp.root.rect.y + i);
			wallTile.transform.localScale = new Vector3Int(1, 5, 1); //just for testing
			wallTile.transform.GetChild(0).GetComponent<Renderer>().material.color = colorEnd;
		}
	}

	void OnDrawGizmos()
	{
		if (!vis) { return; }

		Gizmos.color = Color.white;
		foreach (BSPNode node in bsp.Leafs)
		{
			Gizmos.DrawWireCube(new Vector3(node.rect.x + (node.rect.width * 0.5f), 0f, node.rect.y + (node.rect.height * 0.5f)), new Vector3(node.rect.width, 0f, node.rect.height));
		}
	}
}
