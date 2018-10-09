using System.Collections.Generic;
using UnityEngine;

public class BSP_Node
{
	public BSP_Node parent;
	public BSP_Node root;
	public BSP_Node[] children = new BSP_Node[2];
	public BSP_Node sibling;
	public bool splitVertical;
	public int maxLevel = 0;

	public Rect rect;
	public Rect room;
	public Rect corridor;	

	public int Level { get; private set; }

	public BSP_Node(BSP_Node parent, Rect rect, bool splitVertical)
	{
		this.parent = parent;
		this.rect = rect;
		this.splitVertical = splitVertical;

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

	public BSP_Node[] SplitNode(float percent, bool vertical)
	{
		for (int i = 0; i < children.Length; i++)
		{
			Rect newRect;
			if (i == 0)
			{
				if (!vertical)
					newRect = new Rect(rect.x, rect.y, rect.width, rect.height * percent);
				else
					newRect = new Rect(rect.x, rect.y, rect.width * percent, rect.height);
			}
			else
			{
				if (!vertical)
					newRect = new Rect(rect.x, rect.y + rect.height * percent, rect.width, rect.height * (1 - percent));
				else
					newRect = new Rect(rect.x + rect.width * percent, rect.y, rect.width * (1 - percent), rect.height);
			}

			children[i] = new BSP_Node(this, newRect, vertical);
		}

		children[0].sibling = children[1];
		children[1].sibling = children[0];
		return children;
	}

	public void FillListWithChildren(List<BSP_Node> nodeList)
	{
		if(children[0] != null)
		{
			for (int i = 0; i < children.Length; i++)
			{
				nodeList.Add(children[i]);
				children[i].FillListWithChildren(nodeList);
			}
		}
	}
}
