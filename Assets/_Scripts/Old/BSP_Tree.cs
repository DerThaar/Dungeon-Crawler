using System.Collections.Generic;
using UnityEngine;

public class BSP_Tree
{
	public BSP_Node root;
	public List<BSP_Node> allNodes = new List<BSP_Node>(32);
	List<List<BSP_Node>> nodeLists = new List<List<BSP_Node>>();
	int minNodeWidth;
	int minNodeHeight;
	int numberOfCorridors;

	List<BSP_Node> leafs = new List<BSP_Node>(32);
	public List<BSP_Node> Leafs { get { return leafs; } }
	public List<BSP_Node>[] levels;

	public BSP_Tree(int width, int height, int minNodeWidth, int minNodeHeight)
	{
		root = new BSP_Node(null, new Rect(0f, 0f, width, height), true);
		this.minNodeWidth = minNodeWidth;
		this.minNodeHeight = minNodeHeight;
		Split(root);
	}

	public void Split(BSP_Node node)
	{
		allNodes.Add(node);
		bool vertical = !node.splitVertical;
		bool bigEnough = vertical ? node.rect.width > minNodeWidth : node.rect.height > minNodeHeight;

		if (!bigEnough)
			leafs.Add(node);
		else
		{
			float percent = Random.Range(0.3f, 0.7f);
			BSP_Node[] currentChildren = node.SplitNode(percent, vertical);

			for (int i = 0; i < currentChildren.Length; i++)
			{
				Split(currentChildren[i]);
			}
		}
	}

	public void CreateNodeLists()
	{
		for (int i = 0; i < allNodes.Count; i++)
		{
			if (nodeLists.Count <= allNodes[i].Level)
				nodeLists.Add(new List<BSP_Node>());

			nodeLists[allNodes[i].Level].Add(allNodes[i]);
		}
	}

	public void GenerateRooms()
	{
		for (int i = 0; i < leafs.Count; i++)
		{
			var leafRect = leafs[i].rect;
			float newWidth = leafRect.width * Random.Range(0.6f, 0.8f);
			float newHeight = leafRect.height * Random.Range(0.6f, 0.8f);
			float xMargin = leafRect.width - newWidth;
			xMargin *= Random.Range(0.2f, 0.6f);
			float yMargin = leafRect.height - newHeight;
			yMargin *= Random.Range(0.2f, 0.6f);
			int quantizedX = Mathf.CeilToInt((int)leafRect.x + xMargin);
			int quantizedY = Mathf.CeilToInt((int)leafRect.y + yMargin);
			int quantizedWidth = Mathf.FloorToInt(newWidth);
			int quantizedHeight = Mathf.FloorToInt(newHeight);
			leafs[i].room = new Rect(quantizedX, quantizedY, quantizedWidth, quantizedHeight);			
		}
	}

	public void GenrateCorridors()
	{
		// step through levels
		for (int i = nodeLists.Count - 1; i > 0; i--)
		{
			for (int j = 0; j < nodeLists[i].Count; j++)
			{
				GenerateCorridor2(nodeLists[i][j]);
			}
		}
		Debug.Log("number of corridors: " + numberOfCorridors.ToString());
	}

	void GenerateCorridor2(BSP_Node node)
	{
		BSP_Node sibling = node.sibling;
		int minX;
		int maxX;
		int minY;
		int maxY;

		if (node.splitVertical)
		{
			bool nodeLeftOfSibling = node.rect.center.x > sibling.rect.center.x;
			if (!nodeLeftOfSibling) { return; }

			List<BSP_Node> candidates = new List<BSP_Node>();
			candidates.Add(node);
			node.FillListWithChildren(candidates);

			BSP_Node rightmostNode = FindRightmostNode(candidates);

			candidates = new List<BSP_Node>();
			candidates.Add(sibling);
			sibling.FillListWithChildren(candidates);
			List<BSP_Node> yOverlapping = FindYOverlapping(rightmostNode, candidates);
			if (yOverlapping.Count == 0) { return; }
			numberOfCorridors++;

			BSP_Node bestSibling = FindLeftmostNode(yOverlapping);

			Rect leftRoom = rightmostNode.room;
			Rect rightRoom = bestSibling.room;
			minY = (int)(Mathf.Max(leftRoom.yMin, rightRoom.yMin) + halfYOverlap(leftRoom, rightRoom));
			maxY = minY + 1;
			maxX = (int)rightmostNode.room.xMin;
			minX = (int)bestSibling.room.xMax;
		}
		else
		{
			bool nodeLowerThanSibling = node.room.center.y < sibling.room.center.y;
			if (!nodeLowerThanSibling) { return; }

			List<BSP_Node> candidates = new List<BSP_Node>();
			candidates.Add(node);
			node.FillListWithChildren(candidates);

			BSP_Node topmostNode = FindTopmostNode(candidates);

			candidates = new List<BSP_Node>();
			candidates.Add(sibling);
			sibling.FillListWithChildren(candidates);
			List<BSP_Node> xOverlapping = FindXOverlapping(topmostNode, candidates);
			if (xOverlapping.Count == 0) { return; }
			numberOfCorridors++;

			BSP_Node bestSibling = FindBottommostNode(xOverlapping);

			Rect topRoom = topmostNode.room;
			Rect bottomRoom = bestSibling.room;
			minX = (int)(Mathf.Max(topRoom.xMin, bottomRoom.xMin) + halfXOverlap(topRoom, bottomRoom));
			maxX = minX + 1;
			minY = (int)topmostNode.room.yMax;
			maxY = (int)bestSibling.room.yMin;
		} 
		node.parent.room = new Rect(minX, minY, maxX - minX, maxY - minY);
	}

	BSP_Node FindRightmostNode(List<BSP_Node> candidates)
	{
		BSP_Node rightmostNode = candidates[0];
		for (int i = 1; i < candidates.Count; i++)
		{
			if (candidates[i].room.xMax < rightmostNode.room.xMax)
				rightmostNode = candidates[i];
		}
		return rightmostNode;
	}

	BSP_Node FindLeftmostNode(List<BSP_Node> candidates)
	{
		BSP_Node leftmostNode = candidates[0];
		for (int i = 1; i < candidates.Count; i++)
		{
			if (candidates[i].room.xMin > leftmostNode.room.xMin)
				leftmostNode = candidates[i];
		}
		return leftmostNode;
	}

	BSP_Node FindTopmostNode(List<BSP_Node> candidates)
	{
		BSP_Node topmostNode = candidates[0];
		for (int i = 1; i < candidates.Count; i++)
		{
			if (candidates[i].room.yMin > topmostNode.room.yMin)
				topmostNode = candidates[i];
		}
		return topmostNode;
	}

	BSP_Node FindBottommostNode(List<BSP_Node> candidates)
	{
		BSP_Node bottommostNode = candidates[0];
		for (int i = 1; i < candidates.Count; i++)
		{
			if (candidates[i].room.yMax < bottommostNode.room.yMax)
				bottommostNode = candidates[i];
		}
		return bottommostNode;
	}

	List<BSP_Node> FindYOverlapping(BSP_Node node, List<BSP_Node> candidates)
	{
		List<BSP_Node> yOverlapping = new List<BSP_Node>();
		for (int i = 0; i < candidates.Count; i++)
		{
			float overlappingSize = halfYOverlap(node.room, candidates[i].room);
			if (overlappingSize > 0)
				yOverlapping.Add(candidates[i]);
		}
		return yOverlapping;
	}

	List<BSP_Node> FindXOverlapping(BSP_Node node, List<BSP_Node> candidates)
	{
		List<BSP_Node> xOverlapping = new List<BSP_Node>();
		for (int i = 0; i < candidates.Count; i++)
		{
			float overlappingSize = halfXOverlap(node.room, candidates[i].room);
			if (overlappingSize > 0)
				xOverlapping.Add(candidates[i]);
		}
		return xOverlapping;
	}

	float halfYOverlap(Rect rect1, Rect rect2)
	{
		float overlappingSize = Mathf.Min(rect1.yMax, rect2.yMax) - Mathf.Max(rect1.yMin, rect2.yMin);
		return overlappingSize * 0.5f;
	}

	float halfXOverlap(Rect rect1, Rect rect2)
	{
		float overlappingSize = Mathf.Min(rect1.xMax, rect2.xMax) - Mathf.Max(rect1.xMin, rect2.xMin);
		return overlappingSize * 0.5f;
	}
}

