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

    public Rect rect;
    public Rect room;
    public Rect corridor;
	public List<Vector2Int> doors = new List<Vector2Int>();

    public TileType tileType;
    public int Level { get; private set; }

    public BSPNode(BSPNode parent, Rect rect, bool doSplitVertically)
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
        Rect newRect;
        for (int i = 0; i < NUM_CHILDREN; i++)
        {
            // 1 + percent - 2 * percent = 1 - percent
            // 0 + percent - 0 * percent = percent
            // Alternative: Mathf.Lerp(percent, 1- percent, i)
            newRect = doSplitVertically ?
                 new Rect(rect.x + (i * rect.width * percent), rect.y, rect.width * (i + percent - i * 2 * percent), rect.height) :
                 new Rect(rect.x, rect.y + (i * rect.height * percent), rect.width, rect.height * (i + percent - i * 2 * percent));
            children[i] = new BSPNode(this, newRect, !doSplitVertically);
        }
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

