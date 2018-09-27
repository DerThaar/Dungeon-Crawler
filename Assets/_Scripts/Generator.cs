using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
	public int width = 230;
	public int height = 200;
	public int minNodeWidth = 45;
	public int minNodeHeight = 40;

	BSP_Tree bsp;
	List<Rect> rooms = new List<Rect>(32);
	bool vis = false;


	void Update()
	{
		if (Input.GetKey(KeyCode.Return))
		{
			bsp = new BSP_Tree(width, height, minNodeWidth, minNodeHeight);
			Debug.Log($"all nodes count: {bsp.allNodes.Count}");
			bsp.CreateNodeLists();
			bsp.GenerateRooms();
			bsp.GenrateCorridors();
			vis = true; 
		}
	}

	void OnDrawGizmos()
	{
		if (!vis) { return; }

		Gizmos.color = Color.white;
		//foreach (BSP_Node node in bsp.Leafs)
		//{
		//	Gizmos.DrawWireCube(new Vector3(node.rect.x + (node.rect.width * 0.5f), 0f, node.rect.y + (node.rect.height * 0.5f)), new Vector3(node.rect.width, 0f, node.rect.height));
		//}

		Gizmos.color = Color.green;
		foreach (BSP_Node node in bsp.allNodes)
		{
			Gizmos.DrawWireCube(new Vector3(node.room.x + (node.room.width * 0.5f), 0f, node.room.y + (node.room.height * 0.5f)), new Vector3(node.room.width, 0f, node.room.height));
		}		
	}
}
