using System.Collections.Generic;
using UnityEngine;

public class BSPNode
{
	const int NUM_CHILDREN = 2;
	public BSPNode parent;
	public BSPNode root;
	public BSPNode[] children = new BSPNode[NUM_CHILDREN];
	public BSPNode sibling;
	public bool doSplitVertically;
	public bool isResultOfVerticalSplit;
	public int maxLevel = 0;

	public RectInt rect;
	public Rect room;
	public Rect corridor;
	public List<Vector2Int> doors = new List<Vector2Int>();

	public TileType tileType;
	public int Level { get; private set; }

	public BSPNode(BSPNode parent, RectInt rect, bool doSplitVertically)
	{
		this.parent = parent;
		this.rect = rect;
		this.doSplitVertically = doSplitVertically;
		isResultOfVerticalSplit = !doSplitVertically;
		if (parent != null)
		{
			Level = parent.Level + 1;
			root = parent.root;
			root.maxLevel = Level;
		}
		else
		{
			Level = 0;
			root = this;
		}
	}

	public BSPNode[] SplitNode(float percent)// TODO hardcoden, da immer 2 children
	{
		//RectInt newRect;
		//for (int i = 0; i < NUM_CHILDREN; i++)
		//{
		//	newRect = doSplitVertically ?
		//		//new Rect((int)rect.x + (i * rect.width * percent), (int)rect.y, (int)rect.width * (i + percent - i * 2 * percent), (int)rect.height) :
		//		//new Rect((int)rect.x, (int)rect.y + (i * rect.height * percent), (int)rect.width, (int)rect.height * (i + percent - i * 2 * percent));
		//		new RectInt((int)(rect.x + (i * rect.width * percent)), rect.y, (int)(rect.width * (i + percent - i * 2 * percent)), rect.height) :
		//		 new RectInt(rect.x, (int)(rect.y + (i * rect.height * percent)), rect.width, (int)(rect.height * (i + percent - i * 2 * percent)));
		//	children[i] = new BSPNode(this, newRect, !doSplitVertically);
		//}
		children[0] = new BSPNode(this, new RectInt(), !doSplitVertically);
		children[1] = new BSPNode(this, new RectInt(), !doSplitVertically);
		int width = Mathf.CeilToInt(rect.width * percent);
		int height = Mathf.CeilToInt(rect.height * percent);
		children[0].rect = doSplitVertically ? new RectInt(rect.x, rect.y, width, rect.height) : new RectInt(rect.x, rect.y, rect.width, height);
		children[1].rect = doSplitVertically ? new RectInt(rect.x + children[0].rect.width, rect.y, rect.width - children[0].rect.width, rect.height) : new RectInt(rect.x, (rect.y + children[0].rect.height), rect.width, rect.height - children[0].rect.height);

		children[0].sibling = children[1];
		children[1].sibling = children[0];
		return children;
	}


	public void FillListWithChildren(List<BSPNode> nodeList)
	{
		if (children[0] != null)
		{
			for (int i = 0; i < children.Length; i++)
			{
				nodeList.Add(children[i]);
				children[i].FillListWithChildren(nodeList);
			}
		}
	}
}

